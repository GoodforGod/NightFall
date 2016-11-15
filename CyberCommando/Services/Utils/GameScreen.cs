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
        World                   WCore;
        Color                   ShadowColor = Color.Gray;

        bool                    ShadowEffect = true;
        QuadRenderComponent     SQuadRender;
        bool                    BloomEffect = true;
        BloomRenderComponent    BloomRender;

        ShadowResolver          ShadowRender;

        int LLimit      { get; set; }
        int RLimit      { get; set; }
        int RLightLimit { get; set; }

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);

            this.SQuadRender = new QuadRenderComponent(CoreGame);
            this.BloomRender = new BloomRenderComponent(CoreGame);

            var lvl = (LevelNames) param[0];

            LevelManager.Instance.Initialize();
            LevelManager.Instance.LoadLevel(lvl);

            this.WCore = new World(CoreGame, GraphDev.Viewport.Height, GraphDev.Viewport.Width, GraphDev.Viewport);
            this.WCore.Initialize();
        }

        public override void Resize(ResolutionState res, int width, int height)
        {
            base.Resize(res, width, height);
            WCore.UpdateResolution(width, height, res);
            LLimit = WCore.FWidth / 2;
            RLimit = WCore.LevelLimits.Width - WCore.FWidth / 2;
            RLightLimit = WCore.LevelLimits.Width - WCore.FWidth;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            CoreGame.Components.Add(BloomRender);
            CoreGame.Components.Add(SQuadRender);

            //Resize screen
            Resize(ResolutionState.R1280x720, GraphDev.Viewport.Width, GraphDev.Viewport.Height);

            ShadowRender = new ShadowResolver(GraphDev,
                                                SQuadRender,
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
            WCore.Update(gameTime);
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

            foreach (var light in WCore.LevelLight)
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
            var playerLeft = WCore.Player.WPosition.X - LLimit;

            var lightOnLevel = WCore.LevelLight;

           var LightLeftLimit = new Vector2(-lightOnLevel[0].LAreaSize.X, 0);
           var LightRightLimit = new Vector2(WCore.FWidth + lightOnLevel[0].LAreaSize.X, 0);

            foreach (var light in lightOnLevel)
            {
                if (WCore.Player.WPosition.X < LLimit)
                    light.DPosition = light.WPosition;
                else if (WCore.Player.WPosition.X > RLimit)
                    light.DPosition = new Vector2(light.WPosition.X - RLightLimit, light.WPosition.Y);
                else
                    light.DPosition = new Vector2((light.WPosition.X - playerLeft), light.WPosition.Y);

                if (light.DPosition.X > LightRightLimit.X || light.DPosition.X < LightLeftLimit.X)
                {
                    light.IsOnScreen = false;
                    continue;
                }
                else light.IsOnScreen = true;

                light.BeginDrawingShadowCasters();
                WCore.DrawEntitiesShadowCasters(gameTime, batcher, light, Color.Black);
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
                ShadowRender.BeginDraw();
                DrawLights(batcher);
                ShadowRender.EndDraw();
            }
            // Draw Bloom Effect
            if (BloomEffect)
                BloomRender.BeginDraw();

            // Draw Level background layers
            WCore.DrawLVLBack(gameTime, batcher);

            if (BloomEffect)
            {
                BloomRender.DrawBloom();
                BloomRender.EndDraw();
                BloomRender.DisplayBloomTarget();
            }

            // Display Frontlevel layers and Entitites
            WCore.DrawLVLFront(gameTime, batcher);
            WCore.DrawLVLEntities(gameTime, batcher);

            // Display Shadow Map
            if (ShadowEffect)
                ShadowRender.DisplayShadowCast();

            //base.Draw(gameTime);

            // DrawCharacter without effects cause all effects already accured
            WCore.DrawCharacter(gameTime, batcher);
        }
    }
}
