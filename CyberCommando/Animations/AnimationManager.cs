using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CyberCommando.Animations
{
    public enum AnimationState
    {
        NONE,
        IDLE,
        WALK_LEFT,
        WALK_RIGHT,
        JUMP,
        DUCK,
        FIRE
    }

    class AnimationManager
    {
        public Dictionary<AnimationState, Animation> Animations;
        public Texture2D Spritesheet;
        // do animation load and play

        public Animation CurrentAnimation { get; set; }
        public SpriteEffects CurrentEffect { get; set; }

        public AnimationManager() { }

        public void AddAnimation(Animation animation, AnimationState state)
        {
            Animations.Add(state, animation);
        }

        public bool UpdateSingleAnimationIsEnded(Animation animation, GameTime gameTime)
        {
            if (animation.SingleAnimationCounter == 0)
            {
                animation.SingleAnimationStartTime = DateTime.Now;
                animation.SingleAnimationCounter = 1;
            }
            double secondsIntoAnimation =
                animation.TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = secondsIntoAnimation % animation.Duration.TotalSeconds;

            animation.TimeIntoAnimation = TimeSpan.FromSeconds(remainder);

            if ((DateTime.Now - animation.SingleAnimationStartTime).TotalMilliseconds - animation.Duration.TotalMilliseconds / 100 > animation.Duration.TotalMilliseconds)
            {
                animation.SingleAnimationCounter = 0;
                animation.TimeIntoAnimation = TimeSpan.FromSeconds(0);
                return false;
            }
            else return true;
        }

        public void UpdateCycleAnimation(Animation animation, GameTime gameTime)
        {
            double secondsIntoAnimation =
                animation.TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = secondsIntoAnimation % animation.Duration.TotalSeconds;

            animation.TimeIntoAnimation = TimeSpan.FromSeconds(remainder);
        }
    }
}
