using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Mobile_Fortress_R.GUI;
using Nuclex.UserInterface.Controls.Desktop;
using Mobile_Fortress_R.Rendering;
using Common.MobileFortress;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.Input.Devices;

namespace Mobile_Fortress_R
{
    public partial class ShipBuildingScene
    {
        Point MouseOver = Point.Zero;
        MouseButtonDelegate mouseUp;
        MouseButtonDelegate mouseDown;
        MouseMoveDelegate mouseMoved;
        public override Nuclex.UserInterface.Screen LoadGUI()
        {
            sheet = Cache.Get<Texture2D>("Sprites/Builder/ButtonSheet");
            highlight = Cache.Get<Texture2D>("Sprites/Builder/Highlight");
            var screen = new Nuclex.UserInterface.Screen(MobileFortress.Viewport.Width, MobileFortress.Viewport.Height);

            plane = new BuildingPlane(60, 33);

            WindowControl toolsWindow = new WindowControl();
            toolsWindow.SetBounds(1f, -192, 0f, 24, 0f, 176, 1f, -48);
            toolsWindow.Title = "Building Tools";
            toolsWindow.EnableDragging = true;
            screen.Desktop.Children.Add(toolsWindow);

            toolsWindow.Children.Add(octagon);

            var newButton = new ButtonControl();
            newButton.SetBounds(0, 4, 0, 24, 0, 48, 0, 20);
            newButton.Text = "New";
            newButton.Pressed += new EventHandler(newButton_Pressed);
            toolsWindow.Children.Add(newButton);

            var loadButton = new ButtonControl();
            loadButton.SetBounds(0, 5 + 48, 0, 24, 0, 48, 0, 20);
            loadButton.Text = "Load";
            loadButton.Pressed += new EventHandler(loadButton_Pressed);
            toolsWindow.Children.Add(loadButton);

            var saveButton = new ButtonControl();
            saveButton.SetBounds(0, 6 + 96, 0, 24, 0, 48, 0, 20);
            saveButton.Text = "Save";
            saveButton.Pressed += new EventHandler(saveButton_Pressed);
            toolsWindow.Children.Add(saveButton);

            var closeButton = new GraphicalButtonControl(sheet, new Rectangle(0, 0, 16, 16));
            closeButton.SetBounds(0, 9 + 96 + 48, 0, 25, 0, 18, 0, 18);
            closeButton.Pressed += new EventHandler(closeButton_Pressed);
            toolsWindow.Children.Add(closeButton);

            Counter layerCounter = new Counter(0, 0, 25, 24, 56);
            toolsWindow.Children.Add(layerCounter);
            layerCounter.Changed += new EventHandler(layerCounter_Changed);

            IconControl layerIcon = new IconControl(sheet, new Rectangle(96, 0, 16, 16));
            layerIcon.SetBounds(0, 6, 0, 55, 0, 16, 0, 16);
            toolsWindow.Children.Add(layerIcon);

            loadWindow = new LoadShipWindow();
            loadWindow.SetBounds(0.5f, -225, 0.5f, -125, 0f, 450, 0f, 250);

            saveWindow = new SaveShipWindow();
            saveWindow.SetBounds(0.5f, -225, 0.5f, -125, 0f, 450, 0f, 250);

            confirm = new ChoiceBox("The current map has been modified. Do you want to save your changes?", new string[] { "Save", "Discard", "Cancel" });

            tools[0].toolButton = new GraphicalButtonControl(sheet, new Rectangle(0, 64, 16, 16));
            tools[0].toolButton.SetBounds(0f, 64 + 16, 0f, 55, 0f, 16, 0f, 16);
            toolsWindow.Children.Add(tools[0].toolButton);
            tools[0].toolButton.Pressed += new EventHandler(toolButton_Pressed);
            tools[0].toolButton.Name = "0";

            tools[1].toolButton = new GraphicalButtonControl(sheet, new Rectangle(16, 64, 16, 16));
            tools[1].toolButton.SetBounds(0f, 96 + 4, 0f, 55, 0f, 16, 0f, 16);
            toolsWindow.Children.Add(tools[1].toolButton);
            tools[1].toolButton.Pressed += new EventHandler(toolButton_Pressed);
            tools[1].toolButton.Name = "1";

            ButtonControl testShipButton = new ButtonControl();
            testShipButton.SetBounds(0f, 4, 0f, 96, 0f, 48, 0f, 20);
            toolsWindow.Children.Add(testShipButton);
            testShipButton.Pressed += new EventHandler(testShipButton_Pressed);

            mouseUp = new Nuclex.Input.Devices.MouseButtonDelegate(ShipBuildingScene_MouseButtonReleased);
            mouseDown = new Nuclex.Input.Devices.MouseButtonDelegate(ShipBuildingScene_MouseButtonPressed);
            mouseMoved = new Nuclex.Input.Devices.MouseMoveDelegate(ShipBuildingScene_MouseMoved);

            MobileFortress.Input.GetMouse().MouseButtonPressed += mouseDown;
            MobileFortress.Input.GetMouse().MouseButtonReleased += mouseUp;
            MobileFortress.Input.GetMouse().MouseMoved += mouseMoved;

            return screen;
        }

