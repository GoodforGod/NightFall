using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Services;

namespace CyberCommando.Entities
{
    /// <summary>
    /// Represent level states coresponding to textures of this level
    /// </summary>
    public enum LevelState
    {
        MAIN,
        EXTRA,
        RANDOM
    }

    /// <summary>
    /// Represents whole level with all layers in it
    /// </summary>
    class Level
    {
        public Dictionary<LevelState, Texture2D> Textures { get; set; }
        private List<Layer> Layers { get; set; }
        public Rectangle Limits { get; set; }

        public Level(string levelName, LayerLoader loader)
        {
            Limits = new Rectangle(0, 0, 3200, 860);
            var TextureAndLayers = loader.LoadAll(levelName);
            Textures = TextureAndLayers.Item1;
            Layers = TextureAndLayers.Item2;
            foreach (var layer in Layers)
            {
                layer.Texture = Textures[layer.State];
                layer.camera.Limits = this.Limits;
            }
        }

        public void LayersLookAt(Vector2 position)
        {
            foreach (var layer in Layers)
                layer.camera.LookAt(position);
        }

        public void Draw(SpriteBatch batcher)
        {
            foreach (var layer in Layers)
            {
                layer.Draw(batcher);
            }
        }
    }
}
