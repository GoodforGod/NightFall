using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities.Enviroment
{
    class Platform
    {
        public Vector2      Position    { get; set; }  
        public Rectangle    BoundingBox { get; set; }
        public Texture2D    Sprite      { get; set; }
        public Rectangle    Source      { get; set; }
        public float        Scale       { get; set; }

        public Platform(Vector2 position, Texture2D sprite, Rectangle source)
        {
            this.Position = position;
            this.Sprite = sprite;
            this.Source = source;

            BoundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
        }

        public void Draw(SpriteBatch batcher)
        {
            batcher.Draw(Sprite, Position, Source, Color.White);
        }
    }
}
