using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Controllers.Commands;

namespace CyberCommando.Controllers
{
    [Flags]
    public enum CommandState
    {
        Left = 0x00,
        Right = 0x01,
        Up = 0x02,
        Descend = 0x03,
        Fire = 0x04
    }

    class InputHandler
    {

        void HandlerInput()
        { 

        }

        void IsPressed()
        { 
            KeyboardState state = new KeyboardState();


        }
    }
}
