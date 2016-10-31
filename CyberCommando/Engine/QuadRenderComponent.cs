using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Engine
{
    public partial class QuadRenderComponent : DrawableGameComponent
    {
        VertexPositionTexture[] Vertexs = null;
        short[] ib = null;

        // Constructor
        public QuadRenderComponent(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            Vertexs = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
            };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };

            base.LoadContent();
        }


        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public void Render(Vector2 v1, Vector2 v2)
        {
            Vertexs[0].Position.X = v2.X;
            Vertexs[0].Position.Y = v1.Y;

            Vertexs[1].Position.X = v1.X;
            Vertexs[1].Position.Y = v1.Y;

            Vertexs[2].Position.X = v1.X;
            Vertexs[2].Position.Y = v2.Y;

            Vertexs[3].Position.X = v2.X;
            Vertexs[3].Position.Y = v2.Y;

            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Vertexs, 0, 4, ib, 0, 2);
        }
    }
}
