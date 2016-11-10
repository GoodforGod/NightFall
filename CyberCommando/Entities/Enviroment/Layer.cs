using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Entities.Utils;

namespace CyberCommando.Entities.Enviroment
{
    /// <summary>
    /// Represents a layer in world, displayed via camera
    /// </summary>
    class Layer
    {
        public LevelState State { get; private set; }
        public Texture2D Texture { get; set; }
        public Vector2 Parallax { get; set; }
        public List<Sprite> LayerSprites { get; private set; }
        public Camera Camera { get; private set; }
        public float ResScale { get; set; }
        public bool IsFocusedDraw { get; set; }

        public Layer(Camera camera, List<Sprite> layerRects, LevelState state)
        {
            ResScale = 1f;
            this.Camera = camera;
            this.LayerSprites = layerRects;
            this.State = state;
            this.Parallax = Vector2.One;
            if (State == LevelState.FRONT)
                IsFocusedDraw = true;
        }

        public Layer(Camera camera, List<Sprite> layerRects, Vector2 parallax, LevelState state) 
                                                                            : this(camera, layerRects, state)
        {
            this.Parallax = parallax;
        }

        private bool IsOnScreen(float pos_l, float pos_r, float limitOnLeft, float limitOnRight)
        {
            if (pos_r <= limitOnRight && pos_l >= limitOnLeft)
                return true;
            else return false;
        }

        public void UpdateScale(float scale)
        {
            foreach (var sprite in LayerSprites)
                sprite.ResScale = sprite.Scale * scale;
        }

        private void InitDraw(SpriteBatch batcher)
        {
            batcher.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.LinearWrap,
                        DepthStencilState.Default,
                        RasterizerState.CullNone,
                        null,
                        Camera.GetViewMatrix(Parallax));
        }

        public void Draw(SpriteBatch batcher, Vector2 pos)
        {
            InitDraw(batcher);

            foreach (var sprite in LayerSprites)
            {
                batcher.Draw(Texture,
                                sprite.Position,
                                sprite.Source,
                                Color.Gray,
                                .0f,
                                Vector2.One,
                                sprite.ResScale,
                                SpriteEffects.None,
                                1.0f);
            }
        }

        public void Draw(SpriteBatch batcher, Vector2 pos, Vector2 limits)
        {
            InitDraw(batcher);

            var limR = pos.X + limits.X;
            var limL = pos.X - limits.X;

            foreach (var sprite in LayerSprites)
            {
                if (IsOnScreen(sprite.Position.X + sprite.Source.Width, sprite.Position.X, limL, limR));
                    batcher.Draw(Texture,
                                    sprite.Position,
                                    sprite.Source,
                                    Color.Gray,
                                    .0f,
                                    Vector2.One,
                                    sprite.ResScale,
                                    SpriteEffects.None,
                                    1.0f);
            }
        }

        public void EndDraw(SpriteBatch batcher) { batcher.End(); }
    }
}
