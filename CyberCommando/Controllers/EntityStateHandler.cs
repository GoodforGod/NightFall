using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities;
using CyberCommando.Animations;

namespace CyberCommando.Controllers
{
    /// <summary>
    /// Entity level manager, handles all Entity state changing methods
    /// </summary>
    class EntityStateHandler
    {
        public virtual void MoveRight(Entity entity)
        {
            //entity.Direction = SpriteEffects.None;
            if (entity.CVelocity.X < entity.LVelocity)
                entity.CVelocity.X += entity.IVelocity;
            entity.AniState = AnimationState.WALK;
        }

        public virtual void MoveLeft(Entity entity)
        {
            //entity.Direction = SpriteEffects.FlipHorizontally;
            if (entity.CVelocity.X > -entity.LVelocity)
                entity.CVelocity.X -= entity.IVelocity;
            entity.AniState = AnimationState.WALK;
        }

        public virtual void Jump(Entity entity)
        {
            entity.CVelocity.Y = -entity.IVelocity * 20; 
            entity.AniState = AnimationState.JUMP;
        }

        public virtual void Duck(Entity entity)
        {
            entity.AniState = AnimationState.DUCK;
        }

        public virtual void Fire(Entity entity)
        {
            //entity.AniState = AnimationState.FIRE;
        }

        /// <summary>
        /// Slows entity if it isn't in the WALK state
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Idle(Entity entity)
        {
            if (entity.AniState == AnimationState.WALK)
                return;

            if (entity.CVelocity.X > 0)
                entity.CVelocity.X -= entity.IVelocity;
            else if(entity.CVelocity.X < 0)
                entity.CVelocity.X += entity.IVelocity;
        }

        /// <summary>
        /// Changes entity position <see cref="Entity.WPosition"/> via velocity <see cref="Entity.CVelocity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="gameTime"></param>
        public virtual void HandlePosition(Entity entity, GameTime gameTime)
        {
            entity.WPosition.X += entity.CVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.WPosition.Y += entity.CVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if we are not falling under the world limit on axis Y
            // Double check cause we could fall
            if (entity.WPosition.Y > entity.GetGround())
                entity.WPosition.Y = entity.GetGround();

            if (!entity.IsOnScreen())
            {
                if (entity.WPosition.X < 0)
                    entity.WPosition.X = 0;
                else entity.WPosition.X = entity.CoreWorld.LevelLimits.Width - entity.SpriteOffset.X;
            }
        }

        /// <summary>
        /// Use gravity force if not on the ground
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Gravity(Entity entity)
        {
            if (entity.IsGrounded())
                return;
            entity.AniState = AnimationState.JUMP;
            entity.CVelocity.Y += entity.CoreWorld.Gravity;
        }
    }
}
