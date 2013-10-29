using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile_Fortress_R.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Common.MobileFortress.Old;
using NGenerics.DataStructures.Queues;
using System.IO;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Mobile_Fortress_R.GUI;
using Microsoft.Xna.Framework.Graphics;

namespace Mobile_Fortress_R.Old
{
    public class ShipBuildingScene : Scene
    {
        const int GridWidth = 35;
        const int GridHeight = 25;

        FortressMap map;
        int layer = 0;

        Octagon octagon;
        Stack<UndoCommand> undo = new Stack<UndoCommand>(50);

        LoadShipWindow loadShipWindow;
        SaveShipWindow saveShipWindow;

        bool loadActive = false;
        bool saveActive = false;
        

        public ShipBuildingScene()
        {
            map = FortressMap.GenerateTestMap();
        }

        public override Screen LoadGUI()
        {
            var screen = new Screen(MobileFortress.Viewport.Width, MobileFortress.Viewport.Height);

            int xOffset = MobileFortress.Viewport.Width - 220;

            AddButton(screen, "Load", xOffset, 16, 64, new EventHandler(Load));
            AddButton(screen, "Save", xOffset + 64, 16, 64, new EventHandler(Save));
            AddButton(screen, "New",  xOffset + 128, 16, 64, new EventHandler(New));

            Counter layerCounter = new Counter(0, 0, 99, xOffset + 24, 48);
            screen.Desktop.Children.Add(layerCounter);
            layerCounter.Changed += new EventHandler(layerCounter_Changed);

            //Element layerIcon = new Element(xOffset + 4, 48, 16, 16, Cache.Get<Texture2D>("Sprites/Builder/LayerIcon"));
            //Elements.Add(layerIcon);

            octagon = new Octagon(xOffset + 24, 80);
            screen.Desktop.Children.Add(octagon);

            MobileFortress.Input.GetKeyboard().KeyPressed += new Nuclex.Input.Devices.KeyDelegate(ShipBuildingScene_KeyPressed);
            //MobileFortress.Input.GetMouse().MouseButtonPressed += new Nuclex.Input.Devices.MouseButtonDelegate(ShipBuildingScene_MouseButtonPressed);

            loadShipWindow = new LoadShipWindow();
            loadShipWindow.SetStaticSizeBoundsCentered(0.5f, 0.5f, 400, 250);

            saveShipWindow = new SaveShipWindow();
            saveShipWindow.SetStaticSizeBoundsCentered(0.5f, 0.5f, 400, 250);

            return screen;
        }

        void ShipBuildingScene_KeyPressed(Keys key)
        {
            if (key == Keys.Z && MobileFortress.Controller.Ctrl() && undo.Count > 0)
            {
                var cmd = undo.Pop();
                cmd.Execute(map);
            }
        }

        void layerCounter_Changed(object sender, EventArgs e)
        {
            layer = ((Counter)sender).Count;
            while (layer > map.Layers - 1)
            {
                map.ExpandLayer();
            }
        }

        void AddButton(Screen screen, string text, int x, int y, int w, EventHandler handler)
        {
            ButtonControl button = new ButtonControl();
            button.Bounds = new UniRectangle(new UniScalar(0f, x), new UniScalar(0f, y), new UniScalar(0f, w), new UniScalar(0f, 24));
            button.Text = text;
            button.Pressed += handler;
            screen.Desktop.Children.Add(button);
        }

        public override void Render()
        {
        }

        void GridClick(int x, int y, Nuclex.Input.MouseButtons buttons)
        {
            if (x >= map.Width || y >= map.Height) return;
            undo.Push(new UndoCommand(layer, x, y, map.GetTile(layer, x, y)));
            if (buttons == Nuclex.Input.MouseButtons.Right)
            {
                map.SetTile(layer, x, y, TileType.EMPTY);
            }
            else
            {
                //map.SetTile(layer, x, y, octagon.Flags);
            }
        }

        void New(object sender, EventArgs e)
        {
            map = new FortressMap(GridWidth, GridHeight);
            map.ExpandLayer();
        }
        void Load(object sender, EventArgs e)
        {
            loadActive = true;
            MobileFortress.GUI.Screen.Desktop.Children.Add(loadShipWindow);
            //if(File.Exists("MapL.mf"))
            //    map = FortressMap.ReadMapFromMF("MapL.mf", 40, 25);
        }
        void Save(object sender, EventArgs e)
        {
            saveActive = true;
            MobileFortress.GUI.Screen.Desktop.Children.Add(saveShipWindow);
            //int n = 1;
            //while(File.Exists("Map" + n + ".mf"))
            //{
            //    n++;
            //}
            //map.Save("Map" + n + ".mf");
        }


        struct UndoCommand
        {
            int layer;
            int x;
            int y;
            TileType prev;
            public UndoCommand(int layer, int x, int y, TileType prev)
            {
                this.layer = layer;
                this.x = x;
                this.y = y;
                this.prev = prev;
            }
            public void Execute(FortressMap map)
            {
                map.SetTile(layer, x, y, prev);
            }
        }

        public override void Update(float dt)
        {
            if (loadActive && !loadShipWindow.IsOpen)
            {
                if (loadShipWindow.Selection != null)
                {
                    map = FortressMap.ReadMapFromMF(loadShipWindow.Selection.ToString(), GridWidth, GridHeight);
                }
                loadActive = false;
                loadShipWindow.Reset();
            }
            if (saveActive)
            {
                if (!saveShipWindow.IsOpen)
                {
                    if (saveShipWindow.FileName != null)
                    {
                        map.Save(saveShipWindow.FileName);
                    }
                    saveActive = false;
                    saveShipWindow.Reset();
                }
                else if (saveShipWindow.Confirmation.Choice == "OK")
                {
                    saveShipWindow.Close();
                }
            }
            if (!MobileFortress.GUI.Screen.IsMouseOverGui)
            {
                //var M = MobileFortress.Controller.GetMousePosition();
                //if (MobileFortress.Controller.IsMouseLeftButtonDown())
                //    grid.Update(M, Nuclex.Input.MouseButtons.Left);
                //else if (MobileFortress.Controller.IsMouseRightButtonDown())
                //    grid.Update(M, Nuclex.Input.MouseButtons.Right);
            }
        }

        public override void DisposeGUI()
        {
            throw new NotImplementedException();
        }
    }
}
