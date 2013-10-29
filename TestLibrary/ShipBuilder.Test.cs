using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Mobile_Fortress_R;
using System.Reflection;
using Common.MobileFortress;

namespace TestLibrary
{
    public class ShipBuilderTest
    {
        ShipBuildingScene scene;
        object[] toolList; //Technically DrawingTool[] but it's not a public class so I need to use reflection anyway.

        public ShipBuilderTest()
        {
            scene = new ShipBuildingScene();
            MapTile tile = new MapTile();
            tile.Surface = SurfaceF.NORTH | SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.WEST | SurfaceF.FLOOR;
            object octagon = Test.GetInstanceVariable<object>(scene, "octagon");
            FieldInfo tileInfo = octagon.GetType().GetField("tile", BindingFlags.Instance | BindingFlags.Public);
            tileInfo.SetValue(octagon, tile);
            toolList = Test.GetInstanceVariable<object[]>(scene, "tools");
        }

        [Fact]
        public void DrawingTools_PencilTool_DrawsOnce()
        {
            ToolDown(0, 0, 0);
            ToolDrag(0, 0, 0);
            ToolUp(0, 0, 0);
            Assert.Equal(PlacingTile().Surface, Map().GetTile(0,0,0).Surface);
        }
        [Fact]
        public void DrawingTools_RoomTool_DrawsRoom()
        {
            ToolDown(1, -1, -1);
            ToolDrag(1, -1, -1);
            ToolDrag(1, 0, 0);
            ToolDrag(1, 1, 1);
            ToolUp(1, 1, 1);
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Assert.True(Map().GetTile(x, y, 0).Floor, "Tile "+x+", "+y+" has no floor.");
                }
            }
            Assert.Equal(SurfaceF.NORTH | SurfaceF.WEST | SurfaceF.FLOOR, Map().GetTile(-1, -1, 0).Surface);
            Assert.Equal(SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.FLOOR, Map().GetTile(1, 1, 0).Surface);
        }

        void ToolDown(int i, int x, int y)
        {
            Test.CallInstanceMethod(toolList[i], new object[] { scene, x, y }, "ToolDown");
        }
        void ToolDrag(int i, int x, int y)
        {
            Test.CallInstanceMethod(toolList[i], new object[] { scene, x, y }, "ToolDrag");
        }
        void ToolUp(int i, int x, int y)
        {
            Test.CallInstanceMethod(toolList[i], new object[] { scene, x, y }, "ToolUp");
        }
        FortressMap Map()
        {
            return Test.GetInstanceVariable<FortressMap>(scene, "map");
        }
        MapTile PlacingTile()
        {
            return Test.GetInstanceVariable<MapTile>(scene, "octagon", "tile");
        }
    }
}
