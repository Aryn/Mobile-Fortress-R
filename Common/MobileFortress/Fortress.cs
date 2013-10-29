using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.CollisionShapes;
using Common.Base;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;

namespace Common.MobileFortress
{
    public class Fortress : PhysicsPiece
    {
        public static string[] Materials = new string[] { "Material/Metal", "Material/Undefined", "Material/Undefined", "Material/Undefined" };

        public FortressMap Map { get; private set; }
        public Vector3 Center { get; private set; }
        public List<CompoundShapeEntry> ShapeEntries;
        public List<Structure> Structures;

        public float Weight { get { return Entity.Mass; } }

        public Fortress(FortressMap map)
        {
            Map = map;
            Compose();
        }

        public void Compose()
        {
            ShapeEntries = new List<CompoundShapeEntry>();
            Structures = new List<Structure>();
            for (int z = 0; z < Map.LayerCount; z++)
            {
                for (int x = -FortressMap.MaxMapLateral; x <= FortressMap.MaxMapLateral; x++)
                {
                    for (int y = -FortressMap.MaxMapLateral; y <= FortressMap.MaxMapLateral; y++)
                    {
                        var mapTile = Map.GetTile(x, y, z);
                        Structure.StructuresFromMapTile(mapTile, new Point3D(x,y,z), this);
                    }
                }
            }
            Vector3 center;
            var shape = new CompoundShape(ShapeEntries, out center);
            Center = center;
            AssignEntity(new Entity(shape, 100f*ShapeEntries.Count));
            Entity.IsAffectedByGravity = false;
            Entity.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }

        public enum StructureType { FLOOR = 0, NORTH, SOUTH, EAST, WEST }
        public class Structure
        {
            static Vector3 Scale = new Vector3(10, 16, 10);
            public Vector3 center;
            public Point3D mapCenter;
            EntityShape shape;
            public StructureType type;
            public Quaternion rotation = Quaternion.Identity;
            public byte materialID;
            private Structure(Vector3 center, StructureType type, int materialID)
            {
                if (type == StructureType.FLOOR)
                    shape = new BoxShape(10, 0.5f, 10);
                else
                {
                    shape = new BoxShape(0.5f, 16f, 10f);
                    switch (type)
                    {
                        case StructureType.EAST:
                            center += new Vector3(5, 8, 0);
                            rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi);
                            break;
                        case StructureType.WEST:
                            center += new Vector3(-5, 8, 0);
                            break;
                        case StructureType.NORTH:
                            center += new Vector3(0, 8, 5);
                            rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
                            break;
                        case StructureType.SOUTH:
                            center += new Vector3(0, 8, -5);
                            rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi + MathHelper.PiOver2);
                            break;
                    }
                    //rotation = Quaternion.Identity;
                }
                this.center = center;
                this.type = type;
                this.materialID = (byte)materialID;
            }

            public CompoundShapeEntry GetCompoundEntry()
            {
                return new CompoundShapeEntry(shape, new RigidTransform(center, rotation));
            }

            public static void StructuresFromMapTile(MapTile tile, Point3D position, Fortress fortress)
            {
                if (!tile.Diagonal && !tile.Curved)
                {
                    //There are reasons for this seemingly redundant code:
                    //Each if statement generates a new structure itself, such that N|W|FLOOR gets 3 structures.
                    Vector3 center = position.ToVector() * Scale;
                    if (tile.North)
                    {
                        Structure structure = new Structure(center, StructureType.NORTH, tile.MaterialID);
                        structure.mapCenter = position;
                        fortress.Structures.Add(structure);
                        fortress.ShapeEntries.Add(structure.GetCompoundEntry());
                    }
                    if (tile.South)
                    {
                        Structure structure = new Structure(center, StructureType.SOUTH, tile.MaterialID);
                        structure.mapCenter = position;
                        fortress.Structures.Add(structure);
                        fortress.ShapeEntries.Add(structure.GetCompoundEntry());
                    }
                    if (tile.East)
                    {
                        Structure structure = new Structure(center, StructureType.EAST, tile.MaterialID);
                        structure.mapCenter = position;
                        fortress.Structures.Add(structure);
                        fortress.ShapeEntries.Add(structure.GetCompoundEntry());
                    }
                    if (tile.West)
                    {
                        Structure structure = new Structure(center, StructureType.WEST, tile.MaterialID);
                        structure.mapCenter = position;
                        fortress.Structures.Add(structure);
                        fortress.ShapeEntries.Add(structure.GetCompoundEntry());
                    }
                    if (tile.Floor)
                    {
                        Structure structure = new Structure(center, StructureType.FLOOR, tile.MaterialID);
                        structure.mapCenter = position;
                        fortress.Structures.Add(structure);
                        fortress.ShapeEntries.Add(structure.GetCompoundEntry());
                    }
                    
                }
                else
                {
                    throw new NotImplementedException("Diagonal and curved surfaces not implemented yet.");
                }
            }
        }
    }
}
