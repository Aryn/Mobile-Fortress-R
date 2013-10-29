using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MobileFortress.Old;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysicsDrawer.Models;
using BEPUphysics.CollisionShapes;

namespace Mobile_Fortress_R.Rendering.Old
{
    class FortressRender : IRender
    {
        public static Model floorTile = null;
        public static Model wallTile = null;
        public static Texture2D floorTexture;
        public static Texture2D wallTexture;
        public static Texture2D armorTexture;
        public static Texture2D floorNormal;
        public static Texture2D wallNormal;
        public static Texture2D armorNormal;
        public static Effect effect = null;

        //public static Matrix wallTransform = Matrix.CreateScale(new Vector3(1, 1.6f, 1))
        //* Matrix.CreateRotationX(MathHelper.PiOver2);
        public static Matrix wallRotation90 = Matrix.CreateRotationZ(MathHelper.PiOver2);
        public static Matrix wallRotation45 = Matrix.CreateRotationZ(MathHelper.PiOver4);
        public static Matrix wallRotation270 = Matrix.CreateRotationZ(MathHelper.Pi + MathHelper.PiOver2);
        public static Matrix wallRotation315 = Matrix.CreateRotationZ(MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4);
        public static Matrix wallRotation180 = Matrix.CreateRotationX(MathHelper.Pi);

        //List<RenderTile> tiles = new List<RenderTile>();
        Fortress fort;
        InstancedModelDrawer drawer;

        public FortressRender(Fortress fort)
        {
            this.fort = fort;
            if(wallTile == null) LoadResources();
            //for (int L = 0; L < fort.plans.Layers; L++)
            //{
            //    for (int x = 0; x < fort.plans.Width; x++)
            //    {
            //        for (int y = 0; y < fort.plans.Height; y++)
            //        {
            //            var tilePosition = new Vector3(x * 10f, L * 16, y * 10) - fort.center;
            //            var type = fort.plans.GetTile(L, x, y);
            //            PlaceTileType(tiles, type, tilePosition);
            //        }
            //    }
            //}
            //List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            //List<ushort> indices = new List<ushort>();
            drawer = new InstancedModelDrawer(MobileFortress.Client);
            drawer.Add(fort.Entity);
            drawer.Update();
        }

        public void Draw(Common.Sector sector)
        {
            Matrix fortTransform = fort.World();
            //foreach (RenderTile tile in tiles)
            //{
            //    tile.Draw(fortTransform);
            //}
            foreach (FortressTile tile in fort.tiles)
            {
                DrawTile(tile, fortTransform);
            }
            //drawer.Update();
            //drawer.Draw(Camera.View, Camera.Projection);
        }
        public void Update()
        {
            
        }

        //void PlaceTileType(List<RenderTile> tiles, TileType type, Vector3 position)
        //{
        //    if (type.HasFlag(TileType.FLOOR))
        //    {
        //        tiles.Add(new RenderTile(position, TileType.FLOOR));
        //    }
        //    if (type.HasFlag(TileType.NORTH))
        //    {
        //        tiles.Add(new RenderTile(position + new Vector3(0f, 8f, -5f), TileType.NORTH));
        //    }
        //    if (type.HasFlag(TileType.SOUTH))
        //    {
        //        tiles.Add(new RenderTile(position + new Vector3(0f, 8f, 5f), TileType.SOUTH));
        //    }
        //    if (type.HasFlag(TileType.EAST))
        //    {
        //        tiles.Add(new RenderTile(position + new Vector3(5f, 8f, 0), TileType.EAST));
        //    }
        //    if (type.HasFlag(TileType.WEST))
        //    {
        //        tiles.Add(new RenderTile(position + new Vector3(-5f, 8f, 0), TileType.WEST));
        //    }
        //}

        void DrawTile(FortressTile tile, Matrix fortressTransform)
        {
            Model model = floorTile;
            Matrix local = model.Root.Transform * Matrix.CreateTranslation(tile.center - fort.center);
            Vector2 scale = Vector2.One;
            if (tile.type != TileType.FLOOR)
            {
                effect.SetTextureData(wallTexture, wallNormal, 2);
                effect.SetDualTextureData(armorTexture, armorNormal);
                model = wallTile;
                //if (type == TileRenderType.WallEW) local = wallTransform * local;
                //else local = wallRotation90 * wallTransform * local;
            }
            else
            {
                effect.SetTextureData(floorTexture, floorNormal, 4);
                effect.ClearDualTexture();
                scale = new Vector2(tile.height / 10f, tile.width / 10f);
                local = Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                    local;
            }
            switch (tile.type)
            {
                case TileType.NORTH:
                    scale = new Vector2(tile.width / 10, 1);
                    local = Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) * wallRotation270 * local;
                    break;
                case TileType.SOUTH:
                    scale = new Vector2(tile.width / 10, 1);
                    local = Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) * wallRotation90 * local;
                    break;
                case TileType.EAST:
                    scale = new Vector2(tile.width / 10, 1);
                    local = Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) * wallRotation180 * local;
                    break;
                case TileType.WEST:
                    scale = new Vector2(tile.width / 10, 1);
                    local = Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) * local;
                    break;
            }
            effect.Parameters["TexScale"].SetValue(scale);
            effect.Update(local * fortressTransform);
            effect.SetSun(Vector3.Down, Vector3.One);
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Draw();
            }
        }

        static void LoadResources()
        {
            floorTile = Cache.Get<Model>("Models/Fortress/Tile");
            wallTile = Cache.Get<Model>("Models/Fortress/Wall");
            floorTexture = Cache.Get<Texture2D>("Textures/FortressPieces/Tiles");
            wallTexture = Cache.Get<Texture2D>("Textures/FortressPieces/InternalWall");
            armorTexture = Cache.Get<Texture2D>("Textures/FortressPieces/ExternalWall");
            floorNormal = Cache.Get<Texture2D>("Textures/FortressPieces/NormalMaps/Tiles");
            wallNormal = Cache.Get<Texture2D>("Textures/FortressPieces/NormalMaps/InternalWall");
            armorNormal = Cache.Get<Texture2D>("Textures/FortressPieces/NormalMaps/ExternalWall");
            effect = Cache.Get<Effect>("Shaders/Normal");
            effect.Setup();
            effect.SetAmbient(Vector3.One * 0.3f);
            foreach (ModelMesh mesh in floorTile.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
            foreach (ModelMesh mesh in wallTile.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }
    }
}
