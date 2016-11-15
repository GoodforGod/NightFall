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
    [Flags]
    public enum LevelState
    {
        NONE = 0,
        BACKGROUND = 1,
        BACK = 2,
        MIDDLE = 4,
        FRONT = 8,
        BLURY = 16,
        STATIC = 32
    }

    /// <summary>
    /// Represents whole level with all layers in it
    /// </summary>
    class Level
    {
        public Dictionary<LevelState, Layer> Layers { get; private set; }
        public List<LightSpot>               Lights { get; private set; }
        public Rectangle                     Limits { get; private set; }

        LightningBranch LBranch;
        Texture2D       LBranchSprite;
        DateTime        Begin;
        LevelState      LDrawEndState;

        static Random   LRand = new Random(Guid.NewGuid().GetHashCode());

        public Level() { }

        public void Initialize(string levelName, LoadManager loader)
        {
            var LoadedPack = loader.LoadLevel(levelName, ServiceLocator.Instance.GraphDev);
            this.Limits = LoadedPack.Item4;
            this.Lights = LoadedPack.Item3;
            this.Layers = LoadedPack.Item2;

            foreach (var layer in Layers)
            {
                layer.Value.Texture = LoadedPack.Item1[layer.Value.State];
                layer.Value.Camera.Limits = this.Limits;
            }

            this.LBranchSprite = Layers[LevelState.BACKGROUND].Texture;
            this.LBranch = new LightningBranch(new Vector2(100, Limits.Y), new Vector2(100, Limits.Height), LBranchSprite);
            this.Begin = DateTime.Now;
        }

        public void Dispose()
        {
            foreach (var layer in Layers)
                layer.Value.Dispose();

            LBranchSprite.Dispose();
            LBranch = null;
            Layers.Clear();
            Lights.Clear();
        }

        public void LayersUpdate(int pos, int FWidthHalf)
        {
            var timenow = DateTime.Now;

            if ((timenow - Begin).TotalSeconds > 3)
            {
                var randpos = LRand.Next(pos - FWidthHalf , pos + FWidthHalf);

                LBranch = new LightningBranch(new Vector2(randpos, Limits.Y), new Vector2(randpos, Limits.Height), LBranchSprite);
                Begin = timenow;
            }

            if (LBranch != null)
                LBranch.Update();
        }

        public void LayersZoom(float zoom)
        {
            foreach (var layer in Layers)
                layer.Value.Camera.Zoom = zoom;
        }

        public void LayersLookAt(Vector2 position)
        {
            foreach (var layer in Layers)
                layer.Value.Camera.LookAt(position);
        }

        public void LayersUpdateCameras(Vector2 origin, Viewport viewport)
        {
            foreach (var layer in Layers)
            {
                layer.Value.Camera.Origin = origin;
                layer.Value.Camera.viewport = viewport;
            }
        }

        public void LayersUpdateScale(float scale)
        {
            if (Layers != null)
                foreach (var layer in Layers)
                    layer.Value.UpdateScale(scale);
        }

        public void DrawSpecificLayer(SpriteBatch batcher, Vector2 limits, LevelState lStates, bool endBatcher)
        {
            foreach (LevelState state in Enum.GetValues(lStates.GetType()))
            {
                if (lStates.HasFlag(state) && state != LevelState.NONE)
                {
                    if (Layers[state] != null)
                    {
                        LDrawEndState = state;
                        if (state == LevelState.BACK && LBranch != null)
                            DrawLighting(batcher, Layers[state].Camera.GetViewMatrix(Layers[state].Parallax));
                        Layers[state].Draw(batcher, limits);
                    }
                }
                Layers[state].EndDraw(batcher);
            }
        }

        public void EndDrawSpecificLayer(SpriteBatch bather)
        {
            if (Layers[LDrawEndState] != null)
                Layers[LDrawEndState].EndDraw(bather);
        }

        public void DrawLighting(SpriteBatch batcher, Matrix layerCamMatrix)
        {
            batcher.Begin(SpriteSortMode.Texture,
                                BlendState.Additive, 
                                null, null, null, null, 
                                layerCamMatrix);
            LBranch.Draw(batcher);
            batcher.End();
        }

        public void DrawBackground(SpriteBatch batcher)
        {
            foreach (var layer in Layers)
                if (layer.Value.State != LevelState.FRONT)
                {
                    if (layer.Value.State == LevelState.BACK && LBranch != null)
                        DrawLighting(batcher, layer.Value.Camera.GetViewMatrix(layer.Value.Parallax));
                    layer.Value.Draw(batcher);
                    layer.Value.EndDraw(batcher);
                }
        }
    }
}
