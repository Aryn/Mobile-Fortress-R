using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile_Fortress_R.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.Collidables;
using Common;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Lidgren.Network;
using Common.MobileFortress;
using Nuclex.UserInterface;

namespace Mobile_Fortress_R
{
    public class SectorScene : Scene
    {
        Texture2D[] textures = null;
        Texture2D[] normalmaps = null;
        Common.Sector sec;
        Effect effect;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int width;

        SeaPlane sea;
        SkyBox sky;

        CharacterControllerInput character;

        Fortress fort;

        List<IRender> renderObjects = new List<IRender>();

        public SectorScene(Point coords)
        {
            sec = new Sector(coords);
            effect = Cache.Get<Effect>("Shaders/Terrain");
            effect.Parameters["FMax"].SetValue(0.6f);
            effect.Parameters["FMin"].SetValue(0.0f);
            textures = new Texture2D[]{
                    Cache.Get<Texture2D>("Textures/Land/Mud"),
                    Cache.Get<Texture2D>("Textures/Land/Sand"),
                    Cache.Get<Texture2D>("Textures/Land/Ground"),
                    Cache.Get<Texture2D>("Textures/Land/Stone")
                };
            normalmaps = new Texture2D[]{
                    Cache.Get<Texture2D>("Textures/Land/MudNormal"),
                    Cache.Get<Texture2D>("Textures/Land/SandNormal"),
                    Cache.Get<Texture2D>("Textures/Land/GroundNormal"),
                    Cache.Get<Texture2D>("Textures/Land/StoneNormal"),
                };
            effect.SetupTerrain(textures, normalmaps);
            effect.SetSun(sec.SunDirection, Vector3.One);

            width = sec.Land.Shape.Heights.GetLength(0);
            CopyToBuffers();

            sea = new SeaPlane(sec, 2500);//Sector.lateralScale * sec.Land.Shape.Heights.GetLength(0));
            sky = new SkyBox();

            character = new CharacterControllerInput(sec.Space);
            character.Activate();
            MobileFortress.SwitchControlTo(character);
        }

        public SectorScene(FortressMap map) : this(Point.Zero)
        {
            fort = new Fortress(map);
            FortressRender rendr = new FortressRender(fort);
            fort.SetPRV(Vector3.Up * 50, Quaternion.Identity, Vector3.Zero);
            fort.AddToSpace(sec.Space);
            sec.AddPiece(fort);
            Console.WriteLine("Total Fortress Mass: " + fort.Entity.Mass);
            Console.WriteLine("Structures: " + fort.Structures.Count);
            Console.WriteLine("Shapes: " + fort.ShapeEntries.Count);
            renderObjects.Add(rendr);
        }

        public override Screen LoadGUI()
        {
            return null;
        }

        public override void DisposeGUI()
        {
            
        }

        public void BuildSector(NetIncomingMessage msg)
        {
        }

        public override void Update(float dt)
        {
            Camera.Update(dt);
            character.Update(dt);
            if (MobileFortress.Controller.WasKeyPressed(Keys.F2))
            {
                RenderTarget2D screen = MobileFortress.Screen;
                int ssnum = 1;
                while (File.Exists("Screenshot" + ssnum + ".png"))
                    ssnum++;
                FileStream file = new FileStream("Screenshot"+ssnum+".png", System.IO.FileMode.CreateNew);
                screen.SaveAsPng(file, screen.Width, screen.Height);
                file.Close();
            }
            if(fort != null)
            {
                if (MobileFortress.Controller.IsKeyDown(Keys.I))
                {
                    fort.Entity.LinearVelocity += fort.Entity.WorldTransform.Forward * 2f * dt;
                }
                if (MobileFortress.Controller.IsKeyDown(Keys.K))
                {
                    fort.Entity.LinearVelocity += fort.Entity.WorldTransform.Backward * 2f * dt;
                }
                if (MobileFortress.Controller.IsKeyDown(Keys.J))
                {
                    fort.Entity.AngularVelocity += fort.Entity.WorldTransform.Up * 0.2f * dt;
                }
                if (MobileFortress.Controller.IsKeyDown(Keys.L))
                {
                    fort.Entity.AngularVelocity += fort.Entity.WorldTransform.Down * 0.2f * dt;
                }
            }
            sec.Update(dt);
            sea.Update(dt);
        }

