using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities.Utils
{
    /// <summary>
    /// Textured sprite, representing texture/sprite rectangle and it possition in the world, its texture/sprite
    /// </summary
    class TSprite : SSprite
    {
        public Texture2D    Texture { get; set; }
        public float        Angle   { get; set; }

        public TSprite(Rectangle source, Vector2 position, Texture2D texture)
            : base(source, position)
        {
            this.Angle = .0f;
            this.Texture = texture;
        }

        public TSprite(Rectangle source, Vector2 position, Texture2D texture, float scale)
            : base(source, position, scale)
        {
            this.Angle = .0f;
            this.Texture = texture;
        }

        public TSprite(Rectangle source, Vector2 position, float angle, Texture2D texture)
            : this(source, position, texture) { this.Angle = angle; }

        public TSprite(Rectangle source, Vector2 position, float angle, Texture2D texture, float scale)
          : this(source, position, texture, scale) { this.Angle = angle; }
    }
}
