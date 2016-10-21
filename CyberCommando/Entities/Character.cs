using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;

namespace CyberCommando.Entities
{
    class Character : Entity
    {
        private const string SpriteSheetName = "c-test";

        public const int SpriteHeight = 90;
        public const int SpriteWidth = 62;

        private AnimationManager AniManager;

        public Character(World world) : base(world)
        {
            AniManager = new AnimationManager(world.AniLoader ,SpriteSheetName);
            SpriteSheet = world.Game.Content.Load<Texture2D>(SpriteSheetName);
            AniState = AnimationState.IDLE;
            EntState = EntityState.ACTIVE;
            AniManager.CurrentAnimation = AniManager.Animations[AnimationState.IDLE];
        }

        public override bool IsOnScreen()
        {
            return false;
        }

        public override bool IsGrounded()
        {
            if (Position.Y + SpriteHeight >= world.FrameHeight - world.WorldOffset)
                return true;
            else
                return false;
        }

        public override int GetGround()
        {
            return world.FrameHeight - world.WorldOffset - SpriteHeight;
        }

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
            AniManager.UpdateCycleAnim(AniState, gameTime);
            handler.HandleEntityInput(this, gameTime); 
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(SpriteSheet, 
                            Position, 
                            AniManager.CurrentAnimation.CurrentRectangle, 
                            Color.White, 
                            Angle, 
                            new Vector2(1,1), 
                            Scale, 
                            Direction, 
                            .0f);
        }
    }
}
