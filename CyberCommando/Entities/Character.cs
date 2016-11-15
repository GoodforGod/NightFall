﻿using System;
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
        const int       SHeight = 90;
        const int       SWidth = 62;

        public int      RLimit { get; set; }
        public int      LLimit { get; set; }

        public float    ArmAngle { get; private set; }

        Vector2 ArmLOffeset;
        Vector2 ArmROffeset;
        Vector2 ArmWPosition;

        AnimationManager<AnimationState> AniManager;

        public Character(World world) : base(world)
        {
            DPosition = new Vector2(1,1);
            AniManager = new AnimationManager<AnimationState>();
            AniManager.LoadAnimations(ServiceLocator.Instance.LManager, ServiceLocator.Instance.PLManager.NSPlayer);
            Sprite = ServiceLocator.Instance.PLManager.SPlayer;
            AniState = AnimationState.IDLE;
            EntState = EntityState.ACTIVE;
            AniManager.CurrentAnimation = AniManager.Animations[AnimationState.IDLE];
            SOffset = new Vector2(62 * 4, 90);
            BoundingBox = new Rectangle((int)WPosition.X, (int)WPosition.Y, SHeight, SWidth);
            IsGrounded = false;
        }

        public override bool IsOnScreen()
        {
            if (WPosition.X >= WCore.LevelLimits.X && WPosition.X + SOffset.X <= WCore.LevelLimits.Width)
                return true;
            else return false;
        }

        public override bool CheckIfIsGrounded()
        {
            if (WPosition.Y + SHeight >= WCore.LevelLimits.Height - WCore.WorldOffset)
                return true;
            else
                return false;
        }

        public override int GetGround()
        {
            return WCore.LevelLimits.Height - WCore.WorldOffset - SHeight;
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
            RLimit = WCore.LevelLimits.Width - WCore.FWidth / 2;
            LLimit = WCore.FWidth / 2;

            if (WPosition.X > LLimit && WPosition.X < RLimit)
                DPosition.X = LLimit;
            else if (WPosition.X > RLimit)
                DPosition.X = LLimit + (WPosition.X - RLimit);
            else DPosition.X = WPosition.X;

            DPosition.Y = WPosition.Y;

            ArmAngle = (float)Handler.HandleMouseInput(this);
            if (Direction == SpriteEffects.None)
                ArmWPosition = WPosition + ArmROffeset;
            else ArmWPosition = WPosition + ArmLOffeset;
        }

        public override void Update(GameTime gameTime)
        {
            AniManager.UpdateCycleAnim(AniState, gameTime);

            Handler.HandleKeyboardInput(this, gameTime);

            BoundingBox = new Rectangle((int)WPosition.X, (int)WPosition.Y, SWidth, SHeight);

            CorrectDrawPosition();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Draw(Sprite,
                           WPosition,
                           AniManager.CurrentAnimation.CurrentRectangle,
                           Color.White,
                           Angle,
                           new Vector2(1, 1),
                           ResScale,
                           Direction,
                           .0f);

            if (AniManager.CurrentAnimation != AniManager.Animations[AnimationState.JUMP])
                batcher.Draw(Sprite,
                                ArmWPosition,
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
            batcher.Draw(Sprite,
                            lightArea.ToRelativePosition(WPosition), 
                            AniManager.CurrentAnimation.CurrentRectangle,
                            color, 
                            Angle, 
                            new Vector2(1,1), 
                            ResScale, 
                            Direction, 
                            .0f);

            if (AniManager.CurrentAnimation != AniManager.Animations[AnimationState.JUMP])
                batcher.Draw(Sprite,
                                lightArea.ToRelativePosition(ArmWPosition),
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
