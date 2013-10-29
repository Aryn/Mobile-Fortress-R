using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using Lidgren.Network;
using Mobile_Fortress_R.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Common;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Common.Base;
using System.IO;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.Input;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat.Renderers;
using System.Reflection;

namespace Mobile_Fortress_R
{
    enum DrawMode { Normal, Additive, Interface, Background }
    class MobileFortress : INetInterpreter, IController
    {
        static MobileFortress root;
        public static bool TestingMode = true;

        //Accessors
        public static RenderTarget2D Screen { get { return root.screen; } }
        public static SpriteBatch Sprites { get { return root.client.spriteBatch; } }
        public static ContentManager Content { get { return root.client.Content; } }
        public static GraphicsDevice GraphicsDevice { get { return root.client.GraphicsDevice; } }
        public static Viewport Viewport { get { return root.client.GraphicsDevice.Viewport; } }
        public static bool isReady { get { return root != null; } }
        public static IController Controller { get { return (IController)root; } }

        IControllable controlling;

        public static void SwitchControlTo(IControllable puppet)
        {
            if(root.controlling != null) root.controlling.ReleaseControl(root);
            root.controlling = puppet;
            if (root.controlling != null) root.controlling.TakeControl(root);
        }

        //Base Classes
        CoreGame client;
        Scene scene;
        public static CoreGame Client { get { return root.client; } }

        //Network
        Network net;
        bool isConnected = false;

        //Graphics
        RenderTarget2D screen;
        Effect postProcessing;
        float gameTime = 0f;
        
        //Input
        MouseState currentMouse;
        MouseState oldMouse;
        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;

        //GUI
        public static Nuclex.UserInterface.GuiManager GUI { get { return root.gui; } }
        public static Nuclex.Input.InputManager Input { get { return root.input; } }
        Nuclex.UserInterface.GuiManager gui;
        Nuclex.Input.InputManager input;

        GraphicsDevice device { get { return client.GraphicsDevice; } }
        PresentationParameters graphicSettings { get { return client.GraphicsDevice.PresentationParameters; } }

        public static void Initialize(CoreGame c)
        {
            TestingMode = false;
            root = new MobileFortress(c);
        }

        private MobileFortress(CoreGame c)
        {
            root = this;
            client = c;
            //Start network
            net = new Network(true, this);
            //Start GUI
            input = new InputManager(client.Services);
            client.Components.Add(input);
            gui = new GuiManager(client.Services);
            client.Components.Add(gui);
            var visualizer = FlatGuiVisualizer.FromFile(Content.ServiceProvider, "Content/Skins/Suave.skin.xml");
            visualizer.RendererRepository.AddAssembly(Assembly.GetExecutingAssembly());
            gui.Visualizer = visualizer;
            //Start scene
            SwitchScene(new TitleScene());
            //Start rendering
            screen = new RenderTarget2D(device,
                graphicSettings.BackBufferWidth,
                graphicSettings.BackBufferHeight,
                false, graphicSettings.BackBufferFormat, DepthFormat.Depth24);
            postProcessing = Cache.Get<Effect>("Shaders/PostProcessing");
            currentKeyboard = Keyboard.GetState();
            oldKeyboard = currentKeyboard;
            currentMouse = Mouse.GetState();
            oldMouse = currentMouse;
        }

