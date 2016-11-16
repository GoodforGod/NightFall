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
    class SSprite
    {
        public Rectangle Source     { get; set; }
        public Vector2   Position   { get; set; }
        public Vector2   OPosition  { get; private set; }

        public float    Scale   { get; set; }
        public float    OScale  { get; private set; }

        public SSprite()
        {
            this.Source = Rectangle.Empty;
            this.Position = OPosition = Vector2.One;
            this.Scale = OScale = 1.0f;
        }

        public SSprite(Rectangle source) 
                                    : this ()
        {
            this.Source = source;
        }

        public SSprite(Rectangle source, Vector2 position) 
                                                : this (source)
        {
            this.Position = OPosition = position;
        }

        public SSprite(Rectangle source, Vector2 position, float scale) 
                                                    : this(source, position)
        {
            this.Scale = OScale = scale;
        }
    }
}
