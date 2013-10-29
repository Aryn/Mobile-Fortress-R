using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;

namespace Mobile_Fortress_R.GUI
{
    public class IconControl : Control
    {
        public Texture2D Sheet { get; private set; }
        public Rectangle Source { get; private set; }
        public SpriteBatch Batch { get; set; }

        public bool VerticalFlip { get; set; }
        public bool HorizontalFlip { get; set; }
        public IconControl(Texture2D sheet, Rectangle source)
            : base()
        {
            Sheet = sheet;
            Source = source;
        }
    }
}
