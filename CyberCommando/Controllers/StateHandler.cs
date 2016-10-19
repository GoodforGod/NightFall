using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities;

namespace CyberCommando.Controllers
{
    class StateHandler
    {
        float PositionInc = 5f;

        public StateHandler()
        {

        }

        public virtual void MoveRight(Entity entity)
        {
            entity.Position.X += PositionInc;
        }

        public virtual void MoveLeft(Entity entity)
        {
            entity.Position.X -= PositionInc;
        }

        public virtual void Jump(Entity entity)
        {
            entity.Position.Y -= PositionInc;
        }

        public virtual void Duck(Entity entity)
        {
            entity.Position.Y += PositionInc;
        }

        public virtual void Fire(Entity entity)
        {

        }

    }
}
