using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities;
using CyberCommando.Engine;
using System.Collections.Generic;

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

        Vector2 LightPos;

        Vector2 LightLeftLimit;
        Vector2 LightRightLimit;

        QuadRenderComponent QuadRender;
        Shadows ShadowMapRes;
        List<Light> LevelLight = new List<Light>();
        RenderTarget2D screenShadow;

        const int WIDTH = 1366;
        const int HEIGHT = 768;
        const int LIGHT_COUNT = 9;

        public GameCore()
        {
            // ~~~ UNCOMMENT For fullscreen mode~~~
            //graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - WIDTH / 2,
                                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - HEIGHT / 2);

            QuadRender = new QuadRenderComponent(this);
            this.Components.Add(QuadRender);
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

            ShadowMapRes = new Shadows(GraphicsDevice, QuadRender, ShadowMapSize.Size256, ShadowMapSize.Size1024);
            ShadowMapRes.LoadContent(Content);

            for (int i = 0; i < LIGHT_COUNT; i++)
                LevelLight.Add(new Light(GraphicsDevice, ShadowMapSize.Size512));

            LightPos = new Vector2(150, 580);

            var rand = new System.Random();

            foreach(var light in LevelLight)
            {
                light.WorldPosition = LightPos;
                LightPos.X += 420;

                switch (rand.Next(0, 9))
                {
                    case 0: light.LightColor = Color.Tomato; break;
                    case 1: light.LightColor = Color.LightPink; break;
                    case 2: light.LightColor = Color.LightGreen; break;
                    case 3: light.LightColor = Color.LightGoldenrodYellow; break;
                    case 4: light.LightColor = Color.Blue; break;
                    case 5: light.LightColor = Color.White; break;
                    case 6: light.LightColor = Color.MediumPurple; break;
                    case 7: light.LightColor = Color.SeaGreen; break;
                    case 8: light.LightColor = Color.Orange; break;
                    case 9: light.LightColor = Color.LightCyan; break;
                    default: light.LightColor = Color.LightGreen; break;
                }
            }

            LightLeftLimit = new Vector2(-LevelLight[0].LightAreaSize.X * 2, 0);
            LightRightLimit = new Vector2(world.FrameWidth + LevelLight[0].LightAreaSize.X * 2, 0);

            screenShadow = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

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

            foreach (var light in LevelLight)
            {
                if (world.Player.WorldPosition.X < world.FrameWidth / 2
                    || world.Player.WorldPosition.X < world.Services.Camera.Limits.X - world.FrameWidth / 2)
                    light.DrawPosition = light.WorldPosition;
                else
                //if (world.Player.WorldPosition.X > world.FrameWidth / 2 
                //    || world.Player.WorldPosition.X > world.Services.Camera.Limits.X - world.FrameWidth/2)
                    light.DrawPosition = new Vector2((light.WorldPosition.X
                        - (world.Player.WorldPosition.X - world.FrameWidth / 2)),
                                                   light.WorldPosition.Y);

                //else light.DrawPosition = light.WorldPosition;

                if (light.DrawPosition.X > LightRightLimit.X || light.DrawPosition.X < LightLeftLimit.X)
                {
                    light.IsOnScreen = false;
                    continue;
                }
                else light.IsOnScreen = true;

                light.BeginDrawingShadowCasters();
                world.DrawEntitiesShadowCasters(gameTime, batcher, light, Color.Gray);
                light.EndDrawingShadowCasters();
                ShadowMapRes.ResolveShadows(light.RenderTarget, light.RenderTarget, light.DrawPosition);
            }

            GraphicsDevice.SetRenderTarget(screenShadow);
            GraphicsDevice.Clear(Color.Gray);

            batcher.Begin(SpriteSortMode.Deferred,
                            BlendState.Additive,
                            null, null, null, null, null);
            // world.Services.Camera.GetViewMatrix(new Vector2(0.9f))

            foreach (var light in LevelLight)
            {
                if (light.IsOnScreen)
                    batcher.Draw(light.RenderTarget,
                                 light.DrawPosition - light.LightAreaSize * 0.5f,
                                 light.LightColor);
            }
       
            batcher.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            // Level draw
            world.DrawLevel(gameTime, batcher);

            BlendState blendState = new BlendState();
            blendState.ColorSourceBlend = Blend.DestinationColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;

            batcher.Begin(SpriteSortMode.Immediate, blendState);
            batcher.Draw(screenShadow, Vector2.Zero, Color.White);
            batcher.End();

            // Draw all entities
            world.DrawEntities(gameTime, batcher);

            base.Draw(gameTime);
        }
    }
}
