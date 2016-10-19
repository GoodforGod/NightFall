using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Controllers;

namespace CyberCommando.Entities
{
    [Flags]
    public enum EntityStatus
    {
        NONE,
        ACTIVE,
        PASSIVE,
        DEAD,
        INVISIBLE4
    }

    [Flags]
    public enum AnimationState
    {
        STANDING,
        WALKING,
        JUMPING,
        DUCKING,
        ATTACKING
    }

    public class Entity : IEntity
    {
        readonly World world;

        public InputHandler handler;

        public Texture2D SpriteSheet;

        public Vector2 Position;

        public float Angle;

        public int Health;

        public EntityStatus EntState; 

        public AnimationState AnimState; 

        public BoundingBox boundingBox; 

        public Entity(World world) { this.world = world; }

        public virtual Entity Clone() { return this; } 

        public virtual bool IsOnScreen() { return false; }

        public virtual bool IsGrounded() { return false; }

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

        /// <param name="killer">Убийца сущности</param>
        void Kill(Entity killer);

        /// <param name="gameTime">time ellapsed</param>
        void Update(GameTime gameTime);

        /// <param name="gameTime">time ellapsed</param>
        /// <param name="spriteBatch">batcher</param>
        void Draw(GameTime gameTime, SpriteBatch batcher);
    }
}
