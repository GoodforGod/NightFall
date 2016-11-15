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
    class LightSpot
    {
        GraphicsDevice GraphDev;

        public RenderTarget2D RenderTarget { get; private set; }

        public Rectangle    DrawSource  { get; set; }
        public Vector2      WPosition   { get; set; }
        public Vector2      DPosition   { get; set; }
        public Vector2      LAreaSize   { get; set; }
        public Color        LColor      { get; set; }

        public bool         IsOnScreen  { get; set; }

        public LightSpot(GraphicsDevice graphdev, ShadowMapSize size)
        {
            int baseSize = 2 << (int)size;
            this.LAreaSize = new Vector2(baseSize);
            this.RenderTarget = new RenderTarget2D(graphdev, baseSize, baseSize);
            this.LColor = Color.White;
            this.GraphDev = graphdev;
        }

        public LightSpot(GraphicsDevice graphdev, ShadowMapSize size, Vector2 pos) 
                                                                : this(graphdev, size)
        { this.WPosition = pos; }

        public LightSpot(GraphicsDevice graphdev, ShadowMapSize size, Vector2 pos, Color color) 
                                                                    : this (graphdev, size, pos)
        { this.LColor = color; }

        /// <summary>
        /// Calculates light spot center accourding to the world
        /// </summary>
        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - (DPosition - LAreaSize * 0.5f);
        }

        /// <summary>
        /// Setting render target for light spots
        /// </summary>
        public void BeginDrawingShadowCasters()
        {
            GraphDev.SetRenderTarget(RenderTarget);
            GraphDev.Clear(Color.Transparent);
        }

        /// <summary>
        /// Unset rendeg target
        /// </summary>
        public void EndDrawingShadowCasters()
        {
            GraphDev.SetRenderTarget(null);
        }
    }
}
