using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace Common.MobileFortress.Old
{
    class MapQuadtree
    {
        MapQuadtree[] subtrees = null;
        Rectangle area;
        bool hasFloor;
        public MapQuadtree(TileType[,] fillmap) : this(fillmap, new Rectangle(0, 0, fillmap.GetLength(0), fillmap.GetLength(1))) { }
        public MapQuadtree(TileType[,] fillmap, Rectangle area)
        {
            this.area = area;
            hasFloor = fillmap[area.X, area.Y].HasFlag(TileType.FLOOR);
            for (int x = area.X; x < area.X + area.Width; x++)
            {
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    if (fillmap[x, y].HasFlag(TileType.FLOOR) != hasFloor)
                    {
                        Split(fillmap);
                        return;
                    }
                }
            }
        }
        public void Split(TileType[,] fillmap)
        {
            int splitX = area.Width/2;
            int splitY = area.Height/2;
            subtrees = new MapQuadtree[4];
            subtrees[0] = new MapQuadtree(fillmap, new Rectangle(area.X, area.Y, splitX, splitY));
            subtrees[1] = new MapQuadtree(fillmap, new Rectangle(area.X + splitX, area.Y, area.Width - splitX, splitY));
            subtrees[2] = new MapQuadtree(fillmap, new Rectangle(area.X, area.Y + splitY, splitX, area.Height - splitY));
            subtrees[3] = new MapQuadtree(fillmap, new Rectangle(area.X+splitX, area.Y+splitY, area.Width-splitX, area.Height-splitY));
        }

        public void GetAllBoxes(ref List<CompoundShapeEntry> list, ref List<FortressTile> tiles, int layer)
        {
            if (subtrees != null)
            {
                subtrees[0].GetAllBoxes(ref list,ref tiles,layer);
                subtrees[1].GetAllBoxes(ref list,ref tiles,layer);
                subtrees[2].GetAllBoxes(ref list,ref tiles,layer);
                subtrees[3].GetAllBoxes(ref list,ref tiles,layer);
            }
            else if (hasFloor)
            {
                var center = new Vector3(area.X*10 + area.Width*5-5, layer*16, area.Y*10 + area.Height*5-5);
                tiles.Add(new FortressTile(TileType.FLOOR, center, area.Width*10, area.Height*10));
                list.Add(new CompoundShapeEntry(new BoxShape(area.Width*10, 0.5f, area.Height*10),
                    center, 25));
            }
        }
    }
}
