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
    public enum InputState
    {
        Left = Keys.A | Keys.Left,
        Right = Keys.D | Keys.Right,
        Up = Keys.W | Keys.Up,
        Descend = Keys.S | Keys.Down,
        Fire = Keys.Space
    }

    class InputHandler
    {
        IList<Command> HandlerInput(Entity entity)
        {
            var state = new KeyboardState();
            IList<Command> cmds = new List<Command>();

            if (IsPressed(state, InputState.Left))
                cmds.Add(new MoveLeftCommand(entity,0,0));
            if (IsPressed(state, InputState.Left))
                cmds.Add(new MoveLeftCommand(entity,0,0));
            if (IsPressed(state, InputState.Left))
                cmds.Add(new MoveLeftCommand(entity,0,0));
            if (IsPressed(state, InputState.Left))
                cmds.Add(new MoveLeftCommand(entity,0,0));
            

            return cmds;
        }

        bool IsPressed(KeyboardState state, InputState inputState)
        {
            Keys keys = (Keys)inputState;

            return false;
        }

        bool IsPressed(MouseState state)
        {


            return false;
        }
    }
}
