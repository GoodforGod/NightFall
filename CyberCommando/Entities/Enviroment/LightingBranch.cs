using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities.Enviroment
{
    class LightningBranch
    {
        List<LightningBolt> bolts = new List<LightningBolt>();

        public bool IsComplete { get { return bolts.Count == 0; } }
        public Vector2 End { get; private set; }
        private Vector2 direction;

        Texture2D Sprite;

        static Random rand = new Random();

        public LightningBranch(Vector2 start, Vector2 end, Texture2D sprite)
        {
            this.Sprite = sprite;
            End = end;
            direction = Vector2.Normalize(end - start);
            Create(start, end);
        }

        public void Update()
        {
            bolts = bolts.Where(x => !x.IsComplete).ToList();
            foreach (var bolt in bolts)
                bolt.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);
        }

        private void Create(Vector2 start, Vector2 end)
        {
            var mainBolt = new LightningBolt(start, end, Sprite);
            bolts.Add(mainBolt);

            int numBranches = rand.Next(3, 6);
            Vector2 diff = end - start;

            // pick a bunch of random points between 0 and 1 and sort them
            float[] branchPoints = Enumerable.Range(0, numBranches)
                .Select(x => Rand(0, 1f))
                .OrderBy(x => x).ToArray();

            for (int i = 0; i < branchPoints.Length; i++)
            {
                // Bolt.GetPoint() gets the position of the lightning bolt at specified fraction (0 = start of bolt, 1 = end)
                Vector2 boltStart = mainBolt.GetPoint(branchPoints[i]);

                // rotate 30 degrees. Alternate between rotating left and right.
                Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(30 * ((i & 1) == 0 ? 1 : -1)));
                Vector2 boltEnd = Vector2.Transform(diff * (1 - branchPoints[i]), rot) + boltStart;
                bolts.Add(new LightningBolt(boltStart, boltEnd, Sprite));
            }
        }

        static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
    }
}
