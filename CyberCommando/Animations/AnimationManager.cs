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
    class AnimationManager
    {
        public Dictionary<AnimationState, Animation> Animations;
        //public Texture2D Spritesheet;

        public Animation CurrentAnimation { get; set; }
        public SpriteEffects CurrentEffect { get; set; }

        public AnimationManager(AnimationLoader loader, string spritesheetName) { Animations = loader.LoadAll(spritesheetName); }

        public bool UpdateSingleAnim(AnimationState state, GameTime gameTime)
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

        public void UpdateCycleAnim(AnimationState state, GameTime gameTime)
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
