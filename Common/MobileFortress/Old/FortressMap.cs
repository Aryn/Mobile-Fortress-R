using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using System.IO;
using BEPUphysics.Entities.Prefabs;

namespace Common.MobileFortress.Old
{
    public enum TileType {EMPTY = 0, NORTH = 1, SOUTH = 2, WEST = 4, EAST = 8, FLOOR = 16,
    DIAG = 32, COLOR1 = 64, COLOR2 = 128}
    //If the DIAG flag is set, any NW/SW/NE/SE wall combos become diagonals across the tile.
    //Defining NSE or NSW causes a special diag type to appear that does not cut the floor.
    //The top of the diagonal is slanted in the E/W flag's direction in this case.
    public class FortressMap
    {
        int totalWidth = 0;
        public int Width { get { return totalWidth; } }
        int totalHeight = 0;
        public int Height { get { return totalHeight; } }
        public int Layers { get { return layers.Count; } }

        bool ready = false;
        public bool Ready { get { return ready; } }

        List<FortressLayer> layers = new List<FortressLayer>(1);
        public List<FortressComponent> components = new List<FortressComponent>(1);

        class FortressLayer
        {
            public byte[,] tiles;
            public FortressLayer(FortressMap map)
            {
                tiles = new byte[map.totalWidth, map.totalHeight];
            }
            public void AddSurface(int x, int y, TileType type)
            {
                tiles[x, y] |= (byte)type;
            }
            public void RemoveSurface(int x, int y, TileType type)
            {
                tiles[x, y] &= (byte)~type;
            }
            public void AddRoom(Rectangle r, bool hasWalls = true)
            {
                for (int i = r.X; i <= r.Width; i++)
                {
                    for (int j = r.Y; j <= r.Height; j++)
                    {
                        byte newData = (byte)TileType.FLOOR;
                        if (hasWalls)
                        {
                            if (i == r.X) newData |= (byte)TileType.WEST;
                            else if (i == r.X + r.Width - 1) newData |= (byte)TileType.EAST;
                            if (j == r.Y) newData |= (byte)TileType.NORTH;
                            else if (j == r.Y + r.Height - 1) newData |= (byte)TileType.SOUTH;
                        }
                        tiles[i, j] |= newData;
                    }
                }
            }
        }

        public FortressMap(int w, int h)
        {
            totalWidth = w;
            totalHeight = h;
        }

        public TileType GetTile(int layer, int x, int y)
        {
            return (TileType)layers[layer].tiles[x, y];
        }
        public void SetTile(int layer, int x, int y, TileType type)
        {
            layers[layer].tiles[x, y] = (byte)type;
        }
        public static bool SameColor(TileType A, TileType B)
        {
            return A.HasFlag(TileType.COLOR1) == B.HasFlag(TileType.COLOR1) && A.HasFlag(TileType.COLOR2) == B.HasFlag(TileType.COLOR2);
        }

        public void Expand(int w, int h)
        {
            foreach (FortressLayer L in layers)
            {
                var bigarray = new byte[Width+w, Height+h];
                L.tiles.CopyTo(bigarray,0);
            }
        }
        public void ExpandLayer()
        {
            FortressLayer L = new FortressLayer(this);
            layers.Add(L);
        }

        public byte[,] GetTileLayer(int layer)
        {
            return layers[layer].tiles;
        }

        public void Save(string filename)
        {
            Point min = new Point(Width, Height);
            Point max = new Point(0, 0);
            for (int L = 0; L < Layers; L++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int ix = Width - (x + 1);
                        int iy = Height - (y + 1);
                        TileType minTile = (TileType)layers[L].tiles[x, y];
                        TileType maxTile = (TileType)layers[L].tiles[ix, iy];
                        if (minTile != TileType.EMPTY)
                        {
                            if (min.X > x) min.X = x;
                            if (min.Y > y) min.Y = y;
                        }
                        if (maxTile != TileType.EMPTY)
                        {
                            if (max.X < ix) max.X = ix;
                            if (max.Y < iy) max.Y = iy;
                        }
                    }
                }
            }

