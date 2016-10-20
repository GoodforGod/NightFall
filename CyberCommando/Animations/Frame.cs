using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CyberCommando.Animations
{
    class Frame
    {
        public Frame()
        {

        }

        public Rectangle Rectangle { get; set; }
        
        public TimeSpan Duration { get; set; } 
    }
}
