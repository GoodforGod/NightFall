using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Engine;
using CyberCommando.Entities.Enviroment;
using Microsoft.Xna.Framework.Input;

namespace CyberCommando.Services.Utils
{
    class GameScreen : Screen
    {
        World CoreWorld;
        Color ShadowColor = Color.Gray;

        bool ShadowEffect = true;
        QuadRenderComponent QuadRender;
        bool BloomEffect = true;
        BloomRenderComponent BloomRender;

        ShadowResolver ShadowRender;

        ResolutionState ResolutionCurrent { get; set; }

        int LeftLimit { get; set; }
        int RightLimit { get; set; }
        int RightLightLimit { get; set; }

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);

            this.QuadRender = new QuadRenderComponent(CoreGame);
            this.BloomRender = new BloomRenderComponent(CoreGame);

            var lvl = LevelNames.CYBERTOWN;

            if (param.Length != 0)
                if (!Enum.TryParse(param[0].ToString(), out lvl))
                    throw new ArgumentException("wtf level you load bro");
            LevelManager.Instance.Initialize();
            LevelManager.Instance.LoadLevel(lvl);

            this.CoreWorld = new World(CoreGame, GraphDev.Viewport.Height, GraphDev.Viewport.Width, GraphDev.Viewport);
            this.CoreWorld.Initialize();
        }

        public override void Resize(ResolutionState res, int width, int height)
        {
            base.Resize(res, width, height);
            CoreWorld.UpdateResolution(width, height, res);
            LeftLimit = CoreWorld.FWidth / 2;
            RightLimit = CoreWorld.LevelLimits.Width - CoreWorld.FWidth / 2;
            RightLightLimit = CoreWorld.LevelLimits.Width - CoreWorld.FWidth;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            CoreGame.Components.Add(BloomRender);
            CoreGame.Components.Add(QuadRender);

            //Resize screen
            Resize(ResolutionState.R1280x720, GraphDev.Viewport.Width, GraphDev.Viewport.Height);

            ShadowRender = new ShadowResolver(GraphDev,
                                                QuadRender,
                                                ShadowMapSize.Size256,
                                                ShadowMapSize.Size256,
                                                ShadowColor);
            ShadowRender.LoadContent(Content);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            CoreWorld.Update(gameTime);
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

            foreach (var light in CoreWorld.LevelLight)
            {
                if (light.IsOnScreen)
                    batcher.Draw(light.RenderTarget,
                                 light.DPosition - light.LAreaSize * 0.5f,
                                 light.LColor);
            }

            batcher.End();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResolveLightShadowCasts(SpriteBatch batcher, GameTime gameTime)
        {
            var playerLeft = CoreWorld.Player.WPosition.X - LeftLimit;

            var lightOnLevel = CoreWorld.LevelLight;

           var LightLeftLimit = new Vector2(-lightOnLevel[0].LAreaSize.X, 0);
           var LightRightLimit = new Vector2(CoreWorld.FWidth + lightOnLevel[0].LAreaSize.X, 0);

            foreach (var light in lightOnLevel)
            {
                if (CoreWorld.Player.WPosition.X < LeftLimit)
                    light.DPosition = light.WPosition;
                else if (CoreWorld.Player.WPosition.X > RightLimit)
                    light.DPosition = new Vector2(light.WPosition.X - RightLightLimit, light.WPosition.Y);
                else
                    light.DPosition = new Vector2((light.WPosition.X - playerLeft), light.WPosition.Y);

                if (light.DPosition.X > LightRightLimit.X || light.DPosition.X < LightLeftLimit.X)
                {
                    light.IsOnScreen = false;
                    continue;
                }
                else light.IsOnScreen = true;

                light.BeginDrawingShadowCasters();
                CoreWorld.DrawEntitiesShadowCasters(gameTime, batcher, light, Color.Black);
                light.EndDrawingShadowCasters();
                ShadowRender.ResolveShadows(light.RenderTarget, light.RenderTarget, light.DPosition);
            }
        }

        public override void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            // Resolve shadow map
            if (ShadowEffect)
            {
                ResolveLightShadowCasts(batcher, gameTime);
                ShadowRender.BeginDraw(GraphDev);
                DrawLights(batcher);
                ShadowRender.EndDraw(GraphDev);
            }
            // Draw Bloom Effect
            if (BloomEffect)
                BloomRender.BeginDraw();

            // Draw Level background layers
            CoreWorld.DrawLevelBackground(gameTime, batcher);

            if (BloomEffect)
            {
                BloomRender.DrawBloom();
                BloomRender.EndDraw();
                BloomRender.DisplayBloomTarget();
            }

            // Display Frontlevel layers and Entitites
            CoreWorld.DrawLevelFrontground(gameTime, batcher);
            CoreWorld.DrawLevelEntities(gameTime, batcher);

            // Display Shadow Map
            if (ShadowEffect)
                ShadowRender.DisplayShadowCast(batcher);

            //base.Draw(gameTime);

            // DrawCharacter without effects cause all effects already accured
            CoreWorld.DrawCharacter(gameTime, batcher);
        }
    }
}
