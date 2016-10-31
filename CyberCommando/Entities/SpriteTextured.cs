using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities
{
    /// <summary>
    /// Textured sprite, representing texture/sprite rectangle and it possition in the world, its texture/sprite
    /// </summary
    class SpriteTextured : Sprite
    {
        public Texture2D Texture { get; set; }
        public float Angle { get; set; }

        public SpriteTextured()
            : base() { this.Angle = .0f; }

        public SpriteTextured(Rectangle source) 
            : base(source) { this.Angle = .0f; }

        public SpriteTextured(Rectangle source, Vector2 position)
            : base(source, position) { this.Angle = .0f; }

        public SpriteTextured(Rectangle source, Vector2 position, float angle)
            : this(source, position) { this.Angle = angle; }

        public SpriteTextured(Rectangle source, Vector2 position, Texture2D texture)
            : this(source, position) { this.Texture = texture; }

        public SpriteTextured(Rectangle source, Vector2 position, float angle, Texture2D texture)
            : this(source, position, angle) { this.Texture = texture; }
    }
}
