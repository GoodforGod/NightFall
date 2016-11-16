using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CyberCommando.Entities.Enviroment
{
    class LightningBolt
    {
        public List<SegmentLine> Segments = new List<SegmentLine>();
        public Vector2  Start               { get { return Segments[0].SPoint; } }
        public Vector2  End                 { get { return Segments.Last().EPoint; } }
        public Vector2  LBPosition          { get; set; }

        public float    Alpha               { get; set; }
        public float    AlphaMultiplier     { get; set; }
        public float    FadeOutRate         { get; set; }
        public Color    Tint                { get; set; }

        public bool     IsComplete          { get { return Alpha <= 0; } }
        public bool     IsRendered          { get; private set; }

        public Texture2D LBRender           { get; private set; }
        RenderTarget2D  LBRenderTarget;
        Texture2D       Sprite;

        static Random RandomGen = new Random(Guid.NewGuid().GetHashCode());

        public LightningBolt(Vector2 source, Vector2 dest, Texture2D sprite)
                        : this(source, dest, sprite, new Color(0.9f, 0.8f, 1f)) { }

        public LightningBolt(Vector2 source, Vector2 dest, Texture2D sprite, Color tint)
        {
            this.Sprite = sprite;
            Segments = CreateBolt(LBPosition = source, dest, 2);

            this.Tint = tint;
            this.Alpha = 1f;
            this.AlphaMultiplier = 0.6f;
            this.FadeOutRate = 0.03f;
        }

        public void FillRender(SpriteBatch batcher, GraphicsDevice graphdev)
        {
            PresentationParameters pp = graphdev.PresentationParameters;
            LBRenderTarget = new RenderTarget2D(graphdev, 
                pp.BackBufferWidth,
                pp.BackBufferHeight, 
                true,
                graphdev.DisplayMode.Format, 
                DepthFormat.Depth24);

            Services.ServiceLocator.Instance.GraphDev.SetRenderTarget(LBRenderTarget);

            Draw(batcher);

            LBRender = (Texture2D)LBRenderTarget;
            Services.ServiceLocator.Instance.GraphDev.SetRenderTarget(null);
            IsRendered = true;
        }

        public void DrawRender(SpriteBatch batcher)
        {
            batcher.Draw(LBRender, LBPosition, Tint);
        }

        public void Draw(SpriteBatch batcher)
        {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(batcher, Tint * (Alpha * AlphaMultiplier));
        }

        public virtual void Update() { Alpha -= FadeOutRate; }

        protected List<SegmentLine> CreateBolt(Vector2 source, Vector2 dest, float thickness)
        {
            var results = new List<SegmentLine>();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add(Rand(0, 1));

            positions.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Rand(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new SegmentLine(prevPoint, point, Sprite, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new SegmentLine(prevPoint, dest, Sprite, thickness));

            return results;
        }

        // Returns the point where the bolt is at a given fraction of the way through the bolt. Passing
        // zero will return the start of the bolt, and passing 1 will return the end.
        public Vector2 GetPoint(float position)
        {
            var start = Start;
            float length = Vector2.Distance(start, End);
            Vector2 dir = (End - start) / length;
            position *= length;

            var line = Segments.Find(x => Vector2.Dot(x.EPoint - start, dir) >= position);
            float lineStartPos = Vector2.Dot(line.SPoint - start, dir);
            float lineEndPos = Vector2.Dot(line.EPoint - start, dir);
            float linePos = (position - lineStartPos) / (lineEndPos - lineStartPos);

            return Vector2.Lerp(line.SPoint, line.EPoint, linePos);
        }

        private static float Rand(float min, float max)
        {
            return (float)RandomGen.NextDouble() * (max - min) + min;
        }
    }
}
