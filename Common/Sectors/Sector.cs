using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using BEPUphysics.Collidables;
using Microsoft.Xna.Framework;
using Common.Sectors;
using Common.Base;
using BEPUphysics.MathExtensions;
using Lidgren.Network;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities;
using Common.Net;

namespace Common
{
    public class Sector : ISpace
    {
        Point coords;
        Space space;
        Dictionary<ulong, GamePiece> pieces;
        Terrain land;
        Heightmap map;

        public Vector3 SunDirection = Vector3.Normalize(Vector3.Down + Vector3.Forward + Vector3.Left);
        public Vector3 SunColor = Vector3.One;
        public Vector3 SkyColor = Color.DeepSkyBlue.ToVector3();
        public Vector3 FogColor = Color.White.ToVector3();

        #region Constants
        public const float seaLevel = 0.0f;
        public const float lateralScale = 15;
        public const float verticalScale = 20;
        #endregion

        public Sector(Point coords)
        {
            this.coords = coords;
            space = new Space();
            space.ForceUpdater.Gravity = Vector3.Down * PhysicsPiece.Gravity;
            pieces = new Dictionary<ulong, GamePiece>();
            map = new Heightmap(GetSeed(), 9, 80f);

            float half = (map.Size * lateralScale) / 2;

            AffineTransform transform = new AffineTransform(
                new Vector3(lateralScale, verticalScale, -lateralScale),
                Quaternion.Identity,
                new Vector3(-half, -verticalScale / 2, half));

            land = new Terrain(new TerrainShape(map.Values, QuadTriangleOrganization.BottomLeftUpperRight), transform);
            land.Thickness = 10f;
            space.Add(land);
        }
        public Sector(int x, int y) : this(new Point(x, y)) { }

        #region Space
        public void Add(ISpaceObject spaceObject)
        {
            space.Add(spaceObject);
        }

        public BEPUphysics.DataStructures.ReadOnlyList<BEPUphysics.Entities.Entity> Entities
        {
            get { return space.Entities; }
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, float maximumLength, Func<BEPUphysics.BroadPhaseEntries.BroadPhaseEntry, bool> filter, IList<RayCastResult> outputRayCastResults)
        {
            return space.RayCast(ray, maximumLength, filter, outputRayCastResults);
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, float maximumLength, IList<RayCastResult> outputRayCastResults)
        {
            return space.RayCast(ray, maximumLength, outputRayCastResults);
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, float maximumLength, Func<BEPUphysics.BroadPhaseEntries.BroadPhaseEntry, bool> filter, out RayCastResult result)
        {
            return space.RayCast(ray, maximumLength, filter, out result);
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, float maximumLength, out RayCastResult result)
        {
            return space.RayCast(ray, maximumLength, out result);
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, Func<BEPUphysics.BroadPhaseEntries.BroadPhaseEntry, bool> filter, out RayCastResult result)
        {
            return space.RayCast(ray, filter, out result);
        }

        public bool RayCast(Microsoft.Xna.Framework.Ray ray, out RayCastResult result)
        {
            return space.RayCast(ray, out result);
        }

        public void Remove(ISpaceObject spaceObject)
        {
            space.Remove(spaceObject);
        }
        #endregion

        public void Update(float dt)
        {
            space.Update(dt);
            foreach (GamePiece P in pieces.Values)
            {
                P.Update(dt);
            }
        }

        public void Update()
        {
            space.Update();
        }

        #region Network
        internal void SendCreation(NetConnection connection)
        {
            var msg = Server.Net.Outgoing();
            msg.Write((byte)Net.DataType.Sector);
            msg.Write(coords);
            //msg.Write(f);
            //msg.Write(g);
            //msg.Write(h);
            msg.Write(pieces.Count);
            foreach (GamePiece P in pieces.Values)
            {
                P.WriteCreation(msg);
            }

            connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
        }
        #endregion

        public GamePiece GetPiece(ulong id)
        {
            GamePiece G = null;
            pieces.TryGetValue(id, out G);
            return G;
        }
        public int GetSeed()
        {
            return FNV1a.Hash32(coords.X ^ coords.Y * 100 ^ 2500);
        }
        public void AddPiece(GamePiece P)
        {
            pieces.Add(P.ID(), P);
        }

        //public float[,] Map { get { return map.Values; } }
        public Terrain Land { get { return land; } }
        public Space Space { get { return space; } }
    }
}
