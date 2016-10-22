using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using CyberCommando.Entities;

namespace CyberCommando.Services
{
    class LayerLoader
    {
        private ContentManager Content { get; }

        private List<Layer> Layers = new List<Layer>();
        private Camera camera;
        private LevelState LayerState;
        private LevelState TextureState;
        private Vector2 parallax;
        private Dictionary<LevelState, Texture2D> Textures = new Dictionary<LevelState, Texture2D>();

        public LayerLoader(ContentManager content, Viewport viewport)
        {
            this.camera = new Camera(viewport);
            this.Content = content;
        }

        public Tuple<Dictionary<LevelState, Texture2D>, List<Layer>> LoadAll(string layerName)
        {
            List<Sprite> layer = new List<Sprite>();

            var dataFile = Path.Combine(Content.RootDirectory, layerName + ".txt");
            var dataFileLines = File.ReadAllLines(dataFile);

            // Line starts with #, is comment
            foreach (var cols in from row in dataFileLines
                                 where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                                 select row.Split(';'))
            {
                if(cols[0] == "!")
                {
                    if (!Enum.TryParse(cols[1], true, out TextureState))
                        throw new ArgumentException("Incorrect TextureState format in: " + layerName, cols[0]);

                    for(int i = 1; i < cols.Length - 1; i++)
                    {
                        Textures.Add(TextureState, Content.Load<Texture2D>(cols[i + 1]));
                    }

                    continue;
                }
                else if (cols.Length == 3)
                {
                    if (layer != null && layer.Count != 0)
                        Layers.Add(new Layer(camera, layer, parallax, LayerState));

                    var x = .0f;
                    var y = .0f;

                    if (!Enum.TryParse(cols[0], true, out LayerState))
                        throw new ArgumentException("Incorrect LayerState format in: " + layerName, cols[0]);

                    if(!float.TryParse(cols[1], out x))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[1]);

                    if (!float.TryParse(cols[2], out y))
                        throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[2]);

                    parallax = new Vector2(x, y);
                    layer = new List<Sprite>();
                    continue;
                }
                else if (cols.Length != 4 && cols.Length != 6)
                    throw new InvalidDataException("Incorrect format data in file: " + layerName);

                Rectangle rectangle;

                try
                {
                    rectangle = new Rectangle(
                    int.Parse(cols[0]),
                    int.Parse(cols[1]),
                    int.Parse(cols[2]),
                    int.Parse(cols[3]));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Inccorect rectangle format in: " + layerName, cols[0]);
                }

                if (cols.Length == 4)
                {
                    layer.Add(new Sprite(rectangle));
                }
                else if (cols.Length == 6)
                {
                    Vector2 position = new Vector2();

                    if (!float.TryParse(cols[4], out position.X))
                        throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[4]);

                    if (!float.TryParse(cols[5], out position.Y))
                        throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[5]);

                    layer.Add(new Sprite(rectangle, position));
                }
                Layers.Add(new Layer(camera, layer, parallax, LayerState));
            }
            return new Tuple<Dictionary<LevelState, Texture2D>, List<Layer>>(Textures, Layers);
        }

        public Vector2 PositionGenerator()
        {

            return Vector2.One;
        }
    }
}
