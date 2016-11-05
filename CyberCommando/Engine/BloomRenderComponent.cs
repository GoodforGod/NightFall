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
        Effect bloomExtractEffect;
        Effect bloomCombineEffect;
        Effect gaussianBlurEffect;

        RenderTarget2D sceneRenderTarget;
        RenderTarget2D sceneRenderFinal;
        RenderTarget2D renderTarget1;
        RenderTarget2D renderTarget2;

        Color color;

        // Choose what display settings the bloom should use.
        BloomParams Settings;

        SpriteBatch batcher;

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
            color = Color.Gray;
            Settings = BloomParams.PresetSettings[0];
            ShowBuffer = IntermediateBuffer.FinalResult;
        }

        public void UpdateSettings(int setup)
        {
            Settings = BloomParams.PresetSettings[setup];
        }

        public void UpdateBatcher()
        {
            batcher = new SpriteBatch(GraphicsDevice);

            var pp = Game.GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            // source texture for rendering all bloom effect textures
            sceneRenderTarget = new RenderTarget2D(Game.GraphicsDevice,
                                                            width,
                                                            height,
                                                            false,
                                                            format,
                                                            pp.DepthStencilFormat,
                                                            pp.MultiSampleCount,
                                                            RenderTargetUsage.DiscardContents);

            // final texture for rendering all bloomed to the screen
            sceneRenderFinal = new RenderTarget2D(Game.GraphicsDevice,
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

            renderTarget1 = new RenderTarget2D(Game.GraphicsDevice, width, height, false, format, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(Game.GraphicsDevice, width, height, false, format, DepthFormat.None);
        }

        public void BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            GraphicsDevice.Clear(color);
        }

        public void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(color);
        }

        protected override void LoadContent()
        {
            bloomExtractEffect = Game.Content.Load<Effect>("BloomExtract");
            bloomCombineEffect = Game.Content.Load<Effect>("BloomCombine");
            gaussianBlurEffect = Game.Content.Load<Effect>("GaussianBlur");

            UpdateBatcher();
        }

        protected override void UnloadContent()
        {
            sceneRenderFinal.Dispose();
            sceneRenderTarget.Dispose();
            renderTarget1.Dispose();
            renderTarget2.Dispose();
        }

        public void DisplayBloomTarget()
        {
            batcher.Begin(0, BlendState.AlphaBlend);
            batcher.Draw(sceneRenderFinal, 
                new Rectangle(0, 0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight), 
                                                                                    Color.White);
            batcher.End();
        }

        public void DrawBloom()
        {
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            bloomExtractEffect.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);

            DrawFullscreenQuad(sceneRenderTarget, renderTarget1,
                               bloomExtractEffect,
                               IntermediateBuffer.PreBloom);

            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);
            DrawFullscreenQuad(renderTarget1, renderTarget2,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredHorizontally);

            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);
            DrawFullscreenQuad(renderTarget2, renderTarget1,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredBothWays);

            // shader that combines them to produce the final bloomed result.
            GraphicsDevice.SetRenderTarget(sceneRenderFinal);

            EffectParameterCollection parameters = bloomCombineEffect.Parameters;

            parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

            GraphicsDevice.Textures[1] = sceneRenderTarget;

            DrawFullscreenQuad(renderTarget1,
                               GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                               bloomCombineEffect,
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
            // If the user has selected one of the show intermediate buffer options,
            // we still draw the quad to make sure the image will end up on the
            // but might need to skip applying the custom pixel shader.
            if (ShowBuffer < currentBuffer)
                effect = null;

            batcher.Begin(SpriteSortMode.Deferred, BlendState.Opaque, 
                                                    null, null, null, effect);
            batcher.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            batcher.End();
        }

        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

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
