using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;
using CyberCommando.Entities.Enviroment;
using CyberCommando.Engine;

namespace CyberCommando.Entities
{
    class Character : Entity
    {
        private const string PlayerSpriteName = "c-test";
        private const string GunSpriteName = "gun-sprite-2";

        public const int SpriteHeight = 90;
        public const int SpriteWidth = 62;

        public int RightLimit { get; set; }
        public int LeftLimit { get; set; }

        public float ArmAngle { get; private set; }
        private Vector2 ArmOffesetLeft = new Vector2(32,13);
        private Vector2 ArmOffesetRight = new Vector2(14, 25);
        private Vector2 ArmPosition = new Vector2();
        private AnimationManager<AnimationState> AniManager;
        //private Sprite Gun;

        public Character(World world) : base(world)
        {
            DrawPosition = new Vector2(1,1);
            AniManager = new AnimationManager<AnimationState>();
            AniManager.LoadAnimations(world.Services.AniLoader, PlayerSpriteName);
            SpriteSheet = world.Game.Content.Load<Texture2D>(PlayerSpriteName);
            AniState = AnimationState.IDLE;
            EntState = EntityState.ACTIVE;
            AniManager.CurrentAnimation = AniManager.Animations[AnimationState.IDLE];
 
            // create correct gun!
        }

        public override bool IsOnScreen()
        {
            return false;
        }

        public override bool IsGrounded()
        {
            if (WorldPosition.Y + SpriteHeight >= world.LevelLimits.Height - world.WorldOffset)
                return true;
            else
                return false;
        }

        public override int GetGround()
        {
            return world.LevelLimits.Height - world.WorldOffset - SpriteHeight;
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

        public override void CorrectDrawPosition()
        {
            RightLimit = world.LevelLimits.Width - world.FrameWidth / 2;
            LeftLimit = world.FrameWidth / 2;

            if (WorldPosition.X > LeftLimit && WorldPosition.X < RightLimit)
                DrawPosition.X = LeftLimit;
            else if (WorldPosition.X > RightLimit)
                DrawPosition.X = LeftLimit + (WorldPosition.X - RightLimit);
            else DrawPosition.X = WorldPosition.X;

            DrawPosition.Y = WorldPosition.Y;

            ArmAngle = (float)handler.HandleMouseInput(this);
            if (Direction == SpriteEffects.None)
                ArmPosition = WorldPosition + ArmOffesetRight;
            else ArmPosition = WorldPosition + ArmOffesetLeft;
        }

        public override void Update(GameTime gameTime)
        {
            AniManager.UpdateCycleAnim(AniState, gameTime);

            handler.HandleKeyboardInput(this, gameTime);

            CorrectDrawPosition();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(SpriteSheet,
                           WorldPosition,
                           AniManager.CurrentAnimation.CurrentRectangle,
                           Color.White,
                           Angle,
                           new Vector2(1, 1),
                           Scale,
                           Direction,
                           .0f);

            if (AniManager.CurrentAnimation != AniManager.Animations[AnimationState.JUMP])
                batcher.Draw(SpriteSheet,
                                ArmPosition,
                                AniManager.Animations[AnimationState.FIRE].CurrentRectangle,
                                Color.White,
                                ArmAngle,
                                new Vector2(1, 1),
                                Scale,
                                Direction,
                                .0f);
        }

        public override void DrawRelativelyToLightSpot(GameTime gameTime, SpriteBatch batcher, LightSpot lightArea, Color color)
        {
            /*
            gun.Draw(gameTime, batcher);
            */

            batcher.Draw(SpriteSheet,
                            lightArea.ToRelativePosition(WorldPosition), 
                            AniManager.CurrentAnimation.CurrentRectangle,
                            color, 
                            Angle, 
                            new Vector2(1,1), 
                            ResScale, 
                            Direction, 
                            .0f);

            if (AniManager.CurrentAnimation != AniManager.Animations[AnimationState.JUMP])
                batcher.Draw(SpriteSheet,
                                lightArea.ToRelativePosition(ArmPosition),
                                AniManager.Animations[AnimationState.FIRE].CurrentRectangle,
                                color,
                                ArmAngle,
                                new Vector2(1, 1),
                                ResScale,
                                Direction,
                                .0f);
        }
    }
}
