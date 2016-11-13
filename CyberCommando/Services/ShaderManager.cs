using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Engine;
using CyberCommando.Entities;

namespace CyberCommando.Services
{
    class ShaderManager
    {
        private GraphicsDevice GraphDev;

        private static ShaderManager _Instance;
        public static ShaderManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ShaderManager();
                return _Instance;
            }
        }

        private ShaderManager() { }

        public void Initialize(GraphicsDevice graphdev)
        {
            this.GraphDev = graphdev;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawLights(SpriteBatch batcher, List<LightSpot> lights)
        {
            batcher.Begin(SpriteSortMode.Deferred,
                            BlendState.Additive,
                            null, null, null, null, null);
            // world.Services.Camera.GetViewMatrix(new Vector2(0.9f))

            foreach (var light in lights)
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
        /*
        public void ResolveLightShadowCasts(SpriteBatch batcher, GameTime gameTime, List<Entity> entities)
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

        public void ResolveShadows()
        {
            ResolveLightShadowCasts(batcher, gameTime);
            ShadowRender.BeginDraw(GraphDev);
            DrawLights(batcher);
            ShadowRender.EndDraw(GraphDev);
        }
        */

    }
}
