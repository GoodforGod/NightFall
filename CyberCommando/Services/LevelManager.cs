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
        PROLOG,
        CYBERTOWN
    }

    class LevelManager
    {
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

        public Level CurrentLevel { get; private set; }
        public Rectangle CurrentLimits { get { return CurrentLevel.Limits; } }
        public List<LightSpot> CurrentLights { get { return CurrentLevel.Lights; } }

        private ServiceLocator Services;

        private readonly string[] Levels = { "prolog", "cybertown" };

        private LevelManager() { Services = ServiceLocator.Instance; }

        public void Initialize()
        {
            CurrentLevel = new Level();
            LoadLevel(LevelNames.CYBERTOWN);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadLevel(LevelNames level)
        {
            CurrentLevel.Initialize(Levels[(int)level], Services.LManager);
        }

        /// <summary>
        /// 
        /// </summary>
        public void NextLevel() { }

        public void UpdateScale(float scale, Vector2 origin, Viewport viewport)
        {
            CurrentLevel.LayersUpdateScale(scale);
            CurrentLevel.LayersUpdateCameras(origin, viewport);
        }

        public void Update(Vector2 position, int pos, int halfFrameW)
        {
            CurrentLevel.LayersLookAt(position);
            CurrentLevel.LayersUpdate(pos, halfFrameW);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawBack(SpriteBatch batcher, Vector2 position)
        {
            CurrentLevel.DrawBackground(batcher, position);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void DrawFront(SpriteBatch batcher, Vector2 position, Vector2 limits)
        {
            CurrentLevel.DrawFrontground(batcher, position, limits);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndDrawFront(SpriteBatch batcher)
        {
            CurrentLevel.EndDrawFrontground(batcher);
        }
    }
}
