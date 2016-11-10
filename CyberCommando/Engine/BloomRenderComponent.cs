using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Engine
{
    class BloomRenderComponent : DrawableGameComponent
    {
        Effect EBloomExtract;
        Effect EBloomCombine;
        Effect EGaussianBlur;

        RenderTarget2D SceneRSource;
        RenderTarget2D SceneRFinal;
        RenderTarget2D RTarget1;
        RenderTarget2D RTarget2;

        Color SColor;

        // Choose what display settings the bloom should use.
        BloomParams Settings;

        SpriteBatch Batcher;

        // Optionally displays one of the intermediate buffers used
        // by the bloom postprocess, so you can see exactly what is
        // being drawn into each rendertarget.
        public enum IntermediateBuffer
        {
            PreBloom,
            BlurredHorizontally,
            BlurredBothWays,
            FinalResult,
        }

        public IntermediateBuffer ShowBuffer { get; set; }

        public BloomRenderComponent(Game game) : base(game)
        {
            SColor = Color.Gray;
            Settings = BloomParams.PresetSettings[0];
            ShowBuffer = IntermediateBuffer.FinalResult;
        }

        public void UpdateSettings(int setup)
        {
            Settings = BloomParams.PresetSettings[setup];
        }


        public void BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(SceneRSource);
            GraphicsDevice.Clear(SColor);
        }

        public void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(SColor);
        }

        protected override void LoadContent()
        {
            EBloomExtract = Game.Content.Load<Effect>("BloomExtract");
            EBloomCombine = Game.Content.Load<Effect>("BloomCombine");
            EGaussianBlur = Game.Content.Load<Effect>("GaussianBlur");

            Batcher = new SpriteBatch(GraphicsDevice);

            var pp = Game.GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            // source texture for rendering all bloom effect textures
            SceneRSource = new RenderTarget2D(Game.GraphicsDevice,
                                                            width,
                                                            height,
                                                            false,
                                                            format,
                                                            pp.DepthStencilFormat,
                                                            pp.MultiSampleCount,
                                                            RenderTargetUsage.DiscardContents);

            // final texture for rendering all bloomed to the screen
            SceneRFinal = new RenderTarget2D(Game.GraphicsDevice,
                                                            width,
                                                            height,
                                                            false,
                                                            format,
                                                            pp.DepthStencilFormat,
                                                            pp.MultiSampleCount,
                                                            RenderTargetUsage.DiscardContents);


            // Half of screen due to blur (still lose quality) and performance
            width /= 2;
            height /= 2;

            RTarget1 = new RenderTarget2D(Game.GraphicsDevice, width, height, false, format, DepthFormat.None);
            RTarget2 = new RenderTarget2D(Game.GraphicsDevice, width, height, false, format, DepthFormat.None);
        }

        protected override void UnloadContent()
        {
            SceneRFinal.Dispose();
            SceneRSource.Dispose();
            RTarget1.Dispose();
            RTarget2.Dispose();
        }

        public void DisplayBloomTarget()
        {
            Batcher.Begin(0, BlendState.AlphaBlend);
            Batcher.Draw(SceneRFinal, 
                new Rectangle(0, 0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight), 
                                                                                    Color.White);
            Batcher.End();
        }

        public void DrawBloom()
        {
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            EBloomExtract.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);

            // Draw source scene to bloom it
            DrawFullscreenQuad(SceneRSource, RTarget1,
                               EBloomExtract,
                               IntermediateBuffer.PreBloom);

            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)RTarget1.Width, 0);
            DrawFullscreenQuad(RTarget1, RTarget2,
                               EGaussianBlur,
                               IntermediateBuffer.BlurredHorizontally);

            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)RTarget1.Height);
            DrawFullscreenQuad(RTarget2, RTarget1,
                               EGaussianBlur,
                               IntermediateBuffer.BlurredBothWays);

            // shader that combines them to produce the final bloomed result.
            GraphicsDevice.SetRenderTarget(SceneRFinal);

            EffectParameterCollection parameters = EBloomCombine.Parameters;

            parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

            // Set source to apply all effects
            GraphicsDevice.Textures[1] = SceneRSource;

            DrawFullscreenQuad(RTarget1,
                               GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                               EBloomCombine,
                               IntermediateBuffer.FinalResult);
        }

        public void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, 
                                        Effect effect, IntermediateBuffer currentBuffer)
        {
            Game.GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer);
        }

        void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, 
                                    IntermediateBuffer currentBuffer)
        {
            // for intermidiate buffer options, still draw
            if (ShowBuffer < currentBuffer)
                effect = null;

            Batcher.Begin(SpriteSortMode.Deferred, 
                                BlendState.Opaque, 
                                null, null, null, 
                                effect);
            Batcher.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            Batcher.End();
        }

        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = EGaussianBlur.Parameters["SampleWeights"];
            offsetsParameter = EGaussianBlur.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to  one.
            for (int i = 0; i < sampleWeights.Length; i++)
                sampleWeights[i] /= totalWeights;

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        private float ComputeGaussian(float n)
        {
            float theta = Settings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
