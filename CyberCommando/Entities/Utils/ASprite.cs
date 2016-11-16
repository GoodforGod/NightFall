using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;
using CyberCommando.Services;
using CyberCommando.Entities.Weapons;

namespace CyberCommando.Entities.Utils
{
    /// <summary>
    /// Animated sprite, representing texture/sprite rectangle and it possition in the world, its texture/sprite and animations
    /// <paramref name="TEnum"/> assosiates this sprite with animationState like <see cref="AnimationState"/>
    /// </summary>
    class ASprite<TEnum> : TSprite
        where TEnum : struct, IConvertible
    {
        public AnimationManager<TEnum> AniManager { get; }

        public ASprite(Rectangle source, Vector2 position, Texture2D texture)
           : base(source, position, texture) { AniManager = new AnimationManager<TEnum>(); }

        public ASprite(Rectangle source, Vector2 position, float angle, Texture2D texture)
           : base(source, position, angle, texture) { AniManager = new AnimationManager<TEnum>(); }

        public ASprite(Rectangle source, Vector2 position, Texture2D texture, float scale)
           : base(source, position, texture, scale) { AniManager = new AnimationManager<TEnum>(); }

        public ASprite(Rectangle source, Vector2 position, float angle, Texture2D texture, float scale)
           : base(source, position, angle, texture, scale) { AniManager = new AnimationManager<TEnum>(); }

        public void LoadAnimations(string spriteSheetName)
        {
            AniManager.LoadAnimations(spriteSheetName);
        }

        public void Unload()
        {
            AniManager.Unload();
        }
    }
}
