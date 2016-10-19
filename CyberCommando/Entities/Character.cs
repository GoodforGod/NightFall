using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities
{
    class Character : Entity
    {
        public Character(World world) : base(world)
        {
            SpriteSheet = world.GAME.Content.Load<Texture2D>("altar-2");
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
            handler.HandleEntityInput(this); 
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(SpriteSheet, Position, Color.White);
        }
    }
}
