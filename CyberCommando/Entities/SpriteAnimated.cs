using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;
using CyberCommando.Entities.Weapons;

namespace CyberCommando.Entities
{
    class SpriteAnimated<TEnum> : SpriteTextured
        where TEnum : struct, IConvertible
    {
        public AnimationManager<TEnum> AniManager { get; }

        public SpriteAnimated(Rectangle source) 
            : base (source) { AniManager = new AnimationManager<TEnum>(); }

        public SpriteAnimated(Rectangle source, Vector2 position) 
            : base (source, position) { AniManager = new AnimationManager<TEnum>(); }
        
        public SpriteAnimated( Rectangle source, Vector2 position, float angle) 
            : base (source, position, angle) { AniManager = new AnimationManager<TEnum>(); }
        
        public SpriteAnimated(Rectangle source, Vector2 position, Texture2D texture) 
            : base (source, position, texture) { AniManager = new AnimationManager<TEnum>(); }
        
        public SpriteAnimated(Rectangle source, Vector2 position, float angle, Texture2D texture) 
            : base (source, position, angle, texture) { AniManager = new AnimationManager<TEnum>(); }
        
        public void LoadAnimations(AnimationLoader loader, string spriteSheetName)
        {
            AniManager.LoadAnimations(loader, spriteSheetName);
        }
    }
}
