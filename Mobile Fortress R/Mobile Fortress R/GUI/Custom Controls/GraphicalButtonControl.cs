using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;

namespace Mobile_Fortress_R.GUI
{
    public class GraphicalButtonControl : ButtonControl
    {
        public Texture2D Sheet { get; private set; }
        public Rectangle Source { get; private set; }
        public SpriteBatch Batch { get; set; }

        public bool VerticalFlip { get; set; }
        public bool HorizontalFlip { get; set; }
        public bool Selected { get; set; }
        public GraphicalButtonControl(Texture2D sheet, Rectangle source)
            : base()
        {
            Sheet = sheet;
            Source = source;
        }
    }
}