            BinaryWriter w = new BinaryWriter(File.Create(filename));
            w.Write((byte)Layers);
            w.Write((byte)(max.X - min.X + 1));
            w.Write((byte)(max.Y - min.Y + 1));
            w.Write((ushort)0);
            for (int L = 0; L < Layers; L++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    for (int x = min.X; x <= max.X; x++)
                    {
                        w.Write(layers[L].tiles[x,y]);
                    }
                }
            }
            //Save Components
            w.Close();
        }

        public static FortressMap GenerateTestMap()
        {
            FortressMap map = ReadMapFromText("TestMap.txt");
            map.ready = true;
            return map;
        }

        public static FortressMap ReadMapFromText(string file)
        {
            StreamReader r = File.OpenText(file);
            string[] parameters = r.ReadLine().Split('x');
            byte layers = byte.Parse(parameters[0]);
            byte w = byte.Parse(parameters[1]);
            byte h = byte.Parse(parameters[2]);

            var map = new FortressMap(w, h);
            for (int L = 0; L < layers; L++)
            {
                var layer = new FortressLayer(map);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        char c;
                        do
                            c = (char)r.Read();
                        while ((char.IsWhiteSpace(c)));
                        PlaceBlock(c, layer, x, y);
                    }
                }
                map.layers.Add(layer);
            }
            r.Close();
            return map;
        }
        public static FortressMap ReadMapFromMF(string file)
        {
            BinaryReader r = new BinaryReader(File.OpenRead(file));
            byte layers = r.ReadByte();
            byte w = r.ReadByte();
            byte h = r.ReadByte();
            ushort components = r.ReadUInt16();

            var map = new FortressMap(w, h);
            for (int L = 0; L < layers; L++)
            {
                var layer = new FortressLayer(map);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        layer.tiles[x, y] = r.ReadByte();
                    }
                }
                map.layers.Add(layer);
            }

            for (int c = 0; c < components; c++)
            {
                //Load Components
            }
            r.Close();
            return map;
        }
        public static FortressMap ReadMapFromMF(string file, int w, int h)
        {
            BinaryReader r = new BinaryReader(File.OpenRead(file));
            byte layers = r.ReadByte();
            byte fw = r.ReadByte();
            byte fh = r.ReadByte();
            ushort components = r.ReadUInt16();

            var map = new FortressMap(w, h);
            for (int L = 0; L < layers; L++)
            {
                var layer = new FortressLayer(map);
                for (int y = 0; y < fh; y++)
                {
                    for (int x = 0; x < fw; x++)
                    {
                        layer.tiles[x, y] = r.ReadByte();
                    }
                }
                map.layers.Add(layer);
            }

            for (int c = 0; c < components; c++)
            {
                //Load Components
            }
            r.Close();
            return map;
        }
        static void PlaceBlock(char c, FortressLayer layer, int x, int y)
        {
            switch (c)
            {
                case '_':
                    layer.AddSurface(x, y, TileType.FLOOR);
                    break;
                case '1':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.NORTH | TileType.WEST);
                    break;
                case '2':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.NORTH | TileType.EAST);
                    break;
                case '3':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.SOUTH | TileType.WEST);
                    break;
                case '4':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.SOUTH | TileType.EAST);
                    break;
                case 'A':
                    layer.AddSurface(x, y, TileType.NORTH | TileType.WEST);
                    break;
                case 'B':
                    layer.AddSurface(x, y, TileType.NORTH | TileType.EAST);
                    break;
                case 'C':
                    layer.AddSurface(x, y, TileType.SOUTH | TileType.WEST);
                    break;
                case 'D':
                    layer.AddSurface(x, y, TileType.SOUTH | TileType.EAST);
                    break;
                case 'N':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.NORTH);
                    break;
                case 'S':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.SOUTH);
                    break;
                case 'E':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.EAST);
                    break;
                case 'W':
                    layer.AddSurface(x, y, TileType.FLOOR | TileType.WEST);
                    break;
                case 'n':
                    layer.AddSurface(x, y, TileType.NORTH);
                    break;
                case 's':
                    layer.AddSurface(x, y, TileType.SOUTH);
                    break;
                case 'e':
                    layer.AddSurface(x, y, TileType.EAST);
                    break;
                case 'w':
                    layer.AddSurface(x, y, TileType.WEST);
                    break;
            }
        }

        public static FortressMap Expand(FortressMap map, int w, int h)
        {
            var newmap = new FortressMap(w, h);

            foreach (FortressLayer L in map.layers)
            {
                var newL = new FortressLayer(newmap);
                for (int x = 0; x < L.tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < L.tiles.GetLength(1); y++)
                    {
                        newL.tiles[x, y] = L.tiles[x, y];
                    }
                }
                newmap.layers.Add(newL);
            }
            return newmap;
        }
    }
}
