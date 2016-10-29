using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Controllers;
using CyberCommando.Animations;

namespace CyberCommando.Entities
{
    public enum EntityState
    {
        NONE,
        ACTIVE,
        PASSIVE,
        DYING,
        DEAD,
        INVISIBLE
    }

    public class Entity : IEntity
    {
        public World world { get; }

        internal InputHandler handler { get; set; } 

        protected Texture2D SpriteSheet;

        public Vector2 DrawPosition;
        public Vector2 WorldPosition;
        public Vector2 VelocityCurrent;
        public virtual float VelocityLimit { get { return 290f; } }
        public virtual float VelocityInc { get { return 15f; } }

        public SpriteEffects Direction { get; set; }
        public float Angle { get; set; }
        public float Scale { get; set; }
        public virtual int Health { get; set; }

        public AnimationState AniState { get; set; }
        public EntityState EntState { get; set; }

        public BoundingBox boundingBox { get; set; }

        public Entity(World world)
        {
            this.world = world;
            Direction = SpriteEffects.None;
            Angle = 0f;
            Scale = 1f;
            EntState = EntityState.PASSIVE;
        }

        public virtual Entity Clone() { return this; } 

        public virtual bool IsOnScreen() { return false; }

        public virtual bool IsGrounded() { return false; }

        public virtual int GetGround() { return world.WorldOffset; }

        public virtual void CorrectDrawPosition() { }

        public virtual void Damage(Entity attacker, int damage) { }

        public virtual void Kill(Entity killer) { world.Kill(this); }

        public virtual void Touch(Entity other) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime, SpriteBatch batcher) { }
    }

    interface IEntity
    {
        /// <param name="attacker">attack entity</param>
        /// <param name="damage">damage amount</param>
        void Damage(Entity attacker, int damage);

        /// <param name="attacker">attack entity</param>
        /// <param name="damage">damage amount</param>
        void Touch(Entity other);

        /// <param name="killer">entity killer</param>
        void Kill(Entity killer);

        /// <param name="gameTime">time ellapsed</param>
        void Update(GameTime gameTime);

        /// <param name="gameTime">time ellapsed</param>
        /// <param name="spriteBatch">batcher</param>
        void Draw(GameTime gameTime, SpriteBatch batcher);
    }
}
