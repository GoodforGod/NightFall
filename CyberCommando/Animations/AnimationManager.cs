using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using CyberCommando.Services;

namespace CyberCommando.Animations
{
    /// <summary>
    /// Stores and manages all Animations for the specific entity object
    /// </summary>
    /// <typeparam name="TEnum">
    /// Represent flexible enum for animations states
    /// (each animation type could have specific Enum State for it) like <see cref="AnimationState"/>
    /// </typeparam>
    class AnimationManager<TEnum>
         where TEnum : struct, IConvertible
    {
        /// <summary>
        /// Collection where each state like <see cref="AnimationState"/> represents
        /// with the specific Animation for it <see cref="Animation"/>
        /// </summary>
        public Dictionary<TEnum, Animation> Animations { get; private set; }
        //public Texture2D Spritesheet;
        public Animation CurrentAnimation { get; set; }
        public SpriteEffects CurrentEffect { get; set; }

        public AnimationManager() { Animations = new Dictionary<TEnum, Animation>(); }

        /// <summary>
        /// Call loader to fill the Animations collection
        /// </summary>
        /// <param name="loader">
        /// Loader <see cref="AnimationLoader"/> given by <see cref="Services.ServiceLocator"/>
        /// </param>
        /// <param name="spritesheetName">
        /// Name of the texture/sprite for the animations
        /// </param>
        public void LoadAnimations(AnimationLoader loader, string spritesheetName)
        {
            Animations = loader.LoadAll<TEnum>(spritesheetName);
            if (Animations == null || Animations.Count == 0)
                throw new NullReferenceException("Animations is null or empty, check animation files for: " + spritesheetName);
        }

        /// <summary>
        /// Updates Animation that should play once and then stop
        /// </summary>
        /// <param name="state">
        /// Get the animation to update from <see cref="Animations"/> by this state (where state is the key)
        /// </param>
        /// <param name="gameTime"></param>
        /// <returns>
        /// FALSE - means animation is ended
        /// TRUE - means aimation is still playing
        /// </returns>
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

        /// <summary>
        /// Updates Animations in a cycle
        /// </summary>
        /// <param name="state">
        /// Get the animation to update from <see cref="Animations"/> by this state (where state is the key)
        /// </param>
        /// <param name="gameTime"></param>
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
