using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities.Utils
{
    class TMSprite : TSprite, IMovableInterface
    {
        public Vector2  _IVector    { get; set; }
        public Vector2  IVector     { get; set; }
        public Vector2  _WPosition  { get; set; }
        public Vector2  WPosition   { get; set; }

        public float    _Velocity   { get; set; }
        public float    Velocity    { get; set; }

        public TMSprite(Rectangle source, Vector2 position, Texture2D texture)
            : base(source, position, texture) { }

        public TMSprite(Rectangle source, Vector2 position, Texture2D texture, float scale)
            : base(source, position, texture, scale) { }

        public TMSprite(Rectangle source, Vector2 position, float angle, Texture2D texture)
            : base(source, position, angle, texture) { }

        public TMSprite(Rectangle source, Vector2 position, float angle, Texture2D texture, float scale)
          : base(source, position, angle, texture, scale) { }

        public virtual void Initialize(Vector2 wposition, Vector2 ivector, float velocity)
        {

        }

        public virtual void Reset()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
