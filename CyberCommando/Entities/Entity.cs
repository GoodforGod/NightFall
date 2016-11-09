using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Controllers;
using CyberCommando.Animations;
using CyberCommando.Engine;
using CyberCommando.Entities.Enviroment;

namespace CyberCommando.Entities
{
    /// <summary>
    /// Indicating entity current game state
    /// </summary>
    public enum EntityState
    {
        NONE,
        ACTIVE,
        PASSIVE,
        DYING,
        DEAD,
        INVISIBLE
    }

    /// <summary>
    /// Represents a simple ingame entity
    /// </summary>
    class Entity : IEntity
    {
        /// <summary>
        /// World where entity is living <see cref="World"/>
        /// </summary>
        public World CoreWorld { get; }

        internal InputHandler Handler { get; set; } 

        /// <summary>
        /// Stores entity sprite
        /// </summary>
        protected Texture2D SpriteSheet;

        public Vector2 SpriteOffset { get; internal set; }

        /// <summary>
        /// Entity position to draw on screen
        /// </summary>
        public Vector2 DPosition;
        /// <summary>
        /// Entity position in the world
        /// </summary>
        public Vector2 WPosition;
        /// <summary>
        /// Current velocity vector
        /// </summary>
        public Vector2 CVelocity;
        public virtual float LVelocity { get { return 290f; } }
        public virtual float IVelocity { get { return 15f; } }

        /// <summary>
        /// Entity direction it is moving or facing
        /// </summary>
        public SpriteEffects Direction { get; set; }
        public float Angle { get; set; }
        public float Scale { get; set; }
        public float ResScale { get; set; }
        public virtual int Health { get; set; }

        public AnimationState AniState { get; set; }
        public EntityState EntState { get; set; }

        /// <summary>
        /// Entity rigbody, to check collision
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        public Entity(World world)
        {
            this.CoreWorld = world;
            Direction = SpriteEffects.None;
            Angle = 0f;
            Scale = ResScale = 1f;
            EntState = EntityState.PASSIVE;
        }
        /// <summary>
        /// Returns the same entity
        /// </summary>
        /// <returns></returns>
        public virtual Entity Clone() { return this; } 

        /// <summary>
        /// Check is the entity on screen of out of it
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOnScreen() { return false; }

        /// <summary>
        /// Check is the entity on the ground
        /// </summary>
        /// <returns></returns>
        public virtual bool IsGrounded() { return false; }

        /// <summary>
        /// Returns worlds ground position
        /// </summary>
        /// <returns></returns>
        public virtual int GetGround() { return CoreWorld.FrameHeight - CoreWorld.WorldOffset; }

        /// <summary>
        /// Calculates entity drawable position on the screen
        /// </summary>
        public virtual void CorrectDrawPosition() {  }

        public virtual void Damage(Entity attacker, int damage) { }

        public virtual void Kill(Entity killer) { CoreWorld.Kill(this); }

        public virtual void Touch(Entity other) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime, SpriteBatch batcher) { }

        /// <summary>
        /// Draw entity relatively to light spot, to draw shadow map for shader
        /// </summary>
        public virtual void DrawRelativelyToLightSpot(GameTime gameTime, 
                                                        SpriteBatch batcher, 
                                                        LightSpot lightArea,
                                                        Color color) { }
    }

    interface IEntity
    {
        /// <param name="entity">attack entity</param>
        /// <param name="damage">damage amount</param>
        void Damage(Entity entity, int damage);

        /// <param name="other">entity to collide with</param>
        void Touch(Entity other);

        /// <param name="killer">entity killer</param>
        void Kill(Entity killer);

        /// <param name="gameTime">time ellapsed</param>
        void Update(GameTime gameTime);

        /// <param name="gameTime">time ellapsed</param>
        /// <param name="batcher">batcher</param>
        void Draw(GameTime gameTime, SpriteBatch batcher);
    }
}
