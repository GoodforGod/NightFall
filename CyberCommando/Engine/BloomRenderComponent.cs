using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Services;

namespace CyberCommando.Engine
{
    class BloomRenderComponent : DrawableGameComponent
    {
        GraphicsDevice  GraphDev;
        SpriteBatch     Batcher;
        ContentManager  Content;

        Effect          EBloomExtract;
        Effect          EBloomCombine;
        Effect          EGaussianBlur;

        BloomParams     Settings;

        RenderTarget2D  SceneRSource;
        RenderTarget2D  SceneRFinal;
        RenderTarget2D  RTarget1;
        RenderTarget2D  RTarget2;

        public Color SColor { get; set; }

        // displays one of the intermediate buffers used bloom post 
        //to see what is being drawn into each rendertarget.
        public enum IntermediateBuffer
        {
            PreProcessBloom,
            BHorizontally,
            BVerticaly,
            Final,
        }

        public IntermediateBuffer IBuffer { get; set; }

        public BloomRenderComponent(Game game) : base(game)
        {
            this.GraphDev = game.GraphicsDevice;
            this.Content = game.Content;
            this.SColor = Color.Gray;
            this.Settings = BloomParams.BModes[BloomStates.SOFT];
            this.IBuffer = IntermediateBuffer.Final;
        }

        public void ChangeMode(BloomStates state)
        {
            Settings = BloomParams.BModes[state];
        }

        public void BeginDraw()
        {
            GraphDev.SetRenderTarget(SceneRSource);
            GraphDev.Clear(SColor);
        }

        public void EndDraw()
        {
            GraphDev.SetRenderTarget(null);
            GraphDev.Clear(SColor);
        }

        protected override void LoadContent()
        {
            EBloomExtract = Content.Load<Effect>(ServiceLocator.Instance.PLManager.NEBloomExtract);
            EBloomCombine = Content.Load<Effect>(ServiceLocator.Instance.PLManager.NEBloomComdine);
            EGaussianBlur = Content.Load<Effect>(ServiceLocator.Instance.PLManager.NEGaussBlur);

            Batcher = new SpriteBatch(GraphDev);

            var pp = GraphDev.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            // source texture for rendering all bloom effect textures
            SceneRSource = new RenderTarget2D(GraphDev,
                                                    width,
                                                    height,
                                                    false,
                                                    format,
                                                    pp.DepthStencilFormat,
                                                    pp.MultiSampleCount,
                                                    RenderTargetUsage.DiscardContents);

            // final texture for rendering all bloomed to the screen
            SceneRFinal = new RenderTarget2D(GraphDev,
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

            RTarget1 = new RenderTarget2D(GraphDev, width, height, false, format, DepthFormat.None);
            RTarget2 = new RenderTarget2D(GraphDev, width, height, false, format, DepthFormat.None);
        }

        protected override void UnloadContent()
        {
            SceneRFinal.Dispose();
            SceneRSource.Dispose();
            RTarget1.Dispose();
            RTarget2.Dispose();
            Content.Unload();
        }

        public void DisplayBloomTarget()
        {
            Batcher.Begin(0, BlendState.AlphaBlend);
            Batcher.Draw(SceneRFinal, 
                new Rectangle(0, 0, GraphDev.PresentationParameters.BackBufferWidth,
                                    GraphDev.PresentationParameters.BackBufferHeight), 
                                                                                    Color.White);
            Batcher.End();
        }

        public void DrawBloom()
        {
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            EBloomExtract.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);

            // Draw source scene to bloom it
            DrawFullScreenQuadRender(SceneRSource, RTarget1,
                                       EBloomExtract,
                                       IntermediateBuffer.PreProcessBloom);

            // using a shader to apply a horizontal gaussian blur filter.
            CalcBlurParam(1.0f / (float)RTarget1.Width, 0);
            DrawFullScreenQuadRender(RTarget1, RTarget2,
                                       EGaussianBlur,
                                       IntermediateBuffer.BHorizontally);

            // using a shader to apply a vertical gaussian blur filter.
            CalcBlurParam(0, 1.0f / (float)RTarget1.Height);
            DrawFullScreenQuadRender(RTarget2, RTarget1,
                                       EGaussianBlur,
                                       IntermediateBuffer.BVerticaly);

            // shader that combines them to produce the final bloomed result.
            GraphDev.SetRenderTarget(SceneRFinal);

            EffectParameterCollection parameters = EBloomCombine.Parameters;

            parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

            // Set source to apply all effects
            GraphicsDevice.Textures[1] = SceneRSource;

            DrawFullScreenQuadRender(RTarget1,
                                       GraphDev.Viewport.Width, GraphDev.Viewport.Height,
                                       EBloomCombine,
                                       IntermediateBuffer.Final);
        }

        public void DrawFullScreenQuadRender(Texture2D texture, RenderTarget2D renderTarget, 
                                                Effect effect, IntermediateBuffer currentBuffer)
        {
            GraphDev.SetRenderTarget(renderTarget);

            DrawFullScreenQuadRender(texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer);
        }

        void DrawFullScreenQuadRender(Texture2D texture, int width, int height, Effect effect, 
                                                                IntermediateBuffer currentBuffer)
        {
            // for intermidiate buffer options, still draw
            if (IBuffer < currentBuffer)
                effect = null;

            Batcher.Begin(SpriteSortMode.Deferred, 
                                BlendState.Opaque, 
                                null, null, null, 
                                effect);

            Batcher.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            Batcher.End();
        }

        private void CalcBlurParam(float dx, float dy)
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
            sampleWeights[0] = CalcGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = CalcGaussian(i + 1);
                var doubleI = i * 2 + 1;

                sampleWeights[doubleI] = weight;
                sampleWeights[doubleI + 1] = weight;

                totalWeights += weight * 2;
                float sampleOffset = doubleI + .5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative
                sampleOffsets[doubleI] = delta;
                sampleOffsets[doubleI + 1] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to  one.
            for (int i = 0; i < sampleWeights.Length; i++)
                sampleWeights[i] /= totalWeights;

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        private float CalcGaussian(float n)
        {
            float theta = Settings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
