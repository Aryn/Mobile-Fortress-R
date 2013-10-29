using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface;

namespace Mobile_Fortress_R.GUI
{
    class AlertBox : WindowControl
    {
        public AlertBox(string msg)
        {
            SpriteFont font = Cache.Get<SpriteFont>("Skins/DefaultFont");
            Vector2 textSize = font.MeasureString(msg);
            this.Bounds = new UniRectangle(
                new UniScalar(0.5f, -textSize.X/2), new UniScalar(0.4f, -textSize.Y/2), new UniScalar(0.05f, textSize.X), new UniScalar(0.2f, textSize.Y));
            //Vector2 boxSize = this.GetSize();

            var label = new LabelControl(msg);
            label.Bounds = new UniRectangle(
                new UniScalar(0.5f, -textSize.X/2), new UniScalar(0.5f, -textSize.Y/2),
                UniScalar.Zero, UniScalar.Zero
                );
            Children.Add(label);

            var OK = new ButtonControl();
            OK.Pressed += new EventHandler(OK_Pressed);
            OK.Text = "OK";
            OK.SetRelativeBoundsCentered(0.5f, 0.8f, 0.5f, 0.2f);
            Children.Add(OK);
        }
        void OK_Pressed(object sender, EventArgs e)
        {
            Close();
        }
        public static AlertBox Display(string text, Control container = null)
        {
            if (container == null) container = MobileFortress.GUI.Screen.Desktop;
            var box = new AlertBox(text);
            container.Children.Add(box);
            box.BringToFront();
            return box;
        }
    }
}
