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
        }

        public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            Vector2 tan = EPoint - SPoint;
            float angle = (float)Math.Atan2(tan.Y, tan.X);

            const float ImageThickness = 8;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(SEnding.Width, SEnding.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, SMiddle.Height / 2f);
            Vector2 middleScale = new Vector2(tan.Length(), thicknessScale);

            spriteBatch.Draw(Sprite, SPoint, SMiddle, tint, angle, middleOrigin, middleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Sprite, SPoint, SEnding, tint, angle, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Sprite, EPoint, SEnding, tint, angle + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }
    }
}
