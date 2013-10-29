using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MobileFortress;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mobile_Fortress_R.Rendering
{
    class FortressRender : IRender
    {
        public static Material[] Materials = new Material[] {
            new Material("Material/Glass", Material.RenderMode.Alpha),
            new Material("Material/Metal"),
            null,
            null };
        Fortress fortress;

        BEPUphysicsDrawer.Models.ModelDrawer physics;

        public FortressRender(Fortress fortress)
        {
            this.fortress = fortress;
            //physics = new BEPUphysicsDrawer.Models.InstancedModelDrawer(MobileFortress.Client);
            //physics.Add(fortress.Entity);
        }

        public void Draw(Common.Sector sector)
        {
            //physics.Update();
            //physics.Draw(Camera.View, Camera.Projection);
            Matrix fortressTransform = fortress.Entity.WorldTransform;
            foreach (Fortress.Structure structure in fortress.Structures)
            {
                DrawStructure(structure, fortressTransform, sector);
            }
        }

        public void DrawStructure(Fortress.Structure structure, Matrix fortressTransform, Common.Sector sector)
        {
            Material mat = Materials[structure.materialID];
            Effect effect = mat.effect;
            Model model;
            Matrix scale = Matrix.Identity;
            if (structure.type == Fortress.StructureType.FLOOR)
            {
                mat.ApplyFloor(effect, out model);
                scale = Matrix.CreateScale(2,1,2);
                Vector2 textureOffset = new Vector2(0.5f * (structure.mapCenter.X % 2), 0.5f * (structure.mapCenter.Y % 2));
                effect.Parameters["TexOffset"].SetValue(textureOffset);
            }
            else
            {
                mat.ApplyWall(effect, out model);
                scale = Matrix.CreateScale(0.5f, 1, 1);
                Vector2 textureOffset = new Vector2(0.5f * (structure.mapCenter.X % 2 ^ structure.mapCenter.Y % 2), 0f);
                effect.Parameters["TexOffset"].SetValue(textureOffset);
            }
            Matrix local = model.Root.Transform * scale * Matrix.CreateFromQuaternion(structure.rotation) * Matrix.CreateTranslation(structure.center - fortress.Center);
            //effect.Parameters["TexScale"].SetValue(scale);
            effect.Update(local * fortressTransform);
            effect.SetSun(sector.SunDirection, sector.SunColor);
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Draw();
            }
        }

        public class Material
        {
            public enum RenderMode { Default, Alpha }
            public string contentpath;

            public Effect effect;

            public RenderMode renderMode;

            Model wallModel;
            Model floorModel;
            Model diagonalFloorModel;

            Texture2D floorTexture;
            Texture2D floorNormal;

            Texture2D wallInteriorTexture;
            Texture2D wallInteriorNormal;
            Texture2D wallExteriorTexture;
            Texture2D wallExteriorNormal;

            public Material(string path, RenderMode mode = RenderMode.Default)
            {
                contentpath = path;
                wallModel = Cache.Get<Model>(path + "/Models/Wall");
                floorModel = Cache.Get<Model>(path + "/Models/Floor");
                diagonalFloorModel = Cache.Get<Model>(path + "/Models/DiagonalFloor");

                floorTexture = Cache.Get<Texture2D>(path + "/Textures/Floor");
                floorNormal = Cache.Get<Texture2D>(path + "/Normals/Floor");
                wallExteriorTexture = Cache.Get<Texture2D>(path + "/Textures/ExternalWall");
                wallExteriorNormal = Cache.Get<Texture2D>(path + "/Normals/ExternalWall");
                wallInteriorTexture = Cache.Get<Texture2D>(path + "/Textures/InternalWall");
                wallInteriorNormal = Cache.Get<Texture2D>(path + "/Normals/InternalWall");

                effect = Cache.Get<Effect>("Shaders/Normal");
                effect.Setup();
                foreach(Model model in new Model[]{wallModel, floorModel, diagonalFloorModel})
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = effect;
                        }
                    }
                }
                renderMode = mode;
            }

            public void ApplyFloor(Effect effect, out Model model)
            {
                effect.SetTextureData(floorTexture, floorNormal, 2);
                effect.ClearDualTexture();
                model = floorModel;
            }
            public void ApplyWall(Effect effect, out Model model)
            {
                effect.SetTextureData(wallInteriorTexture, wallInteriorNormal, 2);
                effect.SetDualTextureData(wallExteriorTexture, wallExteriorNormal);
                model = wallModel;
            }
        }

    }
}
