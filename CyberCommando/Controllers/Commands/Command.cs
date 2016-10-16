using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Controllers.Commands
{
    abstract class Command : ICommand
    {
        public abstract void execute();
    }

    interface ICommand
    {
        void execute();
    }
}
