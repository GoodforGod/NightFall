using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCommando.Entities;

namespace CyberCommando.Controllers.Commands
{
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
}
