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
        public Dictionary<string, Rectangle> SSources { get; private set; }

        private ServiceLocator Services;

        private readonly string[] Levels = { "menu", "prolog", "cybertown" };

        private LevelManager() { Services = ServiceLocator.Instance; }

        public void Initialize()
        {
            CurrentLevel = new Level();
            SSources = new Dictionary<string, Rectangle>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadLevel(LevelNames lvl)
        {
            CurrentLevel.Initialize(Levels[(int)lvl], Services.LManager);
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

        public void Update(Vector2 position, int pos, int FWidthHalf)
        {
            CurrentLevel.LayersLookAt(position);
            CurrentLevel.LayersUpdate(pos, FWidthHalf);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawBack(SpriteBatch batcher)
        {
            CurrentLevel.DrawBackground(batcher);
        }
        
        public void DrawBlur(SpriteBatch batcher, Vector2 limits)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawFront(SpriteBatch batcher, Vector2 limits)
        {
            CurrentLevel.DrawFrontground(batcher, limits);
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
