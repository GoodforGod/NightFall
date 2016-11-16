using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities.Utils
{
    /// <summary>
    /// 
    /// </summary>
    interface IMovableInterface
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 _IVector    { get; set; }
        Vector2 IVector     { get; set; }
        Vector2 _WPosition  { get; set; }
        Vector2 WPosition   { get; set; }

        float   _Velocity   { get; set; }
        float   Velocity    { get; set; }

        void Initialize(Vector2 wposition, Vector2 ivector, float velocity);

        void Reset();

        void Update(GameTime gameTime);
    }
}
