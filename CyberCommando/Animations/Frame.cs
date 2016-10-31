using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CyberCommando.Animations
{
    /// <summary>
    /// Represents frame in animations <see cref="Animation"/> 
    /// Stores rectangle of the texture/sprite and frame duration in animation
    /// </summary>
    class Frame
    {
        public Rectangle Rectangle { get; set; }
        
        public TimeSpan Duration { get; set; } 
    }
}
