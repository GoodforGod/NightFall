using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CyberCommando.Services
{
    class Layerclass
    {
        private ContentManager Content;

        public Layerclass(ContentManager contentRoot) { this.Content = contentRoot; }

        public List<Tuple<Rectangle, Texture2D>> LoadAll(string layerName)
        {
            Texture2D texture = null;
            var layers = new List<Tuple<Rectangle, Texture2D>>();

            string prevState = "";

            var dataFile = Path.Combine(Content.RootDirectory, Path.ChangeExtension(layerName, "txt"));
            var dataFileLines = File.ReadAllLines(dataFile);

            // Line starts with #, is comment
            foreach (var cols in from row in dataFileLines
                                 where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                                 select row.Split(';'))
            {
                if (cols.Length != 5)
                    throw new InvalidDataException("Incorrect format data in file: " + layerName);

                if (cols[0] != prevState)
                {
                    texture = Content.Load<Texture2D>(cols[0]);
                    prevState = cols[0];
                }

                Rectangle rectangle;

                try
                {
                    rectangle = new Rectangle(
                    int.Parse(cols[1]),
                    int.Parse(cols[2]),
                    int.Parse(cols[3]),
                    int.Parse(cols[4]));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Inccorect rectangle format in: " + layerName, cols[0]);
                }

                layers.Add(new Tuple<Rectangle, Texture2D>(rectangle, texture));
            }

            return layers;
        }
    }
}
