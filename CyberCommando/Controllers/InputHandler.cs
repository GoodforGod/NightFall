using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Entities;
using CyberCommando.Controllers.Commands;
using CyberCommando.Animations;
using CyberCommando.Entities.Weapons;

namespace CyberCommando.Controllers
{
    public enum KeysState
    {
        Left = Keys.A | Keys.Left,
        Right = Keys.D | Keys.Right,
        Up = Keys.W | Keys.Up,
        Duck = Keys.S | Keys.Down,
        Fire = Keys.Space
    }

    internal class InputHandler
    {
        private CharStateHandler CharHandler = new CharStateHandler();

        public void HandleEntityInput(Character entity, GameTime gameTime)
        {
            KeyboardState currState = Keyboard.GetState();

            entity.AniState = AnimationState.IDLE;

            if(currState.IsKeyDown(Keys.A))
                CharHandler.MoveLeft(entity);

            if (currState.IsKeyDown(Keys.D))
                CharHandler.MoveRight(entity);

            if (currState.IsKeyDown(Keys.W))
                CharHandler.Jump(entity);

            if (currState.IsKeyDown(Keys.S))
                CharHandler.Duck(entity);

            if (currState.IsKeyDown(Keys.Space))
                CharHandler.Fire(entity);

            CharHandler.Gravity(entity);

            CharHandler.Idle(entity);

            CharHandler.HandlePosition(entity, gameTime);
        }

        public double HandleArmInput(Character entity)
        {
            MouseState state = Mouse.GetState();

            if (state.LeftButton == ButtonState.Pressed)
                entity.world.AddToSpawnQueue(typeof(Projectile).FullName, entity.DrawPosition, entity.ArmAngle);

            if (state.X < entity.DrawPosition.X)
                entity.Direction = SpriteEffects.FlipHorizontally;
            else entity.Direction = SpriteEffects.None;
            if (entity.Direction == SpriteEffects.None)
                return Math.Atan2(state.Y - entity.DrawPosition.Y, state.X - entity.DrawPosition.X) - Math.PI / 2;
            else return Math.Atan2(state.Y - entity.DrawPosition.Y, state.X - entity.DrawPosition.X) - Math.PI / 2;
        }
    }
}
