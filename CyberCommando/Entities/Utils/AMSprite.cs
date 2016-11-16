using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;

namespace CyberCommando.Entities.Utils
{
    class AMSprite<TEnum> : TMSprite
        where TEnum : struct, IConvertible
    {
        public AnimationManager<TEnum> AniManager { get; }

        public AMSprite(Rectangle source, Vector2 position, Texture2D texture)
           : base(source, position, texture) { AniManager = new AnimationManager<TEnum>(); }

        public AMSprite(Rectangle source, Vector2 position, float angle, Texture2D texture)
           : base(source, position, angle, texture) { AniManager = new AnimationManager<TEnum>(); }

        public AMSprite(Rectangle source, Vector2 position, Texture2D texture, float scale)
           : base(source, position, texture, scale) { AniManager = new AnimationManager<TEnum>(); }

        public AMSprite(Rectangle source, Vector2 position, float angle, Texture2D texture, float scale)
           : base(source, position, angle, texture, scale) { AniManager = new AnimationManager<TEnum>(); }
    }
}
