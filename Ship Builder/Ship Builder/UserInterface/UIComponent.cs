using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ship_Builder.UserInterface
{
    abstract class UIComponent
    {
        public static SpriteFont UIFont;

        protected Rectangle Dimensions;
        public abstract void Draw(SpriteBatch batch);
        public abstract void LeftClicked(int X, int Y);
        public virtual void Update(float dt) { }

        public UIComponent(Rectangle dimensions) { Dimensions = dimensions; }
        public bool Contains(int x, int y)
        {
            return Dimensions.Contains(x, y);
        }
    }
}
