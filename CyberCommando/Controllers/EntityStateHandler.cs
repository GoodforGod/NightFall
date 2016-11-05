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
            if (entity.VelocityCurrent.X < entity.VelocityLimit)
                entity.VelocityCurrent.X += entity.VelocityInc;
            entity.AniState = AnimationState.WALK;
        }

        public virtual void MoveLeft(Entity entity)
        {
            //entity.Direction = SpriteEffects.FlipHorizontally;
            if (entity.VelocityCurrent.X > -entity.VelocityLimit)
                entity.VelocityCurrent.X -= entity.VelocityInc;
            entity.AniState = AnimationState.WALK;
        }

        public virtual void Jump(Entity entity)
        {
            entity.VelocityCurrent.Y = -entity.VelocityInc * 20; 
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

            if (entity.VelocityCurrent.X > 0)
                entity.VelocityCurrent.X -= entity.VelocityInc;
            else if(entity.VelocityCurrent.X < 0)
                entity.VelocityCurrent.X += entity.VelocityInc;
        }

        /// <summary>
        /// Changes entity position <see cref="Entity.WorldPosition"/> via velocity <see cref="Entity.VelocityCurrent"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="gameTime"></param>
        public virtual void HandlePosition(Entity entity, GameTime gameTime)
        {
            entity.WorldPosition.X += entity.VelocityCurrent.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.WorldPosition.Y += entity.VelocityCurrent.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if we are not falling under the world limit on axis Y
            // Double check cause we could fall
            if (entity.WorldPosition.Y > entity.GetGround())
                entity.WorldPosition.Y = entity.GetGround();
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
            entity.VelocityCurrent.Y += entity.world.Gravity;
        }
    }
}
