using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Services
{
    /// <summary>
    /// Manage all game audio
    /// </summary>
    class AudioManager
    {
        private static AudioManager _Instance;
        public static AudioManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new AudioManager();
                return _Instance;
            }
        }

        private AudioManager() { }

        public void Unload() { }
    }
}
