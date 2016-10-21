using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using CyberCommando.Entities;
using CyberCommando.Controllers.Commands;
using CyberCommando.Animations;

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

    public class InputHandler
    {
        private CharStateHandler CharHandler = new CharStateHandler();

        public void HandleEntityInput(Entity entity, GameTime gameTime)
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
    }
}
