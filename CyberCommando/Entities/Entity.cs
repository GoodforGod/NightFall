using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities
{
    [Flags]
    public enum EntityState
    {
        None = 0x00,
        Active = 0x01,
        Passive = 0x02,
        Dead = 0x03,
        Invisible = 0x04
    }

    public class Entity : IEntity
    {
        readonly World WORLD;
        
        public Texture2D SpriteSheet { get; set; }

        public Vector2 Position { get; set; }

        public float Angle { get; set; }

        public int Health { get; set; }
        
        public EntityState State { get; set; }
        
        public BoundingBox boundingBox { get; set; }

        public Entity(World world) { WORLD = world; }

        public virtual void Damage(Entity attacker, int damage) { }

        public virtual void Kill(Entity killer) { WORLD.Kill(this); }

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
