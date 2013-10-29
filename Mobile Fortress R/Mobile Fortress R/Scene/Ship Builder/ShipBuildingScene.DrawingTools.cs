using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MobileFortress;
using Microsoft.Xna.Framework;
using Mobile_Fortress_R.GUI;
using Microsoft.Xna.Framework.Graphics;

namespace Mobile_Fortress_R
{
    public partial class ShipBuildingScene
    {
        enum ToolState { Up, Down, Reversed }
        DrawingTool[] tools = new DrawingTool[] { new PencilTool(), new RoomTool() };
        DrawingTool currentTool;
        ToolState toolState = ToolState.Up;
        Texture2D highlight;
        void SwapTool(DrawingTool newTool)
        {
            currentTool.toolButton.Selected = false;
            newTool.toolButton.Selected = true;
            currentTool = newTool;
        }
        abstract class DrawingTool
        {
            public GraphicalButtonControl toolButton;

            //MouseDown: ToolDown -> ToolDrag
            //MouseMove: ToolDrag
            //MouseUp: ToolUp
            public virtual void ToolHighlight(ShipBuildingScene scene, int screenx, int screeny)
            {
                var batch = MobileFortress.Sprites;
                Point pixels = scene.plane.ScreenToPixels(new Point(screenx, screeny));
                batch.Draw(scene.highlight, new Rectangle(pixels.X, pixels.Y, 16, 16), Color.White);
            }
            public virtual void ToolDown(ShipBuildingScene scene, int x, int y) { }
            public virtual void ToolDrag(ShipBuildingScene scene, int x, int y) { }
            public virtual void ToolUp(ShipBuildingScene scene, int x, int y) { }

            public virtual void ToolRightDown(ShipBuildingScene scene, int x, int y) { }
            public virtual void ToolRightDrag(ShipBuildingScene scene, int x, int y) { }
            public virtual void ToolRightUp(ShipBuildingScene scene, int x, int y) { }
        }
        class PencilTool : DrawingTool
        {
            public override void ToolDrag(ShipBuildingScene scene, int x, int y)
            {
                MapTile placingTile = scene.placingTile;
                MapTile mapTile = scene.map.GetTile(x, y, scene.layer);
                mapTile.Surface = placingTile.Surface;
                mapTile.Materials = placingTile.Materials;
            }
            public override void ToolRightDrag(ShipBuildingScene scene, int x, int y)
            {
                MapTile mapTile = scene.map.GetTile(x, y, scene.layer);
                mapTile.Surface = SurfaceF.EMPTY;
                mapTile.Materials = ExtraMaterial.NONE;
            }
        }
        class RoomTool : DrawingTool
        {
            Point corner = Point.Zero;
            public override void ToolDown(ShipBuildingScene scene, int x, int y)
            {
                corner = new Point(x, y);
            }
            public override void ToolUp(ShipBuildingScene scene, int x, int y)
            {
                Point nextCorner = new Point(x, y);
                if (nextCorner.X < corner.X)
                {
                    int tmp = corner.X;
                    corner.X = nextCorner.X;
                    nextCorner.X = tmp;
                }
                if (nextCorner.Y < corner.Y)
                {
                    int tmp = corner.Y;
                    corner.Y = nextCorner.Y;
                    nextCorner.Y = tmp;
                }
                DrawRoom(scene, corner, nextCorner);
            }

            public override void ToolRightDown(ShipBuildingScene scene, int x, int y)
            {
                corner = new Point(x,y);
            }
            public override void ToolRightUp(ShipBuildingScene scene, int x, int y)
            {
                Point nextCorner = new Point(x, y);
                if (nextCorner.X < corner.X)
                {
                    int tmp = corner.X;
                    corner.X = nextCorner.X;
                    nextCorner.X = tmp;
                }
                if (nextCorner.Y < corner.Y)
                {
                    int tmp = corner.Y;
                    corner.Y = nextCorner.Y;
                    nextCorner.Y = tmp;
                }
                EraseRoom(scene, corner, nextCorner);
            }

            void DrawRoom(ShipBuildingScene scene, Point A, Point B)
            {
                bool hasFloor = scene.placingTile.Floor;
                for (int x = A.X; x <= B.X; x++)
                {
                    for (int y = A.Y; y <= B.Y; y++)
                    {
                        MapTile tile = scene.map.GetTile(x, y, scene.layer);
                        SurfaceF newSurface = SurfaceF.EMPTY;
                        if (hasFloor) newSurface |= SurfaceF.FLOOR;
                        if (y == A.Y) newSurface |= SurfaceF.NORTH;
                        if (y == B.Y) newSurface |= SurfaceF.SOUTH;
                        if (x == A.X) newSurface |= SurfaceF.WEST;
                        if (x == B.X) newSurface |= SurfaceF.EAST;
                        tile.Surface = newSurface;
                    }
                }
            }

            void EraseRoom(ShipBuildingScene scene, Point A, Point B)
            {
                SurfaceF newSurface = SurfaceF.EMPTY;
                for (int x = A.X; x <= B.X; x++)
                {
                    for (int y = A.Y; y <= B.Y; y++)
                    {
                        MapTile tile = scene.map.GetTile(x, y, scene.layer);
                        tile.Surface = newSurface;
                    }
                }
            }

            public override void ToolHighlight(ShipBuildingScene scene, int screenx, int screeny)
            {
                if (scene.toolState != ToolState.Up)
                {
                    var batch = MobileFortress.Sprites;
                    Point screenCorner = scene.plane.PlaneToScreenCoords(corner);
                    int minX = Math.Min(screenCorner.X, screenx);
                    int minY = Math.Min(screenCorner.Y, screeny);
                    int maxX = Math.Max(screenCorner.X, screenx);
                    int maxY = Math.Max(screenCorner.Y, screeny);
                    int scale = scene.plane.BlockWidth;
                    Point pixels = scene.plane.ScreenToPixels(new Point(minX, minY));
                    Point pixSize = scene.plane.ScreenToPixels(new Point((maxX - minX)+1, (maxY - minY)+1));
                    Rectangle dest = new Rectangle(pixels.X, pixels.Y, pixSize.X, pixSize.Y);
                    batch.Draw(scene.highlight, dest, Color.White);
                }
                else
                    base.ToolHighlight(scene, screenx, screeny);
            }
        }
    }
}
