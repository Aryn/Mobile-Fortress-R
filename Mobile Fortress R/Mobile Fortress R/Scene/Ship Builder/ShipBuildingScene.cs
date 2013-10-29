using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MobileFortress;
using Mobile_Fortress_R.GUI;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.UserInterface.Controls.Desktop;
using Microsoft.Xna.Framework;
using Mobile_Fortress_R.Rendering;

namespace Mobile_Fortress_R
{
    public partial class ShipBuildingScene : Scene
    {
        FortressMap map;
        int layer = 0;

        BuildingPlane plane;
        Octagon octagon;
        Texture2D sheet;

        MapTile placingTile { get { return octagon.tile; } }

        LoadShipWindow loadWindow;
        SaveShipWindow saveWindow;
        bool loadActive = false;
        bool saveActive = false;

        enum ConfirmState { None, Close, New, Load }

        ChoiceBox confirm;
        ConfirmState confirming = ConfirmState.None;
        bool unsavedChanges = false;

        public ShipBuildingScene()
        {
            map = FortressMap.Empty();
            octagon = new Octagon(24, 80);
            currentTool = tools[0];
        }

        public override void Update(float dt)
        {
            if (loadActive && !loadWindow.IsOpen)
            {
                Load();
            }
            if (saveActive)
            {
                Save();
            }
            if (confirming != ConfirmState.None && !loadActive && !saveActive)
            {
                Confirm();
            }
        }

        void Load()
        {
            if (confirming == ConfirmState.Load)
            {
                switch (confirm.Choice)
                {
                    case "Save":
                        saveButton_Pressed(null, null);
                        loadActive = false;
                        loadWindow.Reset();
                        break;
                    case "Discard":
                        MobileFortress.GUI.Screen.Desktop.Children.Add(loadWindow);
                        break;
                    default:
                        loadActive = false;
                        loadWindow.Reset();
                        break;
                }
                confirming = ConfirmState.None;
            }
            else
            {
                if (loadWindow.Selection != null)
                {
                    try
                    {
                        map = FortressMap.LoadFromStream(loadWindow.Selection.Open(System.IO.FileMode.Open));
                        unsavedChanges = false;
                    }
                    catch (NotSupportedException e)
                    {
                        AlertBox.Display(e.Message);
                    }
                }
                loadActive = false;
                loadWindow.Reset();
            }
        }
        void Save()
        {
            if (!saveWindow.IsOpen)
            {
                if (saveWindow.FileName != null)
                {
                    map.Save(saveWindow.FileName);
                    unsavedChanges = false;
                    AlertBox.Display("Saved To: " + saveWindow.FileName);
                }
                saveActive = false;
                saveWindow.Reset();
            }
            else if (saveWindow.Confirmation.Choice == "OK")
            {
                saveWindow.Close();
            }
        }
        void Confirm()
        {
            switch (confirm.Choice)
            {
                case "Save":
                    saveButton_Pressed(null, null);
                    break;
                case "Discard":
                    if (confirming == ConfirmState.New)
                    {
                        map = FortressMap.Empty();
                        unsavedChanges = false;
                    }
                    else
                        MobileFortress.SwitchScene(new TitleScene());
                    break;
                default:
                    break;
            }
            confirming = ConfirmState.None;
        }

        public override void Render()
        {
            MobileFortress.Sprites.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            MapLayer L = map.GetLayer(layer);
            plane.Draw(L);
            currentTool.ToolHighlight(this, MouseOver.X, MouseOver.Y);
            MobileFortress.Sprites.End();
        }
    }
}
