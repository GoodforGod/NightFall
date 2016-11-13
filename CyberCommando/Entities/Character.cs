using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Animations;
using CyberCommando.Services;
using CyberCommando.Entities.Enviroment;
using CyberCommando.Engine;

namespace CyberCommando.Entities
{
    class Character : Entity
    {
        private const string GunSpriteName = "gun-sprite";

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
            DPosition = new Vector2(1,1);
            AniManager = new AnimationManager<AnimationState>();
            AniManager.LoadAnimations(ServiceLocator.Instance.LManager, ServiceLocator.Instance.PLManager.NPlayer);
            SpriteSheet = ServiceLocator.Instance.PLManager.SPlayer;
            AniState = AnimationState.IDLE;
            EntState = EntityState.ACTIVE;
            AniManager.CurrentAnimation = AniManager.Animations[AnimationState.IDLE];
            SpriteOffset = new Vector2(62 * 4, 90);
            // create correct gun!
        }

        public override bool IsOnScreen()
        {
            if (WPosition.X >= CoreWorld.LevelLimits.X && WPosition.X + SpriteOffset.X <= CoreWorld.LevelLimits.Width)
                return true;
            else return false;
        }

        public override bool IsGrounded()
        {
            if (WPosition.Y + SpriteHeight >= CoreWorld.LevelLimits.Height - CoreWorld.WorldOffset)
                return true;
            else
                return false;
        }

        public override int GetGround()
        {
            return CoreWorld.LevelLimits.Height - CoreWorld.WorldOffset - SpriteHeight;
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
            RightLimit = CoreWorld.LevelLimits.Width - CoreWorld.FWidth / 2;
            LeftLimit = CoreWorld.FWidth / 2;

            if (WPosition.X > LeftLimit && WPosition.X < RightLimit)
                DPosition.X = LeftLimit;
            else if (WPosition.X > RightLimit)
                DPosition.X = LeftLimit + (WPosition.X - RightLimit);
            else DPosition.X = WPosition.X;

            DPosition.Y = WPosition.Y;

            ArmAngle = (float)Handler.HandleMouseInput(this);
            if (Direction == SpriteEffects.None)
                ArmPosition = WPosition + ArmOffesetRight;
            else ArmPosition = WPosition + ArmOffesetLeft;
        }

        public override void Update(GameTime gameTime)
        {
            AniManager.UpdateCycleAnim(AniState, gameTime);

            Handler.HandleKeyboardInput(this, gameTime);

            CorrectDrawPosition();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(SpriteSheet,
                           WPosition,
                           AniManager.CurrentAnimation.CurrentRectangle,
                           Color.White,
                           Angle,
                           new Vector2(1, 1),
                           ResScale,
                           Direction,
                           .0f);

            if (AniManager.CurrentAnimation != AniManager.Animations[AnimationState.JUMP])
                batcher.Draw(SpriteSheet,
                                ArmPosition,
                                AniManager.Animations[AnimationState.FIRE].CurrentRectangle,
                                Color.White,
                                ArmAngle,
                                new Vector2(1, 1),
                                ResScale,
                                Direction,
                                .0f);
        }

        public override void DrawRelativelyToLightSpot(GameTime gameTime, SpriteBatch batcher, LightSpot lightArea, Color color)
        {
            /*
            gun.Draw(gameTime, batcher);
            */

            batcher.Draw(SpriteSheet,
                            lightArea.ToRelativePosition(WPosition), 
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
