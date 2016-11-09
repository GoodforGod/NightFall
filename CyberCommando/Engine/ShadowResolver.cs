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
    /// <summary>
    /// Sizes of the shadow map
    /// </summary>
    enum ShadowMapSize
    {
        Size128 = 6,
        Size256 = 7,
        Size512 = 8,
        Size1024 = 9,
    }

    /// <summary>
    /// Calculates shadowMap and apply effects (shaders)
    /// </summary>
    class ShadowResolver
    {
        private GraphicsDevice GraphDev { get; set; }

        private int ReductionChainCount;
        private int BaseSize;
        private int DepthBufferSize;

        private Effect ResolveShadowsEffect { get; set; }
        private Effect ReductionEffect { get; set; }

        public Color SColor { get; set; }

        private RenderTarget2D DistortRT { get; set; }
        private RenderTarget2D SMap { get; set; }
        private RenderTarget2D ShadowsRT { get; set; }
        private RenderTarget2D ProcessedShadowsRT { get; set; }
        private RenderTarget2D SScreen { get; set; }

        private QuadRenderComponent QuadRender { get; set; }
        private RenderTarget2D DistancesRT { get; set; }
        private RenderTarget2D[] ReductionRT;

        /// <summary>
        /// Creates a new shadowmap resolver
        /// </summary>
        /// <param name="graphdev"></param>
        /// <param name="quadRender"></param>
        /// <param name="baseSize">
        /// The size of the light regions 
        /// </param>
        public ShadowResolver(GraphicsDevice graphdev, QuadRenderComponent quadRender,
                            ShadowMapSize maxShadowmapSize, ShadowMapSize maxDepthBufferSize, Color color)
        {
            this.GraphDev = graphdev;
            this.QuadRender = quadRender;

            this.SColor = color;

            ReductionChainCount = (int)maxShadowmapSize;
            BaseSize = 2 << ReductionChainCount;
            DepthBufferSize = 2 << (int)maxDepthBufferSize;
            SScreen = new RenderTarget2D(graphdev, graphdev.Viewport.Width, graphdev.Viewport.Height);
        }

        public void LoadContent(ContentManager content)
        {
            ReductionEffect = content.Load<Effect>("reductionEffect");
            ResolveShadowsEffect = content.Load<Effect>("resolveShadowsEffect");

            DistortRT = new RenderTarget2D(GraphDev, 
                                                BaseSize,
                                                BaseSize, 
                                                false, 
                                                SurfaceFormat.HalfVector2, 
                                                DepthFormat.None);
            DistancesRT = new RenderTarget2D(GraphDev, 
                                                BaseSize, 
                                                BaseSize,
                                                false, 
                                                SurfaceFormat.HalfVector2,
                                                DepthFormat.None);
            SMap = new RenderTarget2D(GraphDev, 
                                                2, 
                                                BaseSize,
                                                false, 
                                                SurfaceFormat.HalfVector2, 
                                                DepthFormat.None);

            ReductionRT = new RenderTarget2D[ReductionChainCount];

            for (int i = 0; i < ReductionChainCount; i++)
            {
                ReductionRT[i] = new RenderTarget2D(GraphDev, 
                                                    2 << i, 
                                                    BaseSize, 
                                                    false, 
                                                    SurfaceFormat.HalfVector2, 
                                                    DepthFormat.None);
            }

            ShadowsRT = new RenderTarget2D(GraphDev, BaseSize, BaseSize);
            ProcessedShadowsRT = new RenderTarget2D(GraphDev, BaseSize, BaseSize);
        }

        public void BeginDraw(GraphicsDevice graphdev)
        {
            graphdev.SetRenderTarget(SScreen);
            graphdev.Clear(SColor);
        }

        public void EndDraw(GraphicsDevice graphdev)
        {
            graphdev.SetRenderTarget(null);
            graphdev.Clear(SColor);
        }

        public void DisplayShadowCast(SpriteBatch batcher)
        {
            BlendState blendState = new BlendState();
            blendState.ColorSourceBlend = Blend.DestinationColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;

            batcher.Begin(SpriteSortMode.Deferred, blendState);
            batcher.Draw(SScreen, Vector2.Zero, Color.White);
            batcher.End();
        }

        public void ResolveShadows(Texture2D shadowCastersTexture, RenderTarget2D result, Vector2 lightPosition)
        {
            GraphDev.BlendState = BlendState.Opaque;

            ExecuteTechnique(shadowCastersTexture, DistancesRT, "ComputeDistances");
            ExecuteTechnique(DistancesRT, DistortRT, "Distort");
            ApplyHorizontalReduction(DistortRT, SMap);
            ExecuteTechnique(null, ShadowsRT, "DrawShadows", SMap);
            ExecuteTechnique(ShadowsRT, ProcessedShadowsRT, "BlurHorizontally");
            ExecuteTechnique(ProcessedShadowsRT, result, "BlurVerticallyAndAttenuate");
        }

        private void ExecuteTechnique(Texture2D source, RenderTarget2D destination, string techniqueName)
        {
            ExecuteTechnique(source, destination, techniqueName, null);
        }

        private void ExecuteTechnique(Texture2D source, RenderTarget2D destination, string techniqueName, Texture2D shadowMap)
        {
            Vector2 renderTargetSize;
            renderTargetSize = new Vector2((float)BaseSize, (float)BaseSize);
            GraphDev.SetRenderTarget(destination);
            GraphDev.Clear(Color.White);
            ResolveShadowsEffect.Parameters["renderTargetSize"].SetValue(renderTargetSize);

            if (source != null)
                ResolveShadowsEffect.Parameters["InputTexture"].SetValue(source);
            if (shadowMap != null)
                ResolveShadowsEffect.Parameters["ShadowMapTexture"].SetValue(shadowMap);

            ResolveShadowsEffect.CurrentTechnique = ResolveShadowsEffect.Techniques[techniqueName];

            foreach (EffectPass pass in ResolveShadowsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                QuadRender.Render(Vector2.One * -1, Vector2.One);
            }
            GraphDev.SetRenderTarget(null);
        }

        private void ApplyHorizontalReduction(RenderTarget2D source, RenderTarget2D destination)
        {
            int step = ReductionChainCount - 1;
            RenderTarget2D s = source;
            RenderTarget2D d = ReductionRT[step];
            ReductionEffect.CurrentTechnique = ReductionEffect.Techniques["HorizontalReduction"];

            while (step >= 0)
            {
                d = ReductionRT[step];

                GraphDev.SetRenderTarget(d);
                GraphDev.Clear(Color.White);

                ReductionEffect.Parameters["SourceTexture"].SetValue(s);
                Vector2 textureDim = new Vector2(1.0f / (float)s.Width, 1.0f / (float)s.Height);
                ReductionEffect.Parameters["TextureDimensions"].SetValue(textureDim);

                foreach (EffectPass pass in ReductionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    QuadRender.Render(Vector2.One * -1, new Vector2(1, 1));
                }

                GraphDev.SetRenderTarget(null);

                s = d;
                step--;
            }

            //copy to destination
            GraphDev.SetRenderTarget(destination);
            ReductionEffect.CurrentTechnique = ReductionEffect.Techniques["Copy"];
            ReductionEffect.Parameters["SourceTexture"].SetValue(d);

            foreach (EffectPass pass in ReductionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                QuadRender.Render(Vector2.One * -1, new Vector2(1, 1));
            }

            ReductionEffect.Parameters["SourceTexture"].SetValue(ReductionRT[ReductionChainCount - 1]);
            GraphDev.SetRenderTarget(null);
        }
    }
}
