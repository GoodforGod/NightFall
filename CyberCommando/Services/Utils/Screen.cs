using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Services.Utils
{
    /// <summary>
    /// 
    /// </summary>
    class Screen
    {
        protected ContentManager Content;

        protected GraphicsDevice GraphDev { get; set; }
        protected Game CoreGame { get; set; }

        protected int FWidth { get; private set; }
        protected int FHeight { get; private set; }
        protected float SScale { get; private set; }

        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            this.GraphDev = graphdev;
            this.CoreGame = game;
            IsInitialized = true;
            SScale = 1.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void Resize(ResolutionState res, int width, int height)
        {
            switch (res)
            {
                case ResolutionState.R1280x720: SScale = 1; break;
                case ResolutionState.R1600x900: SScale = 1.25f; break;
                case ResolutionState.R1920x1080: SScale = 1.5f; break;
            }

            this.FWidth = width;
            this.FHeight = height;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, content.RootDirectory);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UnloadContent() { this.Content.Unload(); }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Draw(SpriteBatch batcher, GameTime gameTime) { }
    }
}
