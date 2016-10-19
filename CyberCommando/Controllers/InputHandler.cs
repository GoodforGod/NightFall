using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities;
using CyberCommando.Controllers.Commands;

namespace CyberCommando.Controllers
{
    [Flags]
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
        CharStateHandler CharHandler = new CharStateHandler();

        EnemyStateHandler EnemyHandler = new EnemyStateHandler();

        public void HandleEntityInput(Entity entity)
        {
            KeyboardState currState = Keyboard.GetState();

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
        } 

        /*
        void ExecuteUserCommands(IList<Command> cmds)
        {

        }
        */

        /*
        IList<Command> HandlerInput(Entity entity)
        {
            var currState = new KeyboardState();
            IList<Command> cmds = new List<Command>();

            if (IsPressed(currState, KeysState.Left))
                cmds.Add(new MoveLeftCommand(entity,0,0));

            if (IsPressed(currState, KeysState.Right))
                cmds.Add(new MoveRightCommand(entity,0,0));

            if (IsPressed(currState, KeysState.Up))
                cmds.Add(new JumpCommand(entity,0,0));

            if (IsPressed(currState, KeysState.Duck))
                cmds.Add(new DuckCommand(entity,0,0));

            if (IsPressed(currState, KeysState.Fire))
                cmds.Add(new FireCommand(entity));

//            if(cmds.Count == 0)
//                cmds.Add

            return cmds;
        }
        */

        bool IsPressed(KeyboardState state, KeysState key)
        {
            return state.IsKeyDown((Keys)key);
        }

        bool IsPressed(MouseState state)
        {


            return false;
        }
    }
}
