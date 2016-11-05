using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using CyberCommando.Entities.Enviroment;
using CyberCommando.Services;

namespace CyberCommando.Entities.Enviroment
{
    /// <summary>
    /// Represent level states coresponding to textures of this level
    /// </summary>
    public enum LevelState
    {
        FRONT,
        MIDDLE,
        BACK,
        BACKGROUND,
        BLURY
    }

    /// <summary>
    /// Represents whole level with all layers in it
    /// </summary>
    class Level
    {
        private List<Layer> Layers { get; set; }
        public Rectangle Limits { get; private set; }
        private Layer Frontground;

        public Level(string levelName, LayerLoader loader)
        {
            var LoadedPack = loader.LoadAll(levelName);
            Limits = LoadedPack.Item3;
            Layers = LoadedPack.Item2;
            foreach (var layer in Layers)
            {
                layer.Texture = LoadedPack.Item1[layer.State];
                layer.Camera.Limits = this.Limits;
                if (layer.State == LevelState.FRONT)
                    Frontground = layer;
            }
        }

        public void LayersZoom(float zoom)
        {
            foreach (var layer in Layers)
                layer.Camera.Zoom = zoom;
        }

        public void LayersLookAt(Vector2 position)
        {
            foreach (var layer in Layers)
                layer.Camera.LookAt(position);
        }

        public void LayersUpdateCameras(Vector2 origin, Viewport viewport)
        {
            foreach (var layer in Layers)
            {
                layer.Camera.Origin = origin;
                layer.Camera.viewport = viewport;
            }
        }

        public void LayersUpdateScale(float scale)
        {
            foreach (var layer in Layers)
                layer.ResScale = scale;
        }

        public void EndDrawFrontground(SpriteBatch bather) { Frontground.EndDraw(bather); }

        public void DrawFrontground(SpriteBatch bather, Vector2 pos) { Frontground.Draw(bather, pos); }

        public void DrawBackground(SpriteBatch batcher, Vector2 pos)
        {
            foreach (var layer in Layers)
                if (layer.State != LevelState.FRONT)
                {
                    layer.Draw(batcher, pos);
                    layer.EndDraw(batcher);
                }
        }
    }
}
