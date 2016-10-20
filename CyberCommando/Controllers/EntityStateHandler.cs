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

        public virtual void MoveRight(Entity entity, GameTime gameTime)
        {
            if (entity.VelocityCurrent.X < entity.VelocityLimit)
                entity.VelocityCurrent.X += entity.VelocityInc;
            entity.Position.X += entity.VelocityCurrent.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.AniState = AnimationState.WALK_RIGHT;
        }

        public virtual void MoveLeft(Entity entity, GameTime gameTime)
        {
            if (entity.VelocityCurrent.X < entity.VelocityLimit)
                entity.VelocityCurrent.X += entity.VelocityInc;
            entity.Position.X -= entity.VelocityCurrent.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.AniState = AnimationState.WALK_LEFT;
        }

        public virtual void Jump(Entity entity, GameTime gameTime)
        {
            entity.Position.Y -= (entity.VelocityCurrent.Y = entity.VelocityInc * 10) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.AniState = AnimationState.JUMP;
        }

        public virtual void Duck(Entity entity, GameTime gameTime)
        {
            entity.Position.Y += (entity.VelocityCurrent.Y = entity.VelocityInc * 10) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.AniState = AnimationState.DUCK;
        }

        public virtual void Fire(Entity entity, GameTime gameTime)
        {
            entity.AniState = AnimationState.FIRE;
        }

        public virtual void Idle(Entity entity, GameTime gameTime)
        {
            if (entity.AniState == AnimationState.WALK_LEFT || entity.AniState == AnimationState.WALK_RIGHT)
                return;
            if (entity.VelocityCurrent.X > 0)
            {
                entity.VelocityCurrent.X -= entity.VelocityInc;
            }
            else if(entity.VelocityCurrent.X < 0)
            {
                entity.VelocityCurrent.X += entity.VelocityInc;
            }
        } 

        public virtual void Gravity(Entity entity, GameTime gameTime)
        {

        }

    }
}
