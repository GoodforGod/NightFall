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
    class Shadows
    {
        private GraphicsDevice graphicsDevice { get; set; }

        private int ReductionChainCount;
        private int BaseSize;
        private int DepthBufferSize;

        private Effect ResolveShadowsEffect { get; set; }
        private Effect ReductionEffect { get; set; }

        private RenderTarget2D DistortRT { get; set; }
        private RenderTarget2D ShadowMap { get; set; }
        private RenderTarget2D ShadowsRT { get; set; }
        private RenderTarget2D ProcessedShadowsRT { get; set; }

        private QuadRenderComponent QuadRender { get; set; }
        private RenderTarget2D DistancesRT { get; set; }
        private RenderTarget2D[] ReductionRT;


        /// <summary>
        /// Creates a new shadowmap resolver
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="quadRender"></param>
        /// <param name="baseSize">
        /// The size of the light regions 
        /// </param>
        public Shadows(GraphicsDevice graphicsDevice, QuadRenderComponent quadRender, ShadowMapSize maxShadowmapSize, ShadowMapSize maxDepthBufferSize)
        {
            this.graphicsDevice = graphicsDevice;
            this.QuadRender = quadRender;

            ReductionChainCount = (int)maxShadowmapSize;
            BaseSize = 2 << ReductionChainCount;
            DepthBufferSize = 2 << (int)maxDepthBufferSize;
        }

        public void LoadContent(ContentManager content)
        {
            ReductionEffect = content.Load<Effect>("reductionEffect");
            ResolveShadowsEffect = content.Load<Effect>("resolveShadowsEffect");

            DistortRT = new RenderTarget2D(graphicsDevice, BaseSize, BaseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            DistancesRT = new RenderTarget2D(graphicsDevice, BaseSize, BaseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            ShadowMap = new RenderTarget2D(graphicsDevice, 2, BaseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            ReductionRT = new RenderTarget2D[ReductionChainCount];
            for (int i = 0; i < ReductionChainCount; i++)
            {
                ReductionRT[i] = new RenderTarget2D(graphicsDevice, 2 << i, BaseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            }

            ShadowsRT = new RenderTarget2D(graphicsDevice, BaseSize, BaseSize);
            ProcessedShadowsRT = new RenderTarget2D(graphicsDevice, BaseSize, BaseSize);
        }

        public void ResolveShadows(Texture2D shadowCastersTexture, RenderTarget2D result, Vector2 lightPosition)
        {
            graphicsDevice.BlendState = BlendState.Opaque;

            ExecuteTechnique(shadowCastersTexture, DistancesRT, "ComputeDistances");
            ExecuteTechnique(DistancesRT, DistortRT, "Distort");
            ApplyHorizontalReduction(DistortRT, ShadowMap);
            ExecuteTechnique(null, ShadowsRT, "DrawShadows", ShadowMap);
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
            graphicsDevice.SetRenderTarget(destination);
            graphicsDevice.Clear(Color.White);
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
            graphicsDevice.SetRenderTarget(null);
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

                graphicsDevice.SetRenderTarget(d);
                graphicsDevice.Clear(Color.White);

                ReductionEffect.Parameters["SourceTexture"].SetValue(s);
                Vector2 textureDim = new Vector2(1.0f / (float)s.Width, 1.0f / (float)s.Height);
                ReductionEffect.Parameters["TextureDimensions"].SetValue(textureDim);

                foreach (EffectPass pass in ReductionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    QuadRender.Render(Vector2.One * -1, new Vector2(1, 1));
                }

                graphicsDevice.SetRenderTarget(null);

                s = d;
                step--;
            }

            //copy to destination
            graphicsDevice.SetRenderTarget(destination);
            ReductionEffect.CurrentTechnique = ReductionEffect.Techniques["Copy"];
            ReductionEffect.Parameters["SourceTexture"].SetValue(d);

            foreach (EffectPass pass in ReductionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                QuadRender.Render(Vector2.One * -1, new Vector2(1, 1));
            }

            ReductionEffect.Parameters["SourceTexture"].SetValue(ReductionRT[ReductionChainCount - 1]);
            graphicsDevice.SetRenderTarget(null);
        }
    }
}
