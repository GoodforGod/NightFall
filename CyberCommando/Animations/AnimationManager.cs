using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CyberCommando.Animations
{
    class AnimationManager<TEnum>
         where TEnum : struct, IConvertible
    {
        public Dictionary<TEnum, Animation> Animations { get; private set; }
        //public Texture2D Spritesheet;
        public Animation CurrentAnimation { get; set; }
        public SpriteEffects CurrentEffect { get; set; }

        public AnimationManager()
        {
            Animations = new Dictionary<TEnum, Animation>();
        }

        public void LoadAnimations(AnimationLoader loader, string spritesheetName)
        {
            Animations = loader.LoadAll<TEnum>(spritesheetName);
            if (Animations == null || Animations.Count == 0)
                throw new NullReferenceException("Animations is null or empty, check animation files for: " + spritesheetName);
        }

        public bool UpdateSingleAnim(TEnum state, GameTime gameTime)
        {
            if (!CurrentAnimation.SingleAnimFlag)
            {
                CurrentAnimation.SingleAnimStartTime = DateTime.Now;
                CurrentAnimation.SingleAnimFlag = true;
            }

            double secondsIntoAnimation =
                CurrentAnimation.TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = 
                secondsIntoAnimation % CurrentAnimation.Duration.TotalSeconds;

            CurrentAnimation.TimeIntoAnimation = TimeSpan.FromSeconds(remainder);

            if ((DateTime.Now - CurrentAnimation.SingleAnimStartTime).TotalMilliseconds 
                - CurrentAnimation.Duration.TotalMilliseconds / 100 > CurrentAnimation.Duration.TotalMilliseconds)
            {
                CurrentAnimation.SingleAnimFlag = false;
                CurrentAnimation.TimeIntoAnimation = TimeSpan.FromSeconds(0);
                return false;
            }
            else return true;
        }

        public void UpdateCycleAnim(TEnum state, GameTime gameTime)
        {
            CurrentAnimation = Animations[state];

            double secondsIntoAnimation =
                CurrentAnimation.TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = 
                secondsIntoAnimation % CurrentAnimation.Duration.TotalSeconds;

            CurrentAnimation.TimeIntoAnimation = TimeSpan.FromSeconds(remainder);
        }
    }
}
