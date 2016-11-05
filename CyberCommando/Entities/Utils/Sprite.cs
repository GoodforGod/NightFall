using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CyberCommando.Entities.Utils
{
    /// <summary>
    /// Simple sprite, representing texture/sprite rectangle and it possition in the world
    /// </summary>
    class Sprite
    {
        public Rectangle Source { get; set; }
        public Vector2 Position { get; set; }
        public float Scale { get; set; }

        public Sprite()
        {
            this.Source = new Rectangle(1, 1, 1, 1);
            this.Position = Vector2.One;
            this.Scale = 1.0f;
        }

        public Sprite(Rectangle source) : this ()
        {
            this.Source = source;
        }

        public Sprite(Rectangle source, Vector2 position) : this (source)
        {
            this.Position = position;
        }

        public Sprite(Rectangle source, Vector2 position, float scale) : this(source, position)
        {
            this.Scale = scale;
        }
    }
}
