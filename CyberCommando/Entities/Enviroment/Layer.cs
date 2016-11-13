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
        public Color LColor { get; set; }
        public float ResScale { get; set; }
        public bool IsFocusedDraw { get; set; }

        public Layer(Camera camera, List<Sprite> layerRects, LevelState state, Color color)
        {
            ResScale = 1f;
            this.Camera = camera;
            this.LColor = color;
            this.LayerSprites = layerRects;
            this.State = state;
            this.Parallax = Vector2.One;
            if (State == LevelState.FRONT)
                IsFocusedDraw = true;
        }

        public Layer(Camera camera, List<Sprite> layerRects, LevelState state, Color color, Vector2 parallax) 
                                                                            : this(camera, layerRects, state, color)
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

        public void Draw(SpriteBatch batcher)
        {
            InitDraw(batcher);

            foreach (var sprite in LayerSprites)
            {
                batcher.Draw(Texture,
                                sprite.Position,
                                sprite.Source,
                                LColor,
                                .0f,
                                Vector2.One,
                                sprite.ResScale,
                                SpriteEffects.None,
                                1.0f);
            }
        }

        public void Draw(SpriteBatch batcher, Vector2 limits)
        {
            InitDraw(batcher);

            //var limR = position.X + limits.X;
            //var limL = position.X - limits.X;
            var limR = limits.X;
            var limL = limits.Y;

            foreach (var sprite in LayerSprites)
            {
                if (IsOnScreen(sprite.Position.X + sprite.Source.Width, sprite.Position.X, limL, limR))
                    batcher.Draw(Texture,
                                    sprite.Position,
                                    sprite.Source,
                                    LColor,
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
