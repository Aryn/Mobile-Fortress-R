using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using Common.MobileFortress;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mobile_Fortress_R.GUI
{
    class Octagon : Control
    {
        public MapTile tile = new MapTile();

        List<GraphicalButtonControl> buttons = new List<GraphicalButtonControl>(9);
        
        public Octagon(int x, int y)
            : base()
        {
            Bounds = new UniRectangle(new UniScalar(0.0f, x), new UniScalar(0.0f, y), new UniScalar(0.0f, 48), new UniScalar(0.0f, 48));
            Texture2D sheet = Cache.Get<Texture2D>("Sprites/Builder/ButtonSheet");
            Rectangle source = new Rectangle(16, 0, 16, 16);
            GraphicalButtonControl borderTop;
            GraphicalButtonControl borderBottom;
            for (int bx = 0; bx < 3; bx++)
            {
                borderTop = new GraphicalButtonControl(sheet, source);
                borderBottom = new GraphicalButtonControl(sheet, source);
                borderTop.SetStaticSizeBounds(0.33f * bx, 0, source.Width, source.Height);
                borderBottom.SetStaticSizeBounds(0.33f * bx, 0.66f, source.Width, source.Height);
                borderBottom.VerticalFlip = true;
                buttons.Add(borderTop);
                buttons.Add(borderBottom);
                source.X += source.Width;
            }
            source = new Rectangle(64, 0, 16, 16);
            borderTop = new GraphicalButtonControl(sheet, source);
            borderBottom = new GraphicalButtonControl(sheet, source);
            borderTop.SetStaticSizeBounds(0f, 0.33f, source.Width, source.Height);
            borderBottom.SetStaticSizeBounds(0.66f, 0.33f, source.Width, source.Height);
            borderBottom.HorizontalFlip = true;
            buttons.Add(borderTop);
            buttons.Add(borderBottom);

            source = new Rectangle(80, 0, 16, 16);
            borderTop = new GraphicalButtonControl(sheet, source);
            borderTop.SetStaticSizeBounds(0.33f, 0.33f, source.Width, source.Height);
            buttons.Add(borderTop);

            foreach (GraphicalButtonControl B in buttons)
            {
                Children.Add(B);
            }

            UpdateControl();
            SubscribeAll();
        }

        void UpdateControl()
        {
            buttons[0].Selected = tile.Diagonal && tile.North && tile.West;
            buttons[1].Selected = tile.Diagonal && tile.South && tile.West;
            buttons[4].Selected = tile.Diagonal && tile.North && tile.East;
            buttons[5].Selected = tile.Diagonal && tile.South && tile.East;
            buttons[2].Selected = !tile.Diagonal && tile.North;
            buttons[3].Selected = !tile.Diagonal && tile.South;
            buttons[6].Selected = !tile.Diagonal && tile.West;
            buttons[7].Selected = !tile.Diagonal && tile.East;
            buttons[8].Selected = tile.Floor;
        }
        void SubscribeAll()
        {
            if(!MobileFortress.TestingMode) MobileFortress.Input.GetKeyboard().KeyPressed += new Nuclex.Input.Devices.KeyDelegate(Octagon_KeyPressed);
            buttons[0].Pressed += new EventHandler(DiagNW);
            buttons[1].Pressed += new EventHandler(DiagSW);
            buttons[2].Pressed += new EventHandler(North);
            buttons[3].Pressed += new EventHandler(South);
            buttons[4].Pressed += new EventHandler(DiagNE);
            buttons[5].Pressed += new EventHandler(DiagSE);
            buttons[6].Pressed += new EventHandler(West);
            buttons[7].Pressed += new EventHandler(East);
            buttons[8].Pressed += new EventHandler(Floor);
        }

        void UnsetDiag()
        {
            if (tile.Diagonal)
            {
                if (tile.Floor)
                    tile.Surface = SurfaceF.FLOOR;
                else
                    tile.Surface = SurfaceF.EMPTY;
            }
        }

        void Octagon_KeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (MobileFortress.Controller.Shift() || MobileFortress.Controller.Alt() || MobileFortress.Controller.Ctrl()) return;
            switch (key)
            {
                case Keys.W:
                    North(null, null);
                    break;
                case Keys.A:
                    West(null, null);
                    break;
                case Keys.S:
                    South(null, null);
                    break;
                case Keys.D:
                    East(null, null);
                    break;
                case Keys.Q:
                    Floor(null, null);
                    break;
            }
        }

        void North(object sender, EventArgs e)
        {
            UnsetDiag();
            tile.Surface ^= SurfaceF.NORTH;
            UpdateControl();
        }
        void South(object sender, EventArgs e)
        {
            UnsetDiag();
            tile.Surface ^= SurfaceF.SOUTH;
            UpdateControl();
        }
        void West(object sender, EventArgs e)
        {
            UnsetDiag();
            tile.Surface ^= SurfaceF.WEST;
            UpdateControl();
        }
        void East(object sender, EventArgs e)
        {
            UnsetDiag();
            tile.Surface ^= SurfaceF.EAST;
            UpdateControl();
        }
        void Floor(object sender, EventArgs e)
        {
            tile.Surface ^= SurfaceF.FLOOR;
            UpdateControl();
        }
        void DiagNW(object sender, EventArgs e)
        {
            SurfaceF diagtype = SurfaceF.DIAG | SurfaceF.NORTH | SurfaceF.WEST;
            SurfaceF pairtype = SurfaceF.DIAG | SurfaceF.SOUTH | SurfaceF.EAST | SurfaceF.FLOOR;
            if (tile.Surface != pairtype)
            {
                if (tile.Surface.HasFlag(SurfaceF.FLOOR)) diagtype |= SurfaceF.FLOOR;
                tile.Surface = diagtype;
            }
            else
            {
                tile.Surface = pairtype | SurfaceF.NORTH;
            }
            UpdateControl();
        }
        void DiagNE(object sender, EventArgs e)
        {
            SurfaceF diagtype = SurfaceF.DIAG | SurfaceF.NORTH | SurfaceF.EAST;
            SurfaceF pairtype = SurfaceF.DIAG | SurfaceF.SOUTH | SurfaceF.WEST | SurfaceF.FLOOR;
            if (tile.Surface != pairtype)
            {
                if (tile.Surface.HasFlag(SurfaceF.FLOOR)) diagtype |= SurfaceF.FLOOR;
                tile.Surface = diagtype;
            }
            else
            {
                tile.Surface = pairtype | SurfaceF.NORTH;
            }
            UpdateControl();
        }
        void DiagSW(object sender, EventArgs e)
        {
            SurfaceF diagtype = SurfaceF.DIAG | SurfaceF.SOUTH | SurfaceF.WEST;
            SurfaceF pairtype = SurfaceF.DIAG | SurfaceF.NORTH | SurfaceF.EAST | SurfaceF.FLOOR;
            if (tile.Surface != pairtype)
            {
                if (tile.Surface.HasFlag(SurfaceF.FLOOR)) diagtype |= SurfaceF.FLOOR;
                tile.Surface = diagtype;
            }
            else
            {
                tile.Surface = pairtype | SurfaceF.SOUTH;
            }
            UpdateControl();
        }
        void DiagSE(object sender, EventArgs e)
        {
            SurfaceF diagtype = SurfaceF.DIAG | SurfaceF.SOUTH | SurfaceF.EAST;
            SurfaceF pairtype = SurfaceF.DIAG | SurfaceF.NORTH | SurfaceF.WEST | SurfaceF.FLOOR;
            if (tile.Surface != pairtype)
            {
                if (tile.Surface.HasFlag(SurfaceF.FLOOR)) diagtype |= SurfaceF.FLOOR;
                tile.Surface = diagtype;
            }
            else
            {
                tile.Surface = pairtype | SurfaceF.SOUTH;
            }
            UpdateControl();
        }
    }
}
