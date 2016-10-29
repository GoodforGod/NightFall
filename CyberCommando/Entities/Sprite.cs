using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CyberCommando.Entities
{
    class Sprite
    {
        public Rectangle Source { get; set; }
        public Vector2 Position { get; set; }

        public Sprite()
        {
            this.Source = new Rectangle(1, 1, 1, 1);
            this.Position = Vector2.One;
        }

        public Sprite(Rectangle source) : this ()
        {
            this.Source = source;
        }

        public Sprite(Rectangle source, Vector2 position) : this (source)
        {
            this.Position = position;
        }
    }
}
