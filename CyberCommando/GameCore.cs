using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities.Enviroment;
using CyberCommando.Engine;
using CyberCommando.Services;

using System.Collections.Generic;

namespace CyberCommando
{
    public enum ResolutionState
    {
        R1280x720 = 80,
        R1600x900 = 100,
        R1920x1080 = 120,
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameCore : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch CoreBatcher;

        ResolutionState ResolutionCurrent { get; set; }
        ScreenManager SManager;

        public bool NeedExit { get; set; }

        public GameCore()
        {
            // ~~~ UNCOMMENT For fullscreen mode ~~~
            //graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;
            graphics = new GraphicsDeviceManager(this);

            SManager = ScreenManager.Instance;

            //this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;
           
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
            SManager.Initialize(GraphicsDevice, this);

            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        public void Resize(ResolutionState res)
        {
            ResolutionCurrent = res;

            int width = 16 * (int)res;
            int height = 9 * (int)res;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - width / 2,
                                         GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - height / 2);
            graphics.ApplyChanges();

            if (!ServiceLocator.Instance.IsInitialized)
                ServiceLocator.Instance.Initialize(this.Content,
                                                    this.GraphicsDevice,
                                                    graphics.PreferredBackBufferWidth,
                                                    graphics.PreferredBackBufferHeight);
            else ServiceLocator.Instance.Resize(res, width, height);

            SManager.UpdateResolution(ResolutionCurrent);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            CoreBatcher = new SpriteBatch(GraphicsDevice);

            Resize(ResolutionState.R1280x720);

            //SManager.UpdateGameObject(this);
            SManager.UpdateResolution(ResolutionCurrent);
            SManager.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            ServiceLocator.Instance.Unload();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape) || NeedExit)
                Exit();

            SManager.Update(gameTime);
           
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SManager.Draw(CoreBatcher, gameTime);

            base.Draw(gameTime);
        }
    }
}
