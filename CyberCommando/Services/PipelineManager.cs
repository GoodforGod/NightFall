using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CyberCommando.Services
{
    class PipelineManager
    {
        private ContentManager Content;

        private static PipelineManager _Instance;
        public static PipelineManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new PipelineManager();
                return _Instance;
            }
        }

        private Texture2D _SPlayer;
        public Texture2D SPlayer
        {
            get
            {
                if (_SPlayer == null)
                    return _SPlayer = Content.Load<Texture2D>(NPlayer);
                else return _SPlayer;
            }
        }
        private Texture2D _SEnemy;
        public Texture2D SEnemy
        {
            get
            {
                if (_SEnemy == null)
                    return _SEnemy = Content.Load<Texture2D>(NEnemy);
                else return _SEnemy;
            }
        }
        private Texture2D _SGun;
        public Texture2D SGun
        {
            get
            {
                if (_SGun == null)
                    return _SGun = Content.Load<Texture2D>(NGun);
                else return _SGun;
            }
        }

        public readonly string NPlayer = "c-test";
        public readonly string NEnemy = "e-test";
        public readonly string NGun = "g-test";
        public readonly string NLevel_1 = "cybertown";
        public readonly string NBloomExtract = "BloomExtract";
        public readonly string NBloomComdine = "BloomCombine";
        public readonly string NGaussBlur = "GaussianBlur";
        public readonly string NReduction = "reductionEffect";
        public readonly string NShadowResolver = "resolveShadowsEffect";

        private PipelineManager() { }

        public void Initialize(ContentManager content)
        {
            this.Content = content;
        }

        public Texture2D LoadT(string name)
        {
            return Content.Load<Texture2D>(name);
        }

        public Effect LoadE(string name)
        {
            return Content.Load<Effect>(name);
        }
    }
}
