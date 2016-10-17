using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities
{
    class Spawner
    {
        Entity prototype;

        public Spawner(Enemy prototype)
        {
            this.prototype = prototype;
        }

        public Entity spawnEnemy()
        {
            return prototype.Clone();
        }
    }
}
