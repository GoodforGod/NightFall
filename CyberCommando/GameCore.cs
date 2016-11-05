using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities.Enviroment;
using CyberCommando.Engine;

using System.Collections.Generic;

namespace CyberCommando
{
    public enum ScreenRes
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
        SpriteBatch batcher;

        World world;
        Color ShadowColor = Color.Gray;

        Vector2 LightPos;

        Vector2 LightLeftLimit;
        Vector2 LightRightLimit;

        bool ShadowEffect = false;
        QuadRenderComponent QuadRender;
        bool BloomEffect = false;
        BloomRenderComponent BloomRender;

        ShadowResolver ShadowRender;
        List<LightSpot> LevelLight = new List<LightSpot>();

        ScreenRes CurrentRes { get; set; }
        
        //int FrameWidth { get; set; }
        //int FrameHeight { get; set; }
        int LIGHT_COUNT = 9;

        int LeftLimit { get; set; }
        int RightLimit { get; set; }
        int RightLightLimit { get; set; }

        public GameCore()
        {
            // ~~~ UNCOMMENT For fullscreen mode ~~~
            //graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;
            graphics = new GraphicsDeviceManager(this);

            this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;

            QuadRender = new QuadRenderComponent(this);
            BloomRender = new BloomRenderComponent(this);

            //this.Components.Add(BloomRender);
            //this.Components.Add(QuadRender);

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
        /// 
        /// </summary>
        /// <param name="res"></param>
        protected void Resize(ScreenRes res)
        {
            CurrentRes = res;

            int width = 16 * (int)res;
            int height = 9 * (int)res;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
            Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - width / 2,
                                         GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - height / 2);
            world.UpdateResolution(width, height, res);
            BloomRender.UpdateBatcher();
            //BloomFront.UpdateBatcher();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batcher = new SpriteBatch(GraphicsDevice);

            world = new World(this, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport);
            world.Initialize();

            //Resize screen
            Resize(ScreenRes.R1280x720);

            LeftLimit = world.FrameWidth / 2;
            RightLimit = world.LevelLimits.Width - world.FrameWidth / 2;
            RightLightLimit = world.LevelLimits.Width - world.FrameWidth;

            ShadowRender = new ShadowResolver(GraphicsDevice, 
                                                QuadRender,
                                                ShadowMapSize.Size256,
                                                ShadowMapSize.Size256, 
                                                ShadowColor);
            ShadowRender.LoadContent(Content);
            SpawnLight();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SpawnLight()
        {
            for (int i = 0; i < LIGHT_COUNT; i++)
                LevelLight.Add(new LightSpot(GraphicsDevice, ShadowMapSize.Size512));

            LightPos = new Vector2(150, 580);

            var rand = new System.Random();

            foreach (var light in LevelLight)
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

            LightLeftLimit = new Vector2(-LevelLight[0].LightAreaSize.X, 0);
            LightRightLimit = new Vector2(world.FrameWidth + LevelLight[0].LightAreaSize.X, 0);
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

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawLights(SpriteBatch batcher)
        {
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
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResolveLightShadowCasts(GameTime gameTime, SpriteBatch batcher)
        {
            var playerLeft = world.Player.WorldPosition.X - LeftLimit;

            foreach (var light in LevelLight)
            {
                if (world.Player.WorldPosition.X < LeftLimit)
                    light.DrawPosition = light.WorldPosition;
                else if (world.Player.WorldPosition.X > RightLimit)
                    light.DrawPosition = new Vector2(light.WorldPosition.X - RightLightLimit, light.WorldPosition.Y);
                else
                    light.DrawPosition = new Vector2((light.WorldPosition.X - playerLeft), light.WorldPosition.Y);

                if (light.DrawPosition.X > LightRightLimit.X || light.DrawPosition.X < LightLeftLimit.X)
                {
                    light.IsOnScreen = false;
                    continue;
                }
                else light.IsOnScreen = true;

                light.BeginDrawingShadowCasters();
                world.DrawEntitiesShadowCasters(gameTime, batcher, light, Color.Black);
                light.EndDrawingShadowCasters();
                ShadowRender.ResolveShadows(light.RenderTarget, light.RenderTarget, light.DrawPosition);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Resolve shadow map
            if (ShadowEffect)
            {
                ResolveLightShadowCasts(gameTime, batcher);
                ShadowRender.BeginDraw(GraphicsDevice);
            }
            DrawLights(batcher);
            if (ShadowEffect)
                ShadowRender.EndDraw(GraphicsDevice);

            // Draw Bloom Effect
            if (BloomEffect)
                BloomRender.BeginDraw();
            world.DrawLevelBackground(gameTime, batcher);
            if (BloomEffect)
            {
                BloomRender.DrawBloom();
                BloomRender.EndDraw();
                BloomRender.DisplayBloomTarget();
            }

            // Display Frontlevel and Entitites
            world.DrawLevelFrontground(gameTime, batcher);
            world.DrawLevelEntities(gameTime, batcher);

            // Display Shadow Map
            if (ShadowEffect)
                ShadowRender.DisplayShadowCast(batcher);

            base.Draw(gameTime);
            
            // DrawCharacter without effects
            world.DrawCharacter(gameTime, batcher);
        }
    }
}
