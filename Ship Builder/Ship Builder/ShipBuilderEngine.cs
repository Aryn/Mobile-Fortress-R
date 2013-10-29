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
using Ship_Builder.UserInterface;

namespace Ship_Builder
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShipBuilderEngine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BuilderInterface grid;

        static MouseState prevMouse;
        static KeyboardState prevKeyboard;

        static MouseState currentMouse;
        static KeyboardState currentKeyboard;

        public static bool LeftClick()
        {
            return currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        }
        public static bool KeyPressed(Keys K)
        {
            return currentKeyboard.IsKeyDown(K) && !prevKeyboard.IsKeyDown(K);
        }

        public ShipBuilderEngine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            prevMouse = Mouse.GetState();
            prevKeyboard = Keyboard.GetState();
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
            Counter.LoadTextures(GraphicsDevice, Content);
            UIComponent.UIFont = Content.Load<SpriteFont>("UI");
            grid = new BuilderInterface(GraphicsDevice);
            
            
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            grid.Update(dt);

            MouseState M = Mouse.GetState();
            bool LeftClick = M.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
            if (LeftClick && grid.Dimensions.Contains(M.X,M.Y))
            {
                grid.LeftClicked(M.X, M.Y);
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            grid.Draw(spriteBatch);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
