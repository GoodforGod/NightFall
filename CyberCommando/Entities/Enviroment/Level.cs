using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using CyberCommando.Entities.Enviroment;
using CyberCommando.Services;
using CyberCommando.Engine;

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
        public List<LightSpot> Lights { get; private set; }
        public Rectangle Limits { get; private set; }
        private Layer Frontground;
        private LightningBranch LBranch;
        private Random LRand;
        private Texture2D LBranchSprite;
        private DateTime Begin;

        public Level(string levelName, LoadManager loader)
        {
            var LoadedPack = loader.LoadLevel(levelName, ServiceLocator.Instance.GraphDev);
            Limits = LoadedPack.Item4;
            Lights = LoadedPack.Item3;
            Layers = LoadedPack.Item2;

            foreach (var layer in Layers)
            {
                layer.Texture = LoadedPack.Item1[layer.State];
                layer.Camera.Limits = this.Limits;
                if (layer.State == LevelState.FRONT)
                    Frontground = layer;
            }
            LBranchSprite = Layers[0].Texture;
            LBranch = new LightningBranch(new Vector2(100, Limits.Y), new Vector2(100, Limits.Height), LBranchSprite);
            Begin = DateTime.Now;
            LRand = new Random(Guid.NewGuid().GetHashCode());
        }

        public void LayersUpdate(int pos, int halfFrameW)
        {
            var timenow = DateTime.Now;

            if ((timenow - Begin).TotalSeconds > 3)
            {
                var randpos = LRand.Next(pos - halfFrameW , pos + halfFrameW);

                LBranch = new LightningBranch(new Vector2(randpos, Limits.Y), new Vector2(randpos, Limits.Height), LBranchSprite);
                Begin = timenow;
            }

            if (LBranch != null)
                LBranch.Update();
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
                layer.UpdateScale(scale);
        }

        public void EndDrawFrontground(SpriteBatch bather) { Frontground.EndDraw(bather); }

        public void DrawFrontground(SpriteBatch bather, Vector2 pos) { Frontground.Draw(bather, pos); }

        public void DrawLighting(SpriteBatch batcher, Matrix layerCamMatrix)
        {
            batcher.Begin(SpriteSortMode.Texture,
                                BlendState.Additive, 
                                null, null, null, null, 
                                layerCamMatrix);
            LBranch.Draw(batcher);
            batcher.End();
        }

        public void DrawBackground(SpriteBatch batcher, Vector2 pos)
        {
            foreach (var layer in Layers)
                if (layer.State != LevelState.FRONT)
                {
                    if (layer.State == LevelState.BACKGROUND && LBranch != null)
                        DrawLighting(batcher, layer.Camera.GetViewMatrix(layer.Parallax));
                    layer.Draw(batcher, pos);
                    layer.EndDraw(batcher);
                }
        }
    }
}
