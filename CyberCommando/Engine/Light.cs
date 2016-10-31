using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Engine
{
    /// <summary>
    /// Represent light spot in the world
    /// </summary>
    class Light
    {
        private GraphicsDevice graphicsDevice;

        public RenderTarget2D RenderTarget { get; private set; }
        public Vector2 WorldPosition { get; set; }
        public Vector2 DrawPosition { get; set; }
        public Vector2 LightAreaSize { get; set; }
        public Color LightColor { get; set; }
        public bool IsOnScreen { get; set; }

        public Light(GraphicsDevice graphicsDevice, ShadowMapSize size)
        {
            int baseSize = 2 << (int)size;
            LightAreaSize = new Vector2(baseSize);
            RenderTarget = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Calculates light spot center accourding to the world
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - (DrawPosition - LightAreaSize * 0.5f);
        }

        public void BeginDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }

        public void EndDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(null);
        }
    }
}
