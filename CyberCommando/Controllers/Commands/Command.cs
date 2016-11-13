using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using CyberCommando.Entities;

namespace CyberCommando.Controllers.Commands
{
    /// <summary>
    /// REDUNDANT
    /// </summary>
    abstract class MoveCommand : Command
    {
        Vector2 Position; 
        Entity Entity;

        public MoveCommand(Entity entity, Vector2 position)
        {
            this.Entity = entity;
            this.Position = position;
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
