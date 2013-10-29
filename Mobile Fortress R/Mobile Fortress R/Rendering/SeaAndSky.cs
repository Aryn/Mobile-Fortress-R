using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Base;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common;

namespace Mobile_Fortress_R.Rendering
{
    class SeaPlane
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Effect effect;

        public Sector S;

        float SeaSize = 2500;
        float delta = 0f;

        static SeaPlane instance;

        public static float Level
        {
            get
            {
                return (float)Math.Cos(instance.delta);
            }
        }

        public SeaPlane(Sector s, float size)
            : base()
        {
            S = s;
            instance = this;
            SeaSize = size;

            effect = Cache.Get<Effect>("Shaders/Water");
            var device = effect.GraphicsDevice;
            
            effect.Setup();
            effect.SetTextureFile("Textures/Land/Sea");
            effect.SetNormalMap("Textures/Land/SeaNormal",3);
            effect.SetAmbient(Vector3.One * 0.5f);
            effect.SetSun(S.SunDirection, Vector3.One);

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(
                new VertexPositionNormalTexture[]
                {
                    new VertexPositionNormalTexture(new Vector3(-SeaSize,0,-SeaSize),Vector3.Up,new Vector2(0,0)),
                    new VertexPositionNormalTexture(new Vector3(SeaSize,0,-SeaSize),Vector3.Up,new Vector2(SeaSize/50,0)),
                    new VertexPositionNormalTexture(new Vector3(-SeaSize,0,SeaSize),Vector3.Up,new Vector2(0,SeaSize/50)),
                    new VertexPositionNormalTexture(new Vector3(SeaSize,0,SeaSize),Vector3.Up,new Vector2(SeaSize/50,SeaSize/50))
                }
                );
            /*
             * 0--1
             * |  |
             * 2--3
             */
            indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, 6, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(new int[] { 0, 1, 2, 2, 1, 3 });
        }
        public void Draw()
        {
            var device = effect.GraphicsDevice;
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);

            Matrix M = Matrix.CreateTranslation(new Vector3(Camera.Position.X, Level, Camera.Position.Z));
            effect.Update(M);
            effect.Parameters["TCoordOffset"].SetValue(delta / MathHelper.TwoPi);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }
        }
        public void Update(float dt)
        {
            if (delta < MathHelper.TwoPi) delta += 0.2f*dt;
            else delta = 0;
        }
    }

    class SkyBox
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Effect effect;

        float SkySize = 100;

        public static Color skyColor = Color.LightSkyBlue;

        public SkyBox()
            : base()
        {
            effect = Cache.Get<Effect>("Shaders/Sky");
            var device = effect.GraphicsDevice;
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), 8, BufferUsage.WriteOnly);
            VertexPositionColor[] vertices = new VertexPositionColor[8]
                {
                    new VertexPositionColor(new Vector3(-SkySize,-SkySize,-SkySize),skyColor),
                    new VertexPositionColor(new Vector3(SkySize,-SkySize,-SkySize),skyColor),
                    new VertexPositionColor(new Vector3(-SkySize,-SkySize,SkySize),skyColor),
                    new VertexPositionColor(new Vector3(SkySize,-SkySize,SkySize),skyColor),

                    new VertexPositionColor(new Vector3(-SkySize,SkySize,-SkySize),skyColor),
                    new VertexPositionColor(new Vector3(SkySize,SkySize,-SkySize),skyColor),
                    new VertexPositionColor(new Vector3(-SkySize,SkySize,SkySize),skyColor),
                    new VertexPositionColor(new Vector3(SkySize,SkySize,SkySize),skyColor)
                };

            vertexBuffer.SetData<VertexPositionColor>(vertices);
            /*
             * 0--1
             * |  |
             * 2--3
             */
            indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, 36, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(new int[] {
                0,1,2,2,1,3,
                4,6,5,7,5,6,

                1,5,3,3,5,7,
                0,2,4,6,4,2,

                0,4,1,1,4,5,
                2,3,6,7,6,3
            });
        }
        public void Draw(Vector3 skyColor, Vector3 fogColor)
        {
            var device = effect.GraphicsDevice;
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);

            effect.Update(Matrix.CreateTranslation(Camera.Position));
            //effect.Parameters["starTexture"].SetValue(stars);
            effect.Parameters["skyColor"].SetValue(skyColor);
            effect.Parameters["Darkness"].SetValue(MathHelper.Clamp(Camera.Position.Y / 5000f - 500f, 0, 1));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            }
        }
    }
}
