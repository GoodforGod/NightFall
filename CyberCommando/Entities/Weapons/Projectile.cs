using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CyberCommando.Entities.Enviroment;

namespace CyberCommando.Entities.Weapons
{
    /// <summary>
    /// Represents projectile/bullet entity in the game
    /// </summary>
    class Projectile : Entity
    {
        private Rectangle Source { get; set; }
        public GunState State { get; set; }

        public Projectile(World world, Vector2 position, GunState state, Texture2D texture, Rectangle source) 
            : base(world)
        {
            this.WorldPosition = position;
            this.State = state;
            this.SpriteSheet = texture;
            this.Source = source;
            this.VelocityCurrent = new Vector2(5f, 5f);
            Scale = ResScale = 0.3f;
        }

        public override Entity Clone()
        {
            return new Projectile(world, WorldPosition, State, SpriteSheet, Source);
        }

        public override bool IsOnScreen()
        {
            if (WorldPosition.X < world.LevelLimits.Width 
                    && WorldPosition.X > world.LevelLimits.X - 200 
                    && WorldPosition.Y < world.LevelLimits.Height 
                    && WorldPosition.Y > world.LevelLimits.Y - 200)
                return true;
            else
                return false;
        }

        public override bool IsGrounded() { return false; }

        public override void Damage(Entity attacker, int damage) { }

        public override void Kill(Entity killer) { base.Kill(this); }

        public override void Touch(Entity other) { } 

        public override void Update(GameTime gameTime)
        {
            WorldPosition += VelocityCurrent * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!this.IsOnScreen())
                this.Kill(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(SpriteSheet,
                               WorldPosition,
                               Source,
                               Color.White * 0.8f,
                               Angle,
                               new Vector2(1, 1),
                               ResScale,
                               Direction,
                               .0f);
        }
    }
}
