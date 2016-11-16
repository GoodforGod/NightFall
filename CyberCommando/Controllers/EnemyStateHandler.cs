using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Controllers
{
    /// <summary>
    /// Enemy level manager, handles all Enemy state via overrided/base methods <see cref="EntityStateHandler"/>
    /// </summary>
    class EnemyStateHandler : EntityStateHandler
    {
        private DateTime         Cooldown    = DateTime.Now;
        

    }
}
