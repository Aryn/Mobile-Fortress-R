using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;
using BEPUphysics.Entities;
using Lidgren.Network;
using BEPUphysics.Entities.Prefabs;

namespace Common.Base
{
    public class PhysicsPiece : GamePiece
    {
        public static Entity NullEntity = new Sphere(Vector3.Zero,1f);

        Entity entity = NullEntity;

        float CrossSection = 0f;
        const double OneThird = 1.0 / 3.0;

        public Entity Entity { get { return entity; } }

        public void AssignEntity(Entity E)
        {
            entity = E;
            CrossSection = E.Volume / (float)Math.Pow(E.Volume, OneThird);
        }

        protected override void WriteData(Lidgren.Network.NetOutgoingMessage msg)
        {
        }

        protected override void ReadData(Lidgren.Network.NetIncomingMessage msg)
        {
        }

        public override Microsoft.Xna.Framework.Vector3 Position()
        {
            return entity.Position;
        }

        public override Microsoft.Xna.Framework.Quaternion Rotation()
        {
            return entity.Orientation;
        }

        public override Microsoft.Xna.Framework.Vector3 Velocity()
        {
            return entity.LinearVelocity;
        }

        protected override void WriteUpdateData(Lidgren.Network.NetOutgoingMessage msg) { }

        public override void ReadUpdateData(NetIncomingMessage msg, float dt) { }

        public override void SetPRV(Vector3 pos, Quaternion rot, Vector3 vel)
        {
            entity.Position = pos;
            entity.Orientation = rot;
            entity.LinearVelocity = vel;
        }

        public void AddToSpace(ISpace space)
        {
            space.Add(entity);
        }

        public void ToggleGravity()
        {
            entity.IsAffectedByGravity = !entity.IsAffectedByGravity;
        }

        public const float Gravity = 19.6f;
        public static float GetBouyantForces(PhysicsPiece E, float dt)
        {
            return GetBuoyantForces(E.entity, dt);
        }
        public static float GetBuoyantForces(Entity E, float dt)
        {
            float impulse = E.Volume * Gravity * 14f * dt;
            return MathHelper.Clamp(impulse, 0, E.Mass * 2);
        }
        public static float GetDrag(PhysicsPiece E, float dt)
        {
            return GetDrag(E.entity, dt, E.CrossSection);
        }
        public static float GetDrag(Entity E, float dt, float crossSection)
        {
            float impulse = 0.0006f * E.LinearVelocity.LengthSquared();
            if (E.Position.Y < Sector.seaLevel) impulse = 0.001f * E.LinearVelocity.LengthSquared();
            return impulse;
        }

        public override void Update(float dt)
        {
        }
    }
}
