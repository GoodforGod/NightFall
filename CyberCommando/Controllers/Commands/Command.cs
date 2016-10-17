using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCommando.Entities;

namespace CyberCommando.Controllers.Commands
{
    abstract class MoveCommand : Command
    {
        float _X;
        float _Y;
        Entity entity;

        public MoveCommand(Entity entity, float _X, float _Y)
        {
            this.entity = entity;
            this._X = _X;
            this._Y = _Y;
        }

        public override void execute(Entity entity) { }

        public abstract void execute();
    }

    abstract class Command : ICommand
    {
        public abstract void execute(Entity entity);
    }

    interface ICommand
    {
        void execute(Entity entity);
    }
}
