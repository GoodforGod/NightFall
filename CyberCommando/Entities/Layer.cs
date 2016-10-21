using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities
{
    class Layer
    {
        public Layer(Camera camera, List<Tuple<Rectangle, Texture2D>> sprites)
        {
            this.camera = camera;
            Parallax = Vector2.One;
            Sprites = sprites; 
        }

        public Vector2 Parallax { get; set; }
        public List<Tuple<Rectangle, Texture2D>> Sprites { get; private set; }

        public void Draw(SpriteBatch batcher)
        {
            batcher.Begin(SpriteSortMode.Deferred, 
                null, null, null, null, null, 
                camera.GetViewMatrix(Parallax));

            foreach (Tuple<Rectangle, Texture2D> sprite in Sprites)
            {
                batcher.Draw(sprite.Item2, sprite.Item1, Color.White);
            } 

            batcher.End();
        }

        private readonly Camera camera;
    }
}
