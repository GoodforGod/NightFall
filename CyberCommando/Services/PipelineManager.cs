using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CyberCommando.Services
{
    class PipelineManager
    {
        private ContentManager Content;

        private static PipelineManager _Instance;
        public static PipelineManager Instance
        { get {
                if (_Instance == null)
                    _Instance = new PipelineManager();
                return _Instance;
            } }

        private Texture2D _SGun;
        public Texture2D SGun
        { get {
                if (_SGun == null)
                    _SGun = Content.Load<Texture2D>(NSGun);
                return _SGun;
            } }

        private Texture2D _SPlayer;
        public Texture2D SPlayer
        { get {
                if (_SPlayer == null)
                    _SPlayer = Content.Load<Texture2D>(NSPlayer);
                return _SPlayer;
            } }

        public readonly static string ERoot = "Effects/";
        public readonly static string SRoot = "Sprites/";
        public readonly static string FRoot = "Fonts/";
        public readonly static string ARoot = "Audio/";

        public readonly Rectangle RProjectileLaser = new Rectangle(652, 102, 100, 184);

        public readonly string NRSegmentLineMiddle = "SegmentLineM";
        public readonly string NRSegmentLineEnding = "SegmentLineE";

        public readonly string NSDefault           = SRoot + "q";
        public readonly string NSPlayer            = SRoot + "c-test";
        public readonly string NSEnemy             = SRoot + "e-test";
        public readonly string NSGun               = SRoot + "g-test";
        public readonly string NSMenu              = SRoot + "menu";
        public readonly string NFTitleFont         = FRoot + "f-test";
        public readonly string NEBloomExtract      = ERoot + "BloomExtract";
        public readonly string NEBloomComdine      = ERoot + "BloomCombine";
        public readonly string NEGaussBlur         = ERoot + "GaussianBlur";
        public readonly string NEShadowReduction   = ERoot + "reductionEffect";
        public readonly string NEShadowResolver    = ERoot + "resolveShadowsEffect";

        public readonly string[] NLevels = { "menu", "prolog", "cybertown", "nighttown" };

        private PipelineManager() { }

        public void Initialize(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, content.RootDirectory);
        }

        public void Unload()
        {
            Content.Unload();
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
