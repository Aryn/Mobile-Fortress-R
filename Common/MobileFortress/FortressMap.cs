using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Common.Base;
using System.IO;

namespace Common.MobileFortress
{
    public enum SurfaceF { EMPTY = 0, NORTH = 1, SOUTH = 2, WEST = 4, EAST = 8, FLOOR = 16, DIAG = 32, COLORA = 64, COLORB = 128 }
    public enum ExtraMaterial { NONE = 0, NORTH = 1, SOUTH = 2, WEST = 4, EAST = 8, FLOOR = 16, CURVE = 32, MATERIALA = 64, MATERIALB = 128 }
    public class FortressMap
    {
        public const string MapVersion = ".MF-1.1";
        public const int MaxMapVertical = 15;
        public const int MaxMapLateral = 40;

        List<MapLayer> layers = new List<MapLayer>();

        public int LayerCount { get { return layers.Count; } }

        private FortressMap()
        {
        }

        #region Factories
        public static FortressMap Empty()
        {
            var map = new FortressMap();
            map.AddEmptyLayer();
            return map;
        }

        public static FortressMap LoadFromPath(string filepath)
        {
            return LoadFromStream(new FileStream(filepath, FileMode.Open));
        }

        public static FortressMap LoadFromStream(Stream stream)
        {
            FortressMap map = new FortressMap();
            Point3D Min, Size;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string version = reader.ReadString();
                if (version != MapVersion)
                {
                    stream.Position = 0;
                    return ConvertFromOldFile(stream);
                }
                Min.X = reader.ReadInt16();
                Min.Y = reader.ReadInt16();
                Min.Z = reader.ReadInt16();
                Size.X = reader.ReadUInt16();
                Size.Y = reader.ReadUInt16();
                Size.Z = reader.ReadUInt16();
                for (int z = 0; z < Size.Z; z++)
                {
                    MapLayer L = new MapLayer();
                    L.ReadFromStream(reader, Min, Size);
                    map.AddLayer(L);
                }
            }
            return map;
        }

        public static FortressMap ConvertFromOldFile(Stream stream)
        {
            FortressMap map = new FortressMap();
            using (BinaryReader r = new BinaryReader(stream))
            {
                byte layers = r.ReadByte();
                byte w = r.ReadByte();
                byte h = r.ReadByte();
                ushort components = r.ReadUInt16();

                for (int L = 0; L < layers; L++)
                {
                    var layer = new MapLayer();
                    for (int y = -h/2; y <= h/2; y++)
                    {
                        for (int x = -w/2; x < w/2; x++)
                        {
                            layer.GetTile(x,y).Surface = (SurfaceF)r.ReadByte();
                        }
                    }
                    map.layers.Add(layer);
                }

                for (int c = 0; c < components; c++)
                {
                    //Load Components
                }
            }
            return map;
        }
        #endregion

        #region Layer and Tile Management
        public void AddEmptyLayer()
        {
            AddLayer(new MapLayer());
        }
        public void AddLayer(MapLayer L)
        {
            layers.Add(L);
        }
        public void InsertLayer(MapLayer L)
        {
            layers.Insert(0, L);
        }
        public void RemoveEmptyLayers()
        {
            MapLayer[] oldLayers = layers.ToArray();
            foreach (MapLayer L in oldLayers)
            {
                if (L.IsEmpty())
                    layers.Remove(L);
            }
        }
        public MapTile GetTile(int x, int y, int z)
        {
            var layer = GetLayer(z);
            return layer.GetTile(x, y);
        }
        public MapLayer GetLayer(int z)
        {
            if (z < 0 || z >= layers.Count) throw new ArgumentOutOfRangeException("Layer not in map: " + z);
            return layers[z];
        }
        #endregion

        #region Saving
        public void Save(string filename)
        {
            SaveToStream(new FileStream(filename, FileMode.Create));
        }
        public void SaveToStream(Stream s)
        {
            Point3D Min, Size;
            FindDimensions(out Min, out Size);
            using (BinaryWriter writer = new BinaryWriter(s))
            {
                writer.Write(MapVersion);
                writer.Write((short)Min.X);
                writer.Write((short)Min.Y);
                writer.Write((short)Min.Z);
                writer.Write((ushort)Size.X);
                writer.Write((ushort)Size.Y);
                writer.Write((ushort)Size.Z);
                foreach (MapLayer layer in layers)
                {
                    layer.WriteToStream(writer, Min, Size);
                }
            }
        }

