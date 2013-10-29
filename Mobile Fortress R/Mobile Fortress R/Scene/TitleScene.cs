using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Common.Net;
using Mobile_Fortress_R.GUI;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface.Controls;
using Common.MobileFortress;

namespace Mobile_Fortress_R.Rendering
{
    public class TitleScene : Scene
    {
        IconControl title;

        LocalPiece refractor;
        LocalPiece room;

        Vector3 LightDirection = Vector3.Down * 0.3f + Vector3.Backward * 0.7f;

        Screen screen;

        //new Vector3(0.02f, 3.52f, -5.35f)

        public TitleScene()
            : base()
        {
            //new Element(Width / 2 - w / 2, 50 - h / 2, w, h, Cache.Get<Texture2D>("Sprites/Menu/Title"));
            //connectButton = new TextButton(Width / 2, 150, "Connect", new Action(Connect));
            //testButton = new TextButton(Width / 2, 350, "Test World Rendering", new Action(Test));
            //shipBuilderButton = new TextButton(Width / 2, 250, "Build Fortress", new Action(Build));

            var roomM = Cache.Get<Model>("Models/Menu/RefractorStand");
            var refractorM = Cache.Get<Model>("Models/Menu/Refractor");

            refractor = new LocalPiece(new Vector3(0.02f, 3.52f, -5.35f), Quaternion.Identity, refractorM);
            room = new LocalPiece(Vector3.Zero, Quaternion.Identity, roomM);

            Camera.Setup(MobileFortress.GraphicsDevice.Viewport);
            Camera.Move(Camera.Position);
            Camera.Target = refractor.Position();
            SetupEffect();
        }

        public override Screen LoadGUI()
        {
            screen = new Screen(MobileFortress.Viewport.Width, MobileFortress.Viewport.Height);
            screen.Desktop.Bounds = new UniRectangle(
                new UniScalar(0.1f, 0), new UniScalar(0.1f, 0),
                new UniScalar(0.8f, 0), new UniScalar(0.8f, 0));
            MobileFortress.SwitchGUIScreen(screen);

            int w = 447;
            int h = 41;
            Texture2D titleTex = Cache.Get<Texture2D>("Sprites/Menu/Title");
            title = new IconControl(titleTex, new Rectangle(0, 0, titleTex.Width, titleTex.Height));
            title.SetBounds(0.5f, -w / 2, 0f, 10 - (h / 2), 0f, w, 0f, h);
            screen.Desktop.Children.Add(title);

            ButtonControl button = new ButtonControl();
            button.Text = "Offline Mode";
            button.SetStaticSizeBoundsCentered(0.5f, 0.25f, 128, 32);
            button.Bounds.Location.Y = new UniScalar(0, 64);
            button.Pressed += new EventHandler(test_Pressed);
            screen.Desktop.Children.Add(button);

            button = new ButtonControl();
            button.Text = "Ship Builder";
            button.SetStaticSizeBoundsCentered(0.5f, 0.25f, 128, 32);
            button.Bounds.Location.Y = new UniScalar(0, 96);
            button.Pressed += new EventHandler(shipBuilder_Pressed);
            screen.Desktop.Children.Add(button);

            //AlertBox.Display("Welcome to the title screen mothafucka.");

            return screen;
        }

        public override void DisposeGUI() { }

        void shipBuilder_Pressed(object sender, EventArgs e)
        {
            MobileFortress.SwitchScene(new ShipBuildingScene());
        }

        void test_Pressed(object sender, EventArgs e)
        {
            MobileFortress.ClearGUIScreen();
            Camera.Move(Vector3.Up * 500);
            MobileFortress.SwitchScene(new SectorScene(FortressMap.LoadFromPath("Ships/Castle.mf")));
        }

        void Connect()
        {
            MobileFortress.NetConnect();
        }

        public override void Update(float dt)
        {
            //if (Engine.Controller.WasMouseClicked())
            //{
            //    Point M = Engine.Controller.GetMousePosition();
            //    if (connectButton.ContainsPointer(M.X, M.Y))
            //    {
            //        connectButton.Push();
            //    }
            //    if (testButton.ContainsPointer(M.X, M.Y))
            //    {
            //        testButton.Push();
            //    }
            //    if (shipBuilderButton.ContainsPointer(M.X, M.Y))
            //        shipBuilderButton.Push();
            //}
            refractor.Rotate(MathHelper.ToRadians(-35f) * dt);
            Camera.Move(Vector3.Transform(Camera.Position, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(2f) * dt)));
        }

        void SetupEffect()
        {
            {
                var custom = Cache.Get<Effect>("Shaders/Normal").Clone();
                room.effect = custom;
                custom.Setup();
                custom.SetTextureFile("Textures/FortressPieces/HexWall");
                custom.SetNormalMap("Textures/FortressPieces/NormalMaps/HexWall", 2);
            }
            room.ApplyEffect();
            {
                var custom = Cache.Get<Effect>("Shaders/Normal").Clone();
                refractor.effect = custom;
                custom.Setup();
                custom.SetTextureFile("Textures/FortressPieces/Tiles");
                custom.SetNormalMap("Textures/FortressPieces/NormalMaps/Tiles", 2);
                custom.SetAmbient(Vector3.One * 0.1f + Vector3.UnitZ * 0.1f);
            }
            refractor.ApplyEffect();
            refractor.ApplyPointLight(refractor.Position() + Vector3.Down*2, Vector3.One, 5);
            room.ApplyPointLight(refractor.Position() + Vector3.Down*2, Vector3.One, 5);
            refractor.ApplyPointLight(Vector3.Zero, Vector3.One, 15);
            room.ApplyPointLight(Vector3.Zero, Vector3.One, 15);
        }

        public override void Render()
        {
            room.Draw(null);
            refractor.Draw(null);
        }

        public override void Interpret(Lidgren.Network.NetIncomingMessage msg)
        {
            var data = (DataType)msg.ReadByte();
            switch (data)
            {
                case DataType.Sector:
                    {
                        MobileFortress.BuildSector(msg);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void OnConnect()
        {
            Console.WriteLine("Now comes the part that's a huge bitch.");
        }

        public override void OnConnectionFail()
        {
        }

        
    }
}
