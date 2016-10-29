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
        public LevelState State { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Parallax { get; set; }
        public List<Sprite> LayerSprites { get; set; }
        public readonly Camera camera;

        public Layer(Camera camera, List<Sprite> layerRects, LevelState state)
        {
            this.camera = camera;
            this.LayerSprites = layerRects;
            this.Parallax = Vector2.One;
        }

        public Layer(Camera camera, List<Sprite> layerRects, Vector2 parallax, LevelState state)
        {
            this.camera = camera;
            this.LayerSprites = layerRects;
            this.State = state;
            this.Parallax = parallax;
        }

        public void Draw(SpriteBatch batcher)
        {
            batcher.Begin(SpriteSortMode.Deferred, 
                null, null, null, null, null, 
                camera.GetViewMatrix(Parallax));

            foreach (var sprite in LayerSprites)
            {
                batcher.Draw(Texture, sprite.Position, sprite.Source, Color.White);
            } 

            batcher.End();
        }
    }
}
