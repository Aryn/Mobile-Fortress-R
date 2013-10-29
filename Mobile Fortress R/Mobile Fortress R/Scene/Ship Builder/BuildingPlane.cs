using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common.MobileFortress;
using Mobile_Fortress_R.Rendering;
using Microsoft.Xna.Framework.Input;

namespace Mobile_Fortress_R.GUI
{
    class BuildingPlane
    {
        public int BlockWidth { get { return BG.Width; } }
        public int BlockHeight { get { return BG.Height; } }
        Texture2D BG;

        Textures[] materialTextures;

        Point upperLeft;
        Point screenSize;

        public BuildingPlane(int w, int h)
        {
            upperLeft = new Point(-w / 2, -h / 2);
            screenSize = new Point(w, h);
            BG = Cache.Get<Texture2D>("Sprites/Builder/BG");
            materialTextures = new Textures[FortressRender.Materials.Length];
            for (int i = 0; i < FortressRender.Materials.Length; i++)
            {
                materialTextures[i] = new Textures(FortressRender.Materials[i]);
            }
        }

        public void Draw(MapLayer layer)
        {
            SpriteBatch batch = MobileFortress.Sprites;

            batch.Draw(BG,
                new Rectangle(0, 0, screenSize.X * BlockWidth, screenSize.Y * BlockHeight),
                new Rectangle(0,0, screenSize.X*BlockWidth, screenSize.Y*BlockHeight),
                Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);

            for (int x = upperLeft.X; x < upperLeft.X + screenSize.X; x++)
            {
                for (int y = upperLeft.Y; y < upperLeft.Y + screenSize.Y; y++)
                {
                    MapTile tile = layer.GetTile(x, y);
                    if (!tile.IsEmpty())
                    {
                        int screenX = (x - upperLeft.X) * BlockWidth + BlockWidth / 2;
                        int screenY = (y - upperLeft.Y) * BlockHeight + BlockHeight / 2;
                        DrawTile(tile, screenX, screenY, batch);
                    }
                }
            }
        }

        void DrawTile(MapTile tile, int x, int y, SpriteBatch batch)
        {
            Textures textures = materialTextures[tile.MaterialID];
            Rectangle dest = new Rectangle(x, y, BlockWidth, BlockHeight);
            Rectangle source = dest;
            source.Location = Point.Zero;
            Vector2 center = new Vector2(BlockWidth / 2, BlockHeight / 2);

            if (tile.Floor)
                batch.Draw(textures.floor,
                    dest, source,
                    Color.White,
                    0.0f, center, SpriteEffects.None, 0.5f);
            if (tile.North)
                batch.Draw(textures.wall,
                    dest, source,
                    Color.White,
                    0.0f, center, SpriteEffects.None, 0f);
            if (tile.South)
                batch.Draw(textures.wall,
                    dest, source,
                    Color.White,
                    MathHelper.Pi, center, SpriteEffects.None, 0);
            if (tile.East)
                batch.Draw(textures.wall,
                    dest, source,
                    Color.White,
                    MathHelper.PiOver2, center, SpriteEffects.None, 0);
            if (tile.West)
                batch.Draw(textures.wall,
                    dest, source,
                    Color.White,
                    MathHelper.Pi+MathHelper.PiOver2, center,
                    SpriteEffects.None, 0);
        }

        public bool MouseOverTile(out Point position)
        {
            if (MobileFortress.GUI.Screen.IsMouseOverGui)
            {
                position = Point.Zero;
                return false;
            }
            MouseState M = MobileFortress.Input.GetMouse().GetState();
            position = new Point(M.X / BlockWidth + upperLeft.X, M.Y / BlockHeight + upperLeft.Y);
            Rectangle bounds = new Rectangle(-FortressMap.MaxMapLateral, -FortressMap.MaxMapLateral, FortressMap.MaxMapLateral * 2, FortressMap.MaxMapLateral * 2);
            return bounds.Contains(position);
        }

        #region Conversion
        public Point PixelsToScreenCoords(Point P)
        {
            Point R = P;
            R.X /= BlockHeight;
            R.Y /= BlockWidth;
            return R;
        }
        public Point ScreenToPlaneCoords(Point S)
        {
            Point R = S;
            R.X += upperLeft.X;
            R.Y += upperLeft.Y;
            return R;
        }
        public Point PlaneToScreenCoords(Point P)
        {
            Point R = P;
            R.X -= upperLeft.X;
            R.Y -= upperLeft.Y;
            return R;
        }
        public Point ScreenToPixels(Point S)
        {
            Point R = S;
            R.X *= BlockHeight;
            R.Y *= BlockWidth;
            return R;
        }
        #endregion

        #region Textures
        class Textures
        {
            public Texture2D wall;
            public Texture2D floor;

            public Textures(FortressRender.Material material)
            {
                if (material != null)
                {
                    wall = Cache.Get<Texture2D>(material.contentpath + "/Builder/Wall");
                    floor = Cache.Get<Texture2D>(material.contentpath + "/Builder/Floor");
                }
            }
        }
        #endregion
    }
}
