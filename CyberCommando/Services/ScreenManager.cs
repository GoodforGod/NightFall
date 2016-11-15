using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Services.Utils;

namespace CyberCommando.Services
{
    /// <summary>
    /// 
    /// </summary>
    public enum ScreenState
    {
        Game,
        Menu,
        Titles,
        Options
    }

    /// <summary>
    /// 
    /// </summary>
   public class ScreenManager
    {
        protected ContentManager Content;
        public ResolutionState ResolutionCurrent { get; set; }

        Screen          CurrentScreen;
        GraphicsDevice  GraphDev;
        GameCore        Core;

        Dictionary<ScreenState, Screen> Screens = new Dictionary<ScreenState, Screen>();

        private static ScreenManager _Instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ScreenManager();
                return _Instance;
            }
        }

        private ScreenManager() { }

        public void Exit() { Core.NeedExit = true; }

        public void Resize(ResolutionState res) { Core.Resize(res); }

        public void SwitchScreen(ScreenState type, params object[] param)
        {
            CurrentScreen.UnloadContent();
            CurrentScreen = Screens[type];

            if (!CurrentScreen.IsInitialized)
                CurrentScreen.Initialize(GraphDev, Core, param);

            CurrentScreen.LoadContent(Content);
            CurrentScreen.Resize(ResolutionCurrent, GraphDev.Viewport.Width, GraphDev.Viewport.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(GraphicsDevice graphdev, Game core)
        {
            this.GraphDev = graphdev;
            this.Core = (GameCore)core;

            Screens.Add(ScreenState.Game, new GameScreen());
            Screens.Add(ScreenState.Menu, new MenuScreen());
            Screens.Add(ScreenState.Options, new OptionScreen());
            Screens.Add(ScreenState.Titles, new TitleScreen());

            CurrentScreen = Screens[ScreenState.Titles];
        }

        public void UpdateResolution(ResolutionState res)
        {
            this.ResolutionCurrent = res;

            foreach (var screen in Screens)
                if (screen.Value.IsInitialized)
                    screen.Value.Resize(res, GraphDev.Viewport.Width, GraphDev.Viewport.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, content.RootDirectory);
            CurrentScreen.Initialize(GraphDev, Core);
            CurrentScreen.LoadContent(content);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnloadContent()
        {
            foreach (var screen in Screens)
                screen.Value.UnloadContent();
            this.Content.Unload();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            CurrentScreen.Draw(batcher, gameTime);
        }
    }
}
