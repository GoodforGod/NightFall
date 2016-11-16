using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Entities.Enviroment;
using Microsoft.Xna.Framework;
using CyberCommando.Engine;

namespace CyberCommando.Services
{
    public enum LevelNames
    {
        MENU,
        PROLOG,
        CYBERTOWN
    }

    class LevelManager
    {
        ServiceLocator Services;

        private static LevelManager _Instance;
        public static LevelManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new LevelManager();
                return _Instance;
            }
        }

        public Level                            CLevel { get; private set; }
        public Rectangle                        CLimits { get { return CLevel.Limits; } }
        public List<LightSpot>                  CLights { get { return CLevel.Lights; } }
        public Dictionary<string, Rectangle>    SSources { get; private set; }

        private LevelManager() { Services = ServiceLocator.Instance; }

        public void Initialize()
        {
            CLevel = new Level();
            SSources = new Dictionary<string, Rectangle>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadLevel(LevelNames lvl)
        {
            CLevel.Initialize(Services.PLManager.NLevels[(int)lvl], Services.LManager);
        }

        public void Unload()
        {
            if (SSources != null)
                SSources.Clear();
            if (CLights != null)
                CLights.Clear();
            if (CLevel != null)
                CLevel.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void NextLevel() { }

        public void UpdateScale(float scale, Vector2 origin, Viewport viewport)
        {
            CLevel.LayersUpdateScale(scale);
            CLevel.LayersUpdateCameras(origin, viewport);
        }

        public void Update(Vector2 position, int pos, int FWidthHalf)
        {
            CLevel.LayersLookAt(position);
            CLevel.LayersUpdate(pos, FWidthHalf);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawSpecific(SpriteBatch batcher, Vector2 limits, LevelState states, LevelState stoplvlState)
        {
            CLevel.DrawSpecificLayers(batcher, limits, states, stoplvlState);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndDrawLastLayer(SpriteBatch batcher)
        {
            CLevel.EndDrawSpecificLayer(batcher);
        }
    }
}
