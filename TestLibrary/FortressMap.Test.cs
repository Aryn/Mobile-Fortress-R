using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Common.MobileFortress;
using System.IO;
using Common.Base;

namespace TestLibrary
{
    public class FortressMapTest
    {
        [Fact]
        public void NewFortressMap_CanCreateEmptyMap()
        {
            Assert.NotNull(FortressMap.Empty());
        }

        [Fact]
        public void IO_CanReadAndWrite()
        {
            FortressMap map = getTestMap();
            map.Save("FortressIOTest.mf");

            FortressMap mapFromFile = FortressMap.LoadFromPath("FortressIOTest.mf");

            for (int z = 0; z < map.LayerCount; z++)
            {
                MapLayer L = map.GetLayer(z);
                MapLayer LFF = mapFromFile.GetLayer(z);
                for (int x = -10; x < 10; x++)
                {
                    for (int y = -10; y < 10; y++)
                    {
                        Assert.Equal(L.GetTile(x, y).Surface, LFF.GetTile(x, y).Surface);
                    }
                }
            }
        }

        [Fact]
        public void FindDimensions_ReturnsNonNegativeSize()
        {
            var map = FortressMap.Empty();
            map.GetTile(-1, -1, 0).Surface = SurfaceF.EAST | SurfaceF.FLOOR;
            map.GetTile(1, 1, 0).Surface = SurfaceF.FLOOR;
            Point3D Min, Size;
            map.FindDimensions(out Min, out Size);
            Assert.InRange<int>(Size.X, 0, ushort.MaxValue);
            Assert.InRange<int>(Size.Y, 0, ushort.MaxValue);
            Assert.InRange<int>(Size.Z, 0, ushort.MaxValue);
        }
        [Fact]
        public void FindDimensions_ReturnsActualSize()
        {
            var map = FortressMap.Empty();
            map.GetTile(-1, -1, 0).Surface = SurfaceF.EAST | SurfaceF.FLOOR;
            map.GetTile(1, 1, 0).Surface = SurfaceF.FLOOR;
            Point3D Min, Size;
            map.FindDimensions(out Min, out Size);
            Assert.Equal(3, Size.X);
            Assert.Equal(3, Size.Y);
            Assert.Equal(1, Size.Z);
        }
        [Fact]
        public void FindDimensions_ReturnsActualMinimum()
        {
            var map = FortressMap.Empty();
            map.GetTile(-1, -1, 0).Surface = SurfaceF.EAST | SurfaceF.FLOOR;
            map.GetTile(1, 1, 0).Surface = SurfaceF.FLOOR;
            Point3D Min, Size;
            map.FindDimensions(out Min, out Size);
            Assert.Equal(-1, Min.X);
            Assert.Equal(-1, Min.Y);
            Assert.Equal(0, Min.Z);
        }

        FortressMap getTestMap()
        {
            var map = FortressMap.Empty();
            map.RemoveEmptyLayers();
            MapLayer L = new MapLayer();
            L.GetTile(-1, -1).Surface = SurfaceF.FLOOR;
            L.GetTile(1, 1).Surface = SurfaceF.NORTH | SurfaceF.SOUTH;
            map.AddLayer(L);
            L = new MapLayer();
            L.GetTile(1, -1).Surface = SurfaceF.EAST | SurfaceF.NORTH | SurfaceF.DIAG;
            L.GetTile(-1, 2).Surface = SurfaceF.SOUTH | SurfaceF.FLOOR | SurfaceF.COLORB;
            map.AddLayer(L);
            return map;
        }
    }

