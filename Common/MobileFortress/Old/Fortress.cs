using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Base;
using Microsoft.Xna.Framework;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;

namespace Common.MobileFortress.Old
{
    public class Fortress : PhysicsPiece
    {
        public FortressMap plans;
        public Vector3 center;
        public List<CompoundShapeEntry> shapes;
        public List<FortressTile> tiles = new List<FortressTile>();

        public float Weight { get { return Entity.Mass; } }

        int maxPower = 0;

        public Fortress(FortressMap map)
        {
            plans = map;
            //List<CompoundShapeEntry> shapes = new List<CompoundShapeEntry>(map.Width*map.Height*map.Layers);
            Compose();
            //Load components first
            //for (int L = 0; L < map.Layers; L++)
            //{
            //    for (int x = 0; x < map.Width; x++)
            //    {
            //        for (int y = 0; y < map.Height; y++)
            //        {
            //            var tilePosition = new Vector3(x * 10, L * 16, y * 10);
            //            var type = map.GetTile(L, x, y);
            //            PlaceTileType(shapes, type, tilePosition);
            //        }
            //    }
            //}
            //var shape = new CompoundShape(shapes, out center);
            //AssignEntity(new BEPUphysics.Entities.Entity(shape, 100*shapes.Count));
            Entity.IsAffectedByGravity = false;
            Entity.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }

        public void Compose()
        {
            shapes = new List<CompoundShapeEntry>();
            for (int L = 0; L < plans.Layers; L++)
            {
                TileType[,] fillmap = new TileType[plans.Width, plans.Height];

                for (int x = 0; x < plans.Width; x++)
                {
                    for (int y = 0; y < plans.Height; y++)
                    {
                        if (!fillmap[x, y].HasFlag(TileType.NORTH) && plans.GetTile(L,x,y).HasFlag(TileType.NORTH))
                        {
                            shapes.Add(Follow(TileType.NORTH, ref fillmap, L, x, y));
                        }
                        if (!fillmap[x, y].HasFlag(TileType.SOUTH) && plans.GetTile(L, x, y).HasFlag(TileType.SOUTH))
                        {
                            shapes.Add(Follow(TileType.SOUTH, ref fillmap, L, x, y));
                        }
                        if (!fillmap[x, y].HasFlag(TileType.EAST) && plans.GetTile(L, x, y).HasFlag(TileType.EAST))
                        {
                            shapes.Add(Follow(TileType.EAST, ref fillmap, L, x, y));
                        }
                        if (!fillmap[x, y].HasFlag(TileType.WEST) && plans.GetTile(L, x, y).HasFlag(TileType.WEST))
                        {
                            shapes.Add(Follow(TileType.WEST, ref fillmap, L, x, y));
                        }
                        if (plans.GetTile(L, x, y).HasFlag(TileType.FLOOR))
                        {
                            fillmap[x, y] |= TileType.FLOOR;
                        }
                    }
                }
                MapQuadtree Q = new MapQuadtree(fillmap);
                Q.GetAllBoxes(ref shapes, ref tiles, L);
            }
            var shape = new CompoundShape(shapes, out center);
            AssignEntity(new Entity(shape, shapes.Count*100));
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        CompoundShapeEntry Follow(TileType type, ref TileType[,] fillmap, int L, int ox, int oy)
        {
            Vector3 center = new Vector3(ox * 10, L * 16, oy * 10);
            int width = 10;
            if (type == TileType.NORTH || type == TileType.SOUTH)
            {
                if (type == TileType.NORTH) center += new Vector3(0f, 8f, -5f);
                else center += new Vector3(0f, 8f, 5f);
                for (int x = ox+1; x < plans.Width; x++)
                {
                    var other = plans.GetTile(L, x, oy);
                    if (other.HasFlag(type))
                    {
                        fillmap[x, oy] |= type;
                        width += 10;
                        center += Vector3.Right * 5f;
                    }
                    else break;
                }
                tiles.Add(new FortressTile(type, center, width));
                return new CompoundShapeEntry(new BoxShape(width, 16f, 0.5f), center, 25);
            }
            else if (type == TileType.EAST || type == TileType.WEST)
            {
                if (type == TileType.WEST) center += new Vector3(-5f, 8f, 0f);
                else center += new Vector3(5f, 8f, 0f);
                for (int y = oy+1; y < plans.Height; y++)
                {
                    var other = plans.GetTile(L, ox, y);
                    if (other.HasFlag(type))
                    {
                        fillmap[ox, y] |= type;
                        width += 10;
                        center += Vector3.Backward * 5f;
                    }
                    else break;
                }
                tiles.Add(new FortressTile(type, center, width));
                return new CompoundShapeEntry(new BoxShape(0.5f, 16f, width), center, 25);
            }
            throw new InvalidOperationException("Unsupported flag.");
        }

        void PlaceTileType(List<CompoundShapeEntry> shapes, TileType type, Vector3 position)
        {
            if (type.HasFlag(TileType.FLOOR))
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(10.1f, 0.5f, 10.1f),
                    position,
                    25
                    ));
            }
            if (type.HasFlag(TileType.NORTH))
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(10f, 16f, 0.5f),
                    position + new Vector3(0f, 8f, -5f),
                    25
                    ));
            }
            if (type.HasFlag(TileType.SOUTH))
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(10f, 16f, 0.5f),
                    position + new Vector3(0f, 8f, 5f),
                    25
                    ));
            }
            if (type.HasFlag(TileType.EAST))
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(0.5f, 16f, 10f),
                    position + new Vector3(5f, 8f, 0f),
                    25
                    ));
            }
            if (type.HasFlag(TileType.WEST))
            {
                shapes.Add(new CompoundShapeEntry(
                    new BoxShape(0.5f, 16f, 10f),
                    position + new Vector3(-5f, 8f, 0f),
                    25
                    ));
            }
        }
    }

    public class FortressTile
    {
        public TileType type;
        public int width;
        public int height;
        public Vector3 center;
        public FortressTile(TileType type, Vector3 center, int width, int height)
        {
            this.type = type;
            this.width = width;
            this.height = height;
            this.center = center;
        }
        public FortressTile(TileType type, Vector3 center, int width) : this(type, center, width, width) { }
    }
}
