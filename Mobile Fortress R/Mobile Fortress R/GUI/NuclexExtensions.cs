using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework;

namespace Mobile_Fortress_R.GUI
{
    static class NuclexExtensions
    {
        public static void SetBounds(this Control c, float xr, int xa, float yr, int ya, float wr, int wa, float hr, int ha)
        {
            c.Bounds = new UniRectangle(new UniScalar(xr, xa), new UniScalar(yr, ya), new UniScalar(wr, wa), new UniScalar(hr, ha));
        }
        public static void SetRelativeBoundsCentered(this Control c, float x, float y, float w, float h)
        {
            c.Bounds = new UniRectangle(new UniScalar(x-w/2, 0), new UniScalar(y-h/2, 0), new UniScalar(w, 0), new UniScalar(h, 0));
        }
        public static void SetStaticSizeBoundsCentered(this Control c, float x, float y, int w, int h)
        {
            c.Bounds = new UniRectangle(new UniScalar(x, -w/2), new UniScalar(y, -h/2), new UniScalar(0, w), new UniScalar(0, h));
        }
        public static void SetStaticSizeBounds(this Control c, float x, float y, int w, int h)
        {
            c.Bounds = new UniRectangle(new UniScalar(x, 0), new UniScalar(y, 0), new UniScalar(0, w), new UniScalar(0, h));
        }
        public static Vector2 GetSize(this Control c)
        {
            if (c.Parent != null)
                return c.Bounds.Size.ToOffset(c.Parent.GetSize());
            else
                return c.Bounds.Size.ToOffset(new Vector2(MobileFortress.Viewport.Width, MobileFortress.Viewport.Height));
        }
    }
}
