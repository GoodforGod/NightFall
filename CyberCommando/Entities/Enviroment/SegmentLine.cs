using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities.Enviroment
{
    public class SegmentLine
    {
        public Vector2 SPoint { get; set; }
        public Vector2 EPoint { get; set; }
        public float Thickness { get; set; }

        public bool IsRendered { get; private set; }

        private RenderTarget2D SRenderTarget;
        public Texture2D SRender { get; private set; }

        Vector2 tan;
        float angle;

        const float ImageThickness = 8;
        float thicknessScale;

        Vector2 capOrigin;
        Vector2 middleOrigin;
        Vector2 middleScale;

        Texture2D Sprite;
        Rectangle SEnding;
        Rectangle SMiddle;

        public SegmentLine() { }
        public SegmentLine(Vector2 spoint, Vector2 epoint, Texture2D sprite, float thickness = 1)
        {
            this.Sprite = sprite;
            this.SPoint = spoint;
            this.EPoint = epoint;
            this.Thickness = thickness;
            SEnding = new Rectangle(1159, 62, 64, 128);
            SMiddle = new Rectangle(1157, 62, 1, 128);

            tan = EPoint - SPoint;
            angle = (float)Math.Atan2(tan.Y, tan.X);

            thicknessScale = Thickness / ImageThickness;

            capOrigin = new Vector2(SEnding.Width, SEnding.Height / 2f);
            middleOrigin = new Vector2(0, SMiddle.Height / 2f);
            middleScale = new Vector2(tan.Length(), thicknessScale);
        }

        public void FillRender(SpriteBatch batcher, Color tint, GraphicsDevice graphdev)
        {
            PresentationParameters pp = graphdev.PresentationParameters;
            SRenderTarget = new RenderTarget2D(graphdev,
                pp.BackBufferWidth,
                pp.BackBufferHeight,
                true,
                graphdev.DisplayMode.Format,
                DepthFormat.Depth24);
            Services.ServiceLocator.Instance.GraphDev.SetRenderTarget(SRenderTarget);

            Draw(batcher, tint);

            SRender = (Texture2D)SRenderTarget;
            Services.ServiceLocator.Instance.GraphDev.SetRenderTarget(null);
            IsRendered = true;
        }

        public void DrawRender(SpriteBatch batcher, Color tint)
        {
            batcher.Draw(SRender, SPoint, tint);
        }

        public void Draw(SpriteBatch batcher, Color tint)
        {
            batcher.Draw(Sprite, SPoint, SMiddle, tint, angle, middleOrigin, middleScale, SpriteEffects.None, 0f);
            batcher.Draw(Sprite, SPoint, SEnding, tint, angle, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            batcher.Draw(Sprite, EPoint, SEnding, tint, angle + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }
    }
}
