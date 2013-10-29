using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Mobile_Fortress_R.Rendering;
using Common.Net;
using Lidgren.Network;
using Common;
using System.IO;

namespace Mobile_Fortress_R
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CoreGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public CoreGame()
        {
            //root = this;
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            //Net = new Common.Net.Network(true, this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Net.Cycle();
            //if (IsActive)
            //{
            //    // Allows the game to exit
            //    KeyboardState K = Keyboard.GetState();
            //    if (K.IsKeyDown(Keys.Escape))
            //        this.Exit();

            //    // TODO: Add your update logic here

            //    scene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            //}
            if (!MobileFortress.isReady) MobileFortress.Initialize(this);
            MobileFortress.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //DrawToTarget();
            //GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
            //    SamplerState.LinearClamp, DepthStencilState.Default,
            //    RasterizerState.CullNone, postProcessingEffect);
            //SetPostProcessing(gameTime);
            //spriteBatch.Draw(screen,
            //        new Rectangle(0, 0,
            //            GraphicsDevice.PresentationParameters.BackBufferWidth,
            //            GraphicsDevice.PresentationParameters.BackBufferHeight),
            //        Color.White);
            //spriteBatch.End();
            MobileFortress.Draw();
            base.Draw(gameTime);
        }
    }
}
