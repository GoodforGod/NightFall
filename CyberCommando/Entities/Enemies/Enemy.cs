using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CyberCommando.Entities.Enviroment;

namespace CyberCommando.Entities.Enemies
{
    /// <summary>
    /// Represents Enemy entity in the game
    /// </summary>
    class Enemy : Entity
    {
        public Enemy(World world) : base(world)
        {

        }

        public override Entity Clone()
        {
            return new Enemy(CoreWorld);
        }

        public override bool IsOnScreen() { return false; }

        public override bool IsGrounded() { return false; }

        public override void Damage(Entity attacker, int damage)
        {

        }

        public override void Kill(Entity killer)
        {
            base.Kill(this);
        }

        public override void Touch(Entity other)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {

        }
    }
}
