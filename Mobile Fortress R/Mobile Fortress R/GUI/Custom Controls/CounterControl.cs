using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Input;

namespace Mobile_Fortress_R.GUI
{
    class Counter : Control
    {
        int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                if (Changed != null)
                    Changed.Invoke(this, EventArgs.Empty);
                display.Text = count.ToString();
            }
        }

        MenuButtonControl upButton;
        MenuButtonControl downButton;
        InputControl display;
        LabelControl label;

        int min = 0;
        int max = 99;

        public event EventHandler Changed;
        
        public Counter(int initial, int min, int max, int x, int y)
        {
            Bounds = new UniRectangle(new UniScalar(0.0f, x), new UniScalar(0.0f, y), new UniScalar(0.0f, 48), new UniScalar(0.0f, 13));

            this.min = min;
            this.max = max;

            label = new LabelControl();
            label.Bounds = new UniRectangle(new UniScalar(0.0f, -24), new UniScalar(0.0f, -16), new UniScalar(0.0f, 48), new UniScalar(0.0f, 16));

            display = new InputControl();
            display.Bounds = new UniRectangle(new UniScalar(0.5f, -12), new UniScalar(0.0f, -5), new UniScalar(0.0f, 24), new UniScalar(0.0f, 22));
            Children.Add(display);
            Count = initial;

            upButton = new MenuButtonControl("minimize");
            upButton.Bounds = new UniRectangle(new UniScalar(1.0f, -13), new UniScalar(0.0f, 0), new UniScalar(0.0f, 13), new UniScalar(0.0f, 13));
            Children.Add(upButton);

            downButton = new MenuButtonControl("close");
            downButton.Bounds = new UniRectangle(new UniScalar(0.0f, 0), new UniScalar(0.0f, 0), new UniScalar(0.0f, 13), new UniScalar(0.0f, 13));
            Children.Add(downButton);

            upButton.Pressed += new EventHandler(upButton_Pressed);
            downButton.Pressed += new EventHandler(downButton_Pressed);

            MobileFortress.Input.GetKeyboard().KeyPressed += new Nuclex.Input.Devices.KeyDelegate(Counter_KeyPressed);
        }

        void Counter_KeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Keys.Enter && display.HasFocus)
            {
                int n;
                if (!int.TryParse(display.Text, out n))
                {
                    display.Text = Count.ToString();
                    return;
                }
                Count = Math.Max(min, Math.Min(max, n));
            }
        }

        void downButton_Pressed(object sender, EventArgs e)
        {
            Count = Math.Max(min, Count - 1);
        }

        void upButton_Pressed(object sender, EventArgs e)
        {
            Count = Math.Min(max, Count + 1);
        }
    }
}
