using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common;

namespace Mobile_Fortress_R.Rendering
{
    public static class EffectExtensions
    {
        public static void Setup(this Effect effect)
        {
            effect.CurrentTechnique = effect.Techniques["Light"];
            effect.SetTextureFile("Textures/Unknown");
        }

        public static void SetupTerrain(this Effect effect, Texture2D[] textures, Texture2D[] normalmaps)
        {
            effect.CurrentTechnique = effect.Techniques["Light"];
            for (int i = 0; i < textures.Length; i++)
            {
                effect.Parameters["Texture" + i].SetValue(textures[i]);
                effect.Parameters["NTexture" + i].SetValue(normalmaps[i]);
            }
        }

        public static void SetTextureFile(this Effect effect, string tex)
        {
            effect.Parameters["Texture"].SetValue(Cache.Get<Texture2D>(tex));
        }
        public static void SetNormalMap(this Effect effect, string map, float depth)
        {
            effect.Parameters["NormalMap"].SetValue(Cache.Get<Texture2D>(map));
            effect.Parameters["NormalConstant"].SetValue(depth);
        }

        public static void SetTextureData(this Effect effect, Texture2D tex, Texture2D normal, float depth)
        {
            effect.Parameters["Texture"].SetValue(tex);
            effect.Parameters["NormalMap"].SetValue(normal);
            effect.Parameters["NormalConstant"].SetValue(depth);
        }

        public static void SetDualTextureData(this Effect effect, Texture2D tex2, Texture2D normal2)
        {
            effect.Parameters["DualTexture"].SetValue(tex2);
            effect.Parameters["DualNormal"].SetValue(normal2);
            effect.Parameters["UseDualTexture"].SetValue(true);
        }

        public static void ClearDualTexture(this Effect effect)
        {
            effect.Parameters["UseDualTexture"].SetValue(false);
        }

        public static void SetAmbient(this Effect effect, Vector3 color)
        {
            effect.Parameters["AmbientColor"].SetValue(color);
        }
        public static void SetSun(this Effect effect, Vector3 direction, Vector3 color)
        {
            effect.Parameters["SunDirection"].SetValue(direction);
            effect.Parameters["SunColor"].SetValue(color);
        }

        public static void Update(this Effect effect, Matrix world)
        {
            if (effect is BasicEffect)
            {
                var basic = (BasicEffect)effect;
                basic.World = world;
                basic.View = Camera.View;
                basic.Projection = Camera.Projection;
            }
            else
            {
                effect.Parameters["World"].SetValue(world);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["ViewerPosition"].SetValue(Camera.Position);
            }
        }
        public static void Update(this Effect effect, Matrix world, Sector sec)
        {
            Update(effect, world);
            effect.Parameters["FogColor"].SetValue(sec.FogColor);
            SetSun(effect, sec.SunDirection, sec.SunColor);
        }
    }
}
