using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities;

namespace CyberCommando
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameCore : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch batcher;

        World world;

        const int WIDTH = 1366;
        const int HEIGHT = 768;

        public GameCore()
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
            // ~~~ UNCOMMENT For fullscreen mode~~~
            //graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;

            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - WIDTH / 2,
                                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - HEIGHT / 2);

            base.Initialize();
        } 

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batcher = new SpriteBatch(GraphicsDevice);

            world = new World(this, HEIGHT, WIDTH, GraphicsDevice.Viewport);
            world.Initialize();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.Draw(gameTime, batcher);

            base.Draw(gameTime);
        }
    }
}
