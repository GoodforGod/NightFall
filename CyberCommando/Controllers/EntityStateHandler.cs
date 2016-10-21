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
    class EntityStateHandler
    {
        public EntityStateHandler()
        {

        }

        public virtual void MoveRight(Entity entity)
        {
            entity.Direction = SpriteEffects.None;
            if (entity.VelocityCurrent.X < entity.VelocityLimit)
                entity.VelocityCurrent.X += entity.VelocityInc;
            entity.AniState = AnimationState.WALK;
        }

        public virtual void MoveLeft(Entity entity)
        {
            entity.Direction = SpriteEffects.FlipHorizontally;
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
            entity.AniState = AnimationState.FIRE;
        }

        public virtual void Idle(Entity entity)
        {
            if (entity.AniState == AnimationState.WALK)
                return;

            if (entity.VelocityCurrent.X > 0)
                entity.VelocityCurrent.X -= entity.VelocityInc;
            else if(entity.VelocityCurrent.X < 0)
                entity.VelocityCurrent.X += entity.VelocityInc;
        }

        public virtual void HandlePosition(Entity entity, GameTime gameTime)
        {
            entity.Position.X += entity.VelocityCurrent.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.Position.Y += entity.VelocityCurrent.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (entity.Position.Y > entity.GetGround())
                entity.Position.Y = entity.GetGround();
        }

        public virtual void Gravity(Entity entity)
        {
            if (entity.IsGrounded())
                return;
            entity.AniState = AnimationState.JUMP;
            entity.VelocityCurrent.Y += entity.world.Gravity;
        }

    }
}
