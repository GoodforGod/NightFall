using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CyberCommando.Entities;
using Microsoft.Xna.Framework;

namespace CyberCommando.Controllers.Commands
{
    /// <summary>
    /// REDUNDANT
    /// </summary>
    class DuckCommand : MoveCommand
    {
        public DuckCommand(Entity entity, Vector2 position) : base(entity, position)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }

    class FireCommand : Command
    {
        public FireCommand(Entity entity)
        {

        }

        public override void execute(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    class JumpCommand : MoveCommand
    {
        public JumpCommand(Entity entity, Vector2 position) : base(entity, position)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }

    class MoveLeftCommand : MoveCommand
    {
        public MoveLeftCommand(Entity entity, Vector2 position) : base(entity, position)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }

    class MoveRightCommand : MoveCommand
    {
        public MoveRightCommand(Entity entity, Vector2 position) : base(entity, position)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }
}
