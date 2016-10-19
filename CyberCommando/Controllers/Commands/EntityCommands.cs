using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCommando.Entities;

namespace CyberCommando.Controllers.Commands
{
    class DuckCommand : MoveCommand
    {
        public DuckCommand(Entity entity, float _X, float _Y) : base(entity, _X, _Y)
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
        public JumpCommand(Entity entity, float _X, float _Y) : base(entity, _X, _Y)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }

    class MoveLeftCommand : MoveCommand
    {
        public MoveLeftCommand(Entity entity, float _X, float _Y) : base(entity, _X, _Y)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }

    class MoveRightCommand : MoveCommand
    {
        public MoveRightCommand(Entity entity, float _X, float _Y) : base(entity, _X, _Y)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }
    }
}
