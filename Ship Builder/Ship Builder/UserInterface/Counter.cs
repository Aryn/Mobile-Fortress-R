using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ship_Builder.UserInterface
{
    class Counter : UIComponent
    {
        const int defaultWidth = 96;
        const int defaultHeight = 24;
        const int buttonWidth = 24;

        static Texture2D upTex;
        static Texture2D downTex;
        public static void LoadTextures(GraphicsDevice device, ContentManager content)
        {
            upTex = content.Load<Texture2D>("UI/Buttons/Arrow");
            downTex = new Texture2D(device, upTex.Width, upTex.Height);
            Color[] data = new Color[downTex.Width * downTex.Height];
            Color[] invertedData = new Color[downTex.Width * downTex.Height];
            upTex.GetData<Color>(data);
            for (int y = 0; y < downTex.Height; y++)
            {
                for(int x = 0; x < downTex.Width; x++)
                {
                    invertedData[y * downTex.Width + x] = data[((downTex.Height-1) * downTex.Width) - (y * downTex.Width) + x];
                }
            }
            downTex.SetData<Color>(invertedData);
        }

        int displayX = 48;
        int x;
        int y;

        int value;
        int upperBoundary = 100;
        int textWidth = 8;

        Button decreaser;
        Button increaser;

        public Counter(int x, int y)
            : base(new Rectangle(x, y, defaultWidth, defaultHeight))
        {
            decreaser = new Button(upTex, new Rectangle(x,y,buttonWidth,defaultHeight), new Action(Decrease));
            increaser = new Button(downTex, new Rectangle(x + defaultWidth - buttonWidth, y, buttonWidth, defaultHeight), new Action(Increase));
            this.x = x;
            this.y = y;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            decreaser.Draw(batch);
            increaser.Draw(batch);
            batch.DrawString(UIComponent.UIFont, value.ToString(), new Vector2(x+displayX-textWidth, y), Color.White);
        }

        public override void LeftClicked(int X, int Y)
        {
            if (X <= x + 24) decreaser.LeftClicked(X, Y);
            else if (X >= x + defaultWidth - 24) increaser.LeftClicked(X, Y);
        }

        void Increase()
        {
            if(value < upperBoundary) value++;
            textWidth = (int)UIFont.MeasureString(value.ToString()).X / 2;
        }
        void Decrease()
        {
            if(value > 0) value--;
            textWidth = (int)UIFont.MeasureString(value.ToString()).X / 2;
        }

        public int GetValue()
        {
            return value;
        }
    }
}
