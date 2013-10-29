using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ship_Builder.UserInterface
{
    class Button : UIComponent
    {
        Texture2D tex;
        Action Pressed;

        public Button(Texture2D tex, Rectangle dim, Action pressed) : base(dim)
        {
            this.tex = tex;
            Pressed = pressed;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            batch.Draw(tex, Dimensions, Color.White);
        }

        public override void LeftClicked(int X, int Y)
        {
            throw new NotImplementedException();
        }
    }
}