        public override void Render()
        {
            var graphics = effect.GraphicsDevice;
            graphics.DepthStencilState = DepthStencilState.None;
            sky.Draw(sec.SkyColor, sec.FogColor);
            graphics.DepthStencilState = DepthStencilState.Default;
            graphics.Indices = indexBuffer;
            graphics.SetVertexBuffer(vertexBuffer);

            effect.Update(Matrix.Identity);
            effect.SetSun(sec.SunDirection, sec.SunColor);
            
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, width * width, 0, ((width - 1) * (width - 1) * 6) / 3);
            }

            foreach (IRender r in renderObjects)
            {
                r.Draw(sec);
            }

            graphics.DepthStencilState = DepthStencilState.DepthRead;
            graphics.RasterizerState = RasterizerState.CullNone;
            graphics.BlendState = BlendState.Additive;
            sea.Draw();
            graphics.DepthStencilState = DepthStencilState.Default;
            graphics.RasterizerState = RasterizerState.CullCounterClockwise;
            graphics.BlendState = BlendState.AlphaBlend;
        }

        void CopyToBuffers()
        {
            var vertices = Vertices(sec.Land);
            var indices = Indices();

            MFMath.CalculateTangentArray(ref vertices, indices);

            Console.WriteLine(vertices[0].Normal);

            if (vertexBuffer == null) vertexBuffer = new VertexBuffer(effect.GraphicsDevice, VertexMultitextured.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexMultitextured>(vertices);

            if (indexBuffer == null) indexBuffer = new IndexBuffer(effect.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);
        }
        VertexMultitextured[] Vertices(Terrain terrain)
        {
            float[,] values = terrain.Shape.Heights;
            var vertices = new VertexMultitextured[width * width];
            int x, y;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < width; y++)
                {
                    int i = x + y * width;
                    terrain.GetPosition(x, y, out vertices[i].Position);

                    /*float sx = values[x < width - 1 ? x + 1 : x, y] - values[x > 0 ? x - 1 : x, y];
                    if (x == 0 || x == width - 1)
                        sx *= 2;

                    float sy = values[x, y < width - 1 ? y + 1 : y] - values[x, y > 0 ? y - 1 : y];
                    if (y == 0 || y == width - 1)
                        sy *= 2;

                    vertices[i].Normal = new Vector3(-sx * Sector.verticalScale, 2 * Sector.lateralScale, sy * Sector.verticalScale);
                    vertices[i].Normal.Normalize();*/
                    
                    vertices[i].TextureCoordinate = new Vector2(x, y);
                    
                    if (values[x, y] < 0)
                        vertices[i].TexWeights = new Vector4(
                            1,
                            0,
                            0,
                            0
                            );
                    else
                        vertices[i].TexWeights = new Vector4(
                            MathHelper.Clamp(1.0f - Math.Abs(values[x, y] + 1.9f) / 2f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(values[x, y] - 0.5f) / 0.75f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(values[x, y] - 3) / 2.5f, 0, 1),
                            MathHelper.Clamp(1.0f - Math.Abs(values[x, y] - 7) / 2.0f, 0, 1)
                            );
                    vertices[i].TexWeights.Normalize();
                }
            }
            return vertices;
        }
        public int[] Indices()
        {
            int counter = 0;
            var indices = new int[(width - 1) * (width - 1) * 6];
            for (int y = 0; y < width - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
            return indices;
        }

        public override void Interpret(Lidgren.Network.NetIncomingMessage msg)
        {
            
        }

        public override void OnConnect()
        {
            throw new NotImplementedException();
        }

        public override void OnConnectionFail()
        {
            throw new NotImplementedException();
        }

        public override void OnDisconnect()
        {
            
        }
    }
    struct VertexMultitextured
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector4 TexWeights;
        public Vector3 Tangent;
        public Vector3 Binormal;

        public static int SizeInBytes = (3 + 3 + 2 + 4 + 3 + 3) * sizeof(float);
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(  0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
            new VertexElement(  sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            new VertexElement(  sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ),
            new VertexElement(  sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1 ),
            new VertexElement(  sizeof(float) * 12, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(  sizeof(float) * 15, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
        });

        public void AddTangent(Vector3 t)
        {
            Tangent += t;
            Tangent.Normalize();
        }
        public void AddBitangent(Vector3 b)
        {
            Binormal += b;
            Binormal.Normalize();
        }
        public void AddNormal(Vector3 n)
        {
            //Console.Write(n + " -> " + Normal);
            Normal += n;
            Normal.Normalize();
            //Console.WriteLine(" = " + Normal);
        }
    }
}
