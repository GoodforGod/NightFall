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

        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialize(GraphicsDevice graphdev, Game game)
        {
            this.GraphDev = graphdev;
            this.CoreGame = game;
            IsInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void Resize(ResolutionState res, int width, int height) { }

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
        public virtual void Draw(SpriteBatch batcher) { }
    }
}
