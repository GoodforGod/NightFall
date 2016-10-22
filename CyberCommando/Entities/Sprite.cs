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
        public Sprite (Rectangle source) { this.Source = source; this.Position = Vector2.One; }

        public Sprite(Rectangle source, Vector2 position) { this.Source = source; this.Position = position; }

        public Rectangle Source { get; set; }

        public Vector2 Position { get; set; }
    }
}
