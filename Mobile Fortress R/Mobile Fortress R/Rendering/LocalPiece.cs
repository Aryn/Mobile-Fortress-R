using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mobile_Fortress_R.Rendering
{
    class LocalPiece : GamePiece, IRender
    {
        Vector3 position;
        Quaternion rotation;

        public Model model;
        //public MFEffect effect;
        public Effect effect;
        public Vector3 baseColor = Vector3.One;

        public Vector3[] pointLights = new Vector3[3];
        public Vector3[] pointLightColors = new Vector3[3];
        public float[] pointLightRadius = new float[3];
        int lightCount = 0;

        public LocalPiece(Vector3 pos, Quaternion rot, Model M)
            : base()
        {
            position = pos;
            rotation = rot;
            model = M;
        }

        public void ApplyEffect()
        {
            //effect.Parameters["lightDirection"].SetValue(Vector3.Transform(Vector3.Forward * 0.3f + Vector3.Up * 0.8f, World()));
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        public void Draw(Common.Sector sector)
        {
            var baseWorld = World();
            foreach (ModelMesh mesh in model.Meshes)
            {
                var world = mesh.ParentBone.Transform * baseWorld;
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect.Update(world);
                }
                mesh.Draw();
            }
        }

        public void ApplyPointLight(Vector3 position, Vector3 color, float radius)
        {
            if (lightCount >= 4)
                throw new ApplicationException("Only four lights can be applied to an object.");
            pointLights[lightCount] = position;
            pointLightColors[lightCount] = color;
            pointLightRadius[lightCount] = radius;
            lightCount++;
            effect.Parameters["LightPosition"].SetValue(pointLights);
            effect.Parameters["LightColor"].SetValue(pointLightColors);
            effect.Parameters["LightRadius"].SetValue(pointLightRadius);
        }
        public void ApplySun(Vector3 direction, Vector3 color)
        {
            effect.Parameters["SunDirection"].SetValue(direction);
            effect.Parameters["SunColor"].SetValue(color);
        }

        public void Rotate(float angle)
        {
            rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle)*rotation;
        }

        public override Microsoft.Xna.Framework.Vector3 Position()
        {
            return position;
        }

        public override Microsoft.Xna.Framework.Quaternion Rotation()
        {
            return rotation;
        }

        public override Microsoft.Xna.Framework.Vector3 Velocity()
        {
            return Vector3.Zero;
        }

        public override void Update(float dt)
        {
            
        }

        protected override void WriteData(Lidgren.Network.NetOutgoingMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override void ReadData(Lidgren.Network.NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override void WriteUpdateData(Lidgren.Network.NetOutgoingMessage msg)
        {
            throw new NotImplementedException();
        }

        public override void ReadUpdateData(Lidgren.Network.NetIncomingMessage msg, float dt)
        {
            throw new NotImplementedException();
        }

        public override void SetPRV(Vector3 pos, Quaternion rot, Vector3 vel)
        {
            throw new NotImplementedException();
        }
    }
}
