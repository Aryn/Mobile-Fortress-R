using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;

namespace Mobile_Fortress_R.GUI
{
    class ClosableWindow : WindowControl
    {
        bool isMinimized { get; set; }
        UniRectangle fullBounds { get; set; }
        public ClosableWindow()
            : base()
        {
            var close = new MenuButtonControl("close");
            close.SetBounds(1f, -20, 0f, 2, 0f, 16, 0f, 16);
            close.Pressed += new EventHandler(close_Pressed);
            Children.Add(close);
        }

        void close_Pressed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
