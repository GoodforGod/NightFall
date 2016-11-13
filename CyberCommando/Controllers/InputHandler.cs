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

    /// <summary>
    /// Stores methods to handle user input
    /// </summary>
    internal class InputHandler
    {
        private static InputHandler _Instance;
        public static InputHandler Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new InputHandler();
                return _Instance;

            }
        }

        private InputHandler() { }

        private CharStateHandler CharHandler = new CharStateHandler();

        private DateTime Cooldown = DateTime.Now;

        /// <summary>
        /// Handle all keyboard input and handle Character state
        /// </summary>
        public void HandleKeyboardInput(Character entity, GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();

            entity.AniState = AnimationState.IDLE;

            if(kState.IsKeyDown(Keys.A))
                CharHandler.MoveLeft(entity);

            if (kState.IsKeyDown(Keys.D))
                CharHandler.MoveRight(entity);

            if (kState.IsKeyDown(Keys.W))
                CharHandler.Jump(entity);

            if (kState.IsKeyDown(Keys.S))
                CharHandler.Duck(entity);

            if (kState.IsKeyDown(Keys.Space))
                CharHandler.Fire(entity);

            CharHandler.Gravity(entity);

            CharHandler.Idle(entity);

            CharHandler.HandlePosition(entity, gameTime);
        }

        /// <summary>
        /// Handle mouse input, changes entity direction and projectile spawn
        /// </summary>
        /// <returns>
        /// Angle between mouse and Character, so that character arm could be drawn correctly
        /// </returns>
        public double HandleMouseInput(Character entity)
        {
            MouseState mState = Mouse.GetState();

            if (mState.LeftButton == ButtonState.Pressed && (DateTime.Now - Cooldown).TotalMilliseconds > 100)
            {
                Cooldown = DateTime.Now;
                entity.CoreWorld.AddToSpawnQueue(typeof(Projectile).FullName, entity.WPosition, entity.ArmAngle);
            }

            if (mState.X < entity.DPosition.X)
                entity.Direction = SpriteEffects.FlipHorizontally;
            else entity.Direction = SpriteEffects.None;

            return Math.Atan2(mState.Y - entity.DPosition.Y, mState.X - entity.DPosition.X) - Math.PI / 2;
        }
    }
}