        public override void DisposeGUI()
        {
            MobileFortress.Input.GetMouse().MouseButtonPressed -= mouseDown;
            MobileFortress.Input.GetMouse().MouseButtonReleased -= mouseUp;
            MobileFortress.Input.GetMouse().MouseMoved -= mouseMoved;
        }

        void testShipButton_Pressed(object sender, EventArgs e)
        {
            MobileFortress.SwitchScene(new SectorScene(map));
        }

        void toolButton_Pressed(object sender, EventArgs e)
        {
            GraphicalButtonControl button = (GraphicalButtonControl)sender;
            SwapTool(tools[int.Parse(button.Name)]);
        }

        void ShipBuildingScene_MouseMoved(float x, float y)
        {
            MouseState S = MobileFortress.Input.GetMouse().GetState();
            Point M;
            if ((toolState != ToolState.Up) && plane.MouseOverTile(out M))
            {
                if (toolState == ToolState.Down)
                    currentTool.ToolDrag(this, M.X, M.Y);
                else
                    currentTool.ToolRightDrag(this, M.X, M.Y);
            }
            if (!MobileFortress.GUI.Screen.IsMouseOverGui)
                MouseOver = new Point(S.X / plane.BlockWidth, S.Y / plane.BlockHeight);
        }

        void ShipBuildingScene_MouseButtonReleased(Nuclex.Input.MouseButtons buttons)
        {
            Point M;
            if ((toolState != ToolState.Up) && plane.MouseOverTile(out M))
            {
                if (buttons == MouseButtons.Left)
                {
                    currentTool.ToolUp(this, M.X, M.Y);
                    toolState = ToolState.Up;
                }
                else if(buttons == MouseButtons.Right)
                {
                    currentTool.ToolRightUp(this, M.X, M.Y);
                    toolState = ToolState.Up;
                }
            }
        }

        void ShipBuildingScene_MouseButtonPressed(Nuclex.Input.MouseButtons buttons)
        {
            Point M;
            if (plane.MouseOverTile(out M))
            {
                if (buttons == MouseButtons.Left)
                {
                    currentTool.ToolDown(this, M.X, M.Y);
                    currentTool.ToolDrag(this, M.X, M.Y);
                    toolState = ToolState.Down;
                }
                else if (buttons == MouseButtons.Right)
                {
                    currentTool.ToolRightDown(this, M.X, M.Y);
                    currentTool.ToolRightDrag(this, M.X, M.Y);
                    toolState = ToolState.Reversed;
                }
            }
        }

        void layerCounter_Changed(object sender, EventArgs e)
        {
            layer = ((Counter)sender).Count;
            while (layer > map.LayerCount - 1)
            {
                map.AddEmptyLayer();
            }
        }

        void closeButton_Pressed(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                confirming = ConfirmState.Close;
                confirm.Display();
            }
            else
                MobileFortress.SwitchScene(new TitleScene());
        }

        void saveButton_Pressed(object sender, EventArgs e)
        {
            saveActive = true;
            MobileFortress.GUI.Screen.Desktop.Children.Add(saveWindow);
        }

        void loadButton_Pressed(object sender, EventArgs e)
        {
            loadActive = true;
            if (unsavedChanges)
            {
                confirming = ConfirmState.Load;
                confirm.Display();
            }
            else
                MobileFortress.GUI.Screen.Desktop.Children.Add(loadWindow);
        }

        void newButton_Pressed(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                confirming = ConfirmState.New;
                confirm.Display();
            }
            else
            {
                map = FortressMap.Empty();
                unsavedChanges = false;
            }
        }
    }
}