        #region Network
        public void Interpret(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)msg.ReadByte();
                    switch (status)
                    {
                        case NetConnectionStatus.Connected:
                            Console.WriteLine("Successful Connection: " + msg.SenderEndPoint.Address);
                            scene.OnConnect();
                            isConnected = true;
                            break;
                        case NetConnectionStatus.Disconnected:
                            if (isConnected)
                            {
                                Console.WriteLine("Disconnected.");
                                scene.OnDisconnect();
                            }
                            else
                            {
                                Console.WriteLine("Connection failed.");
                                scene.OnConnectionFail();
                            }
                            break;
                        default:
                            Console.WriteLine(status + ": " + msg.SenderEndPoint.Address);
                            break;
                    }
                    break;
                case NetIncomingMessageType.Data:
                    scene.Interpret(msg);
                    break;
            }
        }

        public static void NetConnect()
        {
            root.net.TryConnect();
        }

        public static void BuildSector(NetIncomingMessage msg)
        {
            var p = msg.ReadPoint();
            root.scene = new SectorScene(p);
            //Write the rest of the stuff.
        }
        #endregion

        #region Rendering
        public static void Draw()
        {
            root.DrawGame();
            root.DrawScreen();
        }
        void DrawGame()
        {
            device.SetRenderTarget(screen);
            device.Clear(Color.Black);
            scene.Render();
        }
        void DrawScreen()
        {
            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            client.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                 SamplerState.LinearClamp, DepthStencilState.Default,
                 RasterizerState.CullNone, postProcessing);
            SetPostProcessing();
            client.spriteBatch.Draw(screen,
                    new Rectangle(0, 0,
                        graphicSettings.BackBufferWidth,
                        graphicSettings.BackBufferHeight),
                    Color.White);
            client.spriteBatch.End();
        }

        void SetPostProcessing()
        {
            if (Camera.Position.Y < Sector.seaLevel)
            {
                postProcessing.CurrentTechnique = postProcessing.Techniques["Wave"];
            }
            else
            {
                postProcessing.CurrentTechnique = postProcessing.Techniques["None"];
            }
            postProcessing.Parameters["time"].SetValue(gameTime);
        }
        public static void SetDrawMode(DrawMode mode)
        {
            switch (mode)
            {
                case DrawMode.Additive:
                    root.device.BlendState = BlendState.Additive;
                    root.device.DepthStencilState = DepthStencilState.DepthRead;
                    root.device.RasterizerState = RasterizerState.CullNone;
                    break;
                case DrawMode.Background:
                case DrawMode.Interface:
                    root.device.BlendState = BlendState.AlphaBlend;
                    root.device.DepthStencilState = DepthStencilState.None;
                    root.device.RasterizerState = RasterizerState.CullCounterClockwise;
                    break;
                default:
                    root.device.BlendState = BlendState.AlphaBlend;
                    root.device.DepthStencilState = DepthStencilState.Default;
                    root.device.RasterizerState = RasterizerState.CullCounterClockwise;
                    break;
            }
        }
        #endregion

        #region Update
        public static void Update(float dt)
        {
            root.net.Cycle();
            root.UpdateInterface();
            root.UpdateGame(dt);
            root.oldKeyboard = root.currentKeyboard;
            root.oldMouse = root.currentMouse;
        }
        void UpdateGame(float dt)
        {
            gameTime += dt;
            if (gameTime > 60f) gameTime -= 60f;
            scene.Update(dt);
        }
        void UpdateInterface()
        {
            currentKeyboard = Keyboard.GetState();
            currentMouse = Mouse.GetState();
            if (client.IsActive)
            {
                if (IsKeyDown(Keys.Escape))
                    client.Exit();
            }
            if(controlling != null)
                controlling.ControlUpdate(this);
        }

        public static void SwitchScene(Scene S)
        {
            if(root.scene != null) root.scene.DisposeGUI();
            root.scene = S;
            SwitchGUIScreen(S.LoadGUI());
            if (GUI.Screen == null) Client.IsMouseVisible = false;
        }
        public static void SwitchGUIScreen(Screen S)
        {
            GUI.Screen = S;
            Client.IsMouseVisible = true;
        }
        public static void ClearGUIScreen()
        {
            GUI.Screen = null;
            Client.IsMouseVisible = false;
        }

        public static void SetMouse(bool visible)
        {
            root.client.IsMouseVisible = visible;
        }

        public bool IsKeyDown(Keys K)
        {
            return root.currentKeyboard.IsKeyDown(K);
        }
        public bool WasKeyPressed(Keys K)
        {
            return root.currentKeyboard.IsKeyDown(K) && root.oldKeyboard.IsKeyUp(K);
        }
        public bool Ctrl()
        {
            return IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
        }
        public bool Shift()
        {
            return IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
        }
        public bool Alt()
        {
            return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
        }
        public Point MouseMovement
        {
            get { return new Point(root.currentMouse.X - root.oldMouse.X,
                root.currentMouse.Y - root.oldMouse.Y); }
        }
        public Point MousePosition
        {
            get { return new Point(root.currentMouse.X, root.currentMouse.Y); }
        }
        public bool WasMouseClicked()
        {
            return root.currentMouse.LeftButton == ButtonState.Pressed &&
                root.oldMouse.LeftButton == ButtonState.Released;
        }
        public bool WasMouseRightClicked()
        {
            return root.currentMouse.RightButton == ButtonState.Pressed &&
                root.oldMouse.RightButton == ButtonState.Released;
        }

        public static KeyboardState GetCurrentKeyboard()
        {
            return root.currentKeyboard;
        }
        public static KeyboardState GetOldKeyboard()
        {
            return root.oldKeyboard;
        }
        #endregion

        public Point GetMousePosition()
        {
            return MousePosition;
        }

        public Point GetMouseDelta()
        {
            return MouseMovement;
        }

        public Vector2 GetAngle()
        {
            return Vector2.Zero;
        }

        public Vector2 GetAngleDelta()
        {
            return Vector2.Zero;
        }


        public bool IsMouseLeftButtonDown()
        {
            return root.currentMouse.LeftButton == ButtonState.Pressed;
        }

        public bool IsMouseRightButtonDown()
        {
            return root.currentMouse.RightButton == ButtonState.Pressed;
        }
    }
}
