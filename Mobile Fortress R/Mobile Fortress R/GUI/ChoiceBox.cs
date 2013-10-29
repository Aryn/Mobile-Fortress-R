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
    class ChoiceBox : WindowControl
    {
        LabelControl label;
        public string Choice;
        public string Text
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
            }
        }
        public ChoiceBox(string msg, string[] choices)
        {
            SpriteFont font = Cache.Get<SpriteFont>("Skins/DefaultFont");
            Vector2 textSize = font.MeasureString(msg);
            this.Bounds = new UniRectangle(
                new UniScalar(0.5f, -textSize.X/2), new UniScalar(0.4f, -textSize.Y/2), new UniScalar(0.05f, textSize.X), new UniScalar(0.2f, textSize.Y));
            //Vector2 boxSize = this.GetSize();

            label = new LabelControl(msg);
            label.Bounds = new UniRectangle(
                new UniScalar(0.5f, -textSize.X/2), new UniScalar(0.5f, -textSize.Y/2),
                UniScalar.Zero, UniScalar.Zero
                );
            Children.Add(label);

            int choicecount = 0;
            foreach (string s in choices)
            {
                var choice = new ButtonControl();
                choice.Pressed += new EventHandler(Choice_Pressed);
                choice.Text = s;
                choice.SetBounds(0.0f, 4+choicecount*50, 1.0f, -32, 0f, 48, 0f, 24);
                Children.Add(choice);
                choicecount++;
            }
        }
        public void Display(Control container = null)
        {
            if (container == null) container = MobileFortress.GUI.Screen.Desktop;
            container.Children.Add(this);
            BringToFront();
        }
        void Choice_Pressed(object sender, EventArgs e)
        {
            Choice = ((ButtonControl)sender).Text;
            Close();
        }
        public static ChoiceBox Display(string text, string[] choices = null, Control container = null)
        {
            if (choices == null) choices = new string[] { "OK", "Cancel" };
            var box = new ChoiceBox(text, choices);
            box.Display(container);
            return box;
        }
    }
}
