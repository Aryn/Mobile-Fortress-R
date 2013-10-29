using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;

namespace Mobile_Fortress_R.GUI
{
    public class MenuButtonControl : ButtonControl
    {
        public string SkinTag { get; private set; }
        public string[] states { get; private set; }
        public MenuButtonControl(string tag)
            : base()
        {
            SkinTag = tag;
            states = new string[] {
                SkinTag + ".disabled",
                SkinTag + ".normal",
                SkinTag + ".highlighted",
                SkinTag + ".depressed"
                };
        }
    }
}