        public void FindDimensions(out Point3D Min, out Point3D Size)
        {
            Min = Point3D.Zero;
            Point3D Max = Point3D.Zero;
            for (int z = 0; z < layers.Count; z++)
            {
                for (int x = -MaxMapLateral; x <= MaxMapLateral; x++)
                {
                    for (int y = -MaxMapLateral; y <= MaxMapLateral; y++)
                    {
                        MapTile tile = GetTile(x, y, z);
                        if (!tile.IsEmpty())
                        {
                            Min.X = Math.Min(Min.X, x);
                            Min.Y = Math.Min(Min.Y, y);
                            Min.Z = Math.Min(Min.Z, z);
                            Max.X = Math.Max(Max.X, x);
                            Max.Y = Math.Max(Max.Y, y);
                            Max.Z = Math.Max(Max.Z, z);
                        }
                    }
                }
            }
            Size = (Max - Min) + Point3D.One;
        }
        #endregion
    }
    public class MapLayer
    {
        List<MapComponent> Components = new List<MapComponent>();
        MapTile[,] AllTiles = new MapTile[FortressMap.MaxMapLateral*2+1, FortressMap.MaxMapLateral*2+1];
        public MapTile GetTile(int x, int y)
        {
            x += FortressMap.MaxMapLateral;
            y += FortressMap.MaxMapLateral;
            try
            {
                return AllTiles[x, y];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Tile not in layer: (" + x + "," + y + ")", e);
            }
        }

        public MapLayer()
        {
            for (int x = 0; x < FortressMap.MaxMapLateral * 2+1; x++)
            {
                for (int y = 0; y < FortressMap.MaxMapLateral * 2+1; y++)
                {
                    AllTiles[x, y] = new MapTile();
                }
            }
        }

        public void WriteToStream(BinaryWriter writer, Point3D min, Point3D size)
        {
            for (int x = min.X; x < min.X + size.X; x++)
            {
                for (int y = min.Y; y < min.Y + size.Y; y++)
                {
                    MapTile tile = GetTile(x, y);
                    tile.WriteToStream(writer);
                }
            }
            writer.Write((ushort)Components.Count);
            foreach (MapComponent C in Components)
            {
                C.WriteToStream(writer);
            }
        }
        public void ReadFromStream(BinaryReader reader, Point3D min, Point3D size)
        {
            for (int x = min.X; x < min.X + size.X; x++)
            {
                for (int y = min.Y; y < min.Y + size.Y; y++)
                {
                    MapTile tile = GetTile(x, y);
                    tile.ReadFromStream(reader);
                }
            }
            int componentCount = reader.ReadUInt16();
            for(int i = 0; i < componentCount; i++)
            {
                Components.Add(MapComponent.ReadFromStream(reader));
            }
        }
        public bool IsEmpty()
        {
            for (int x = -FortressMap.MaxMapLateral; x <= FortressMap.MaxMapLateral; x++)
            {
                for (int y = -FortressMap.MaxMapLateral; y <= FortressMap.MaxMapLateral; y++)
                {
                    MapTile tile = GetTile(x, y);
                    if (!tile.IsEmpty()) return false;
                }
            }
            return true;
        }
    }
    public class MapTile
    {
        public SurfaceF Surface = SurfaceF.EMPTY;
        public ExtraMaterial Materials = ExtraMaterial.NONE;

        public bool North { get { return Surface.HasFlag(SurfaceF.NORTH); } }
        public bool South { get { return Surface.HasFlag(SurfaceF.SOUTH); } }
        public bool East { get { return Surface.HasFlag(SurfaceF.EAST); } }
        public bool West { get { return Surface.HasFlag(SurfaceF.WEST); } }
        public bool Floor { get { return Surface.HasFlag(SurfaceF.FLOOR); } }
        public bool Diagonal { get { return HasValidDiag(); } }
        public bool Curved { get { return Materials.HasFlag(ExtraMaterial.CURVE); } }

        public int MaterialID
        {
            get
            {
                return (byte)(Materials & (ExtraMaterial.MATERIALA | ExtraMaterial.MATERIALB)) >> 6;
            }
        }

        public bool HasValidDiag()
        {
            if (!Surface.HasFlag(SurfaceF.DIAG)) return false; //Not possible to have valid diagonal if not diagonal.
            //Catches invalid diagonals such as EW or NSE.
            if ((North && South) || (East && West)) return false;
            //Catches empty tiles / floor tiles.
            if (!((North || South) && (East || West))) return false;

            return true;
        }
        public bool IsEmpty()
        {
            return !(((byte)Surface & 31) > 0);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write((byte)Surface);
            writer.Write((byte)Materials);
        }
        public void ReadFromStream(BinaryReader reader)
        {
            Surface = (SurfaceF)reader.ReadByte();
            Materials = (ExtraMaterial)reader.ReadByte();
        }
    }
    public class MapComponent
    {
        public void WriteToStream(BinaryWriter writer)
        {
        }

        public static MapComponent ReadFromStream(BinaryReader reader)
        {
            return null;
        }
    }
}