    public class MapLayerTest
    {
        [Fact]
        public void GetTile_ReturnsTileOnValidGet()
        {
            var testEmptyMap = FortressMap.Empty();
            Assert.NotNull(testEmptyMap.GetTile(0, 0, 0));
            Assert.NotNull(testEmptyMap.GetTile(-40, -40, 0));
            Assert.NotNull(testEmptyMap.GetTile(40, 40, 0));
        }
        [Fact]
        public void GetTile_ThrowsExceptionOnOutOfRange()
        {
            var testEmptyMap = FortressMap.Empty();
            Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { testEmptyMap.GetTile(-41, -41, 0); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { testEmptyMap.GetTile(41, 41, 0); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { testEmptyMap.GetTile(0, 0, 1); });
        }

        [Fact]
        public void IO_CanReadAndWrite()
        {
            Point3D min = new Point3D(-1, -1, 0);
            Point3D size = new Point3D(3, 3, 1);
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            MapLayer L = new MapLayer();
            L.GetTile(-1, -1).Surface = SurfaceF.FLOOR;
            L.GetTile(1, 1).Surface = SurfaceF.NORTH | SurfaceF.SOUTH;
            L.WriteToStream(writer, min, size);

            stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream);

            MapLayer LFF = new MapLayer();
            LFF.ReadFromStream(reader, min, size);

            reader.Close();
            writer.Close();

            //Checking within a small but definite range to ensure that no edges are cut off.
            for (int x = -10; x < 10; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    Assert.Equal(L.GetTile(x, y).Surface, LFF.GetTile(x, y).Surface);
                }
            }
        }
    }

    public class MapTileTest
    {
        [Fact]
        public void ValidDiag_ReturnsTrueOnValid()
        {
            MapTile T = new MapTile();
            T.Surface = SurfaceF.NORTH | SurfaceF.WEST | SurfaceF.DIAG;
            Assert.True(T.HasValidDiag());
            T.Surface = SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.DIAG;
            Assert.True(T.HasValidDiag());
        }
        [Fact]
        public void ValidDiag_ReturnsFalseOnInvalid()
        {
            MapTile T = new MapTile();
            T.Surface = SurfaceF.NORTH | SurfaceF.DIAG;
            Assert.False(T.HasValidDiag());
            T.Surface = SurfaceF.DIAG | SurfaceF.FLOOR;
            Assert.False(T.HasValidDiag());
            T.Surface = SurfaceF.NORTH | SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.WEST | SurfaceF.DIAG;
            Assert.False(T.HasValidDiag());
            T.Surface = SurfaceF.EMPTY;
            Assert.False(T.HasValidDiag());
        }

        [Fact]
        public void MaterialID_ReturnsMaterialID()
        {
            MapTile T = new MapTile();
            T.Materials = ExtraMaterial.MATERIALA | ExtraMaterial.EAST | ExtraMaterial.FLOOR;
            Assert.Equal(1, T.MaterialID);
        }

        [Fact]
        public void IsEmpty_ReturnsCorrectValues()
        {
            MapTile T = new MapTile();
            T.Surface = SurfaceF.FLOOR;
            Assert.False(T.IsEmpty());
            T.Surface = SurfaceF.NORTH | SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.WEST;
            Assert.False(T.IsEmpty());

            T.Surface = SurfaceF.EMPTY;
            Assert.True(T.IsEmpty());
            T.Surface = SurfaceF.DIAG;
            Assert.True(T.IsEmpty());
        }

        [Fact]
        public void IO_CanRead()
        {
            MemoryStream stream = new MemoryStream(new byte[] { 16, 255 });
            Assert.Equal(2, stream.Length);
            Assert.Equal(0, stream.Position);
            BinaryReader reader = new BinaryReader(stream);

            MapTile tileFromFile = new MapTile();
            tileFromFile.ReadFromStream(reader);

            reader.Close();

            Assert.Equal(16, (byte)tileFromFile.Surface);
            Assert.Equal(255, (byte)tileFromFile.Materials);
        }
        [Fact]
        public void IO_CanWrite()
        {
            MemoryStream stream = new MemoryStream(new byte[]{0,0});
            Assert.Equal(2, stream.Length);
            Assert.Equal(0, stream.Position);
            BinaryWriter writer = new BinaryWriter(stream);

            MapTile tile = new MapTile();
            tile.Materials = ExtraMaterial.MATERIALA | ExtraMaterial.MATERIALB;
            tile.Surface = SurfaceF.FLOOR;
            tile.WriteToStream(writer);

            stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream);

            MapTile tileFromFile = new MapTile();
            tileFromFile.ReadFromStream(reader);
            writer.Close();
            reader.Close();

            Assert.Equal(tile.Surface, tileFromFile.Surface);
            Assert.Equal(tile.Materials, tileFromFile.Materials);
        }
    }
}
