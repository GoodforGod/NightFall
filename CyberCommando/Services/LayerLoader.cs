using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using CyberCommando.Entities.Utils;
using CyberCommando.Entities.Enviroment;

namespace CyberCommando.Services
{
    /// <summary>
    /// Contain methods to load level
    /// </summary>
    class LayerLoader
    {
        private ContentManager Content { get; }
        private Camera camera;

        public LayerLoader(ContentManager content, Viewport viewport)
        {
            this.camera = new Camera(viewport);
            this.Content = content;
        }

        /// <summary>
        /// Loads all level components
        /// </summary>
        /// <param name="layerName">
        /// Name of the level
        /// </param>
        /// <returns>
        /// All textures assosiated with level and list of layers
        /// </returns>
        public Tuple<Dictionary<LevelState, Texture2D>, List<Layer>, Rectangle> LoadAll(string layerName)
        {
            LevelState TextureState;
            LevelState LayerState = LevelState.BACK;
            Rectangle limits = new Rectangle(0, 0, 1280, 720);

            var layers = new List<Layer>();
            var textures = new Dictionary<LevelState, Texture2D>();
            var sprites = new List<Sprite>();

            string textureName = "";
            Texture2D texture = Content.Load<Texture2D>("q");

            var parallax = new Vector2();

            var dataFile = Path.Combine(Content.RootDirectory, layerName + ".txt");
            var dataFileLines = File.ReadAllLines(dataFile);

            var cycles = 1;
            var incX = 0;
            var incY = 0;

            // Line starts with #, is comment
            foreach (var cols in from row in dataFileLines
                                 where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                                 select row.Split(';'))
            {
                if(cols[0] == "!")
                {
                    if (!Enum.TryParse(cols[1], true, out TextureState))
                        throw new ArgumentException("Incorrect TextureState format in: " + layerName, cols[0]);
                    for (int i = 1; i < cols.Length - 1; i++)
                    {
                        if (textureName != cols[i + 1])
                        {
                            textureName = cols[i + 1];
                            texture = Content.Load<Texture2D>(cols[i + 1]);
                        }
                        textures.Add(TextureState, texture);
                    }

                    continue;
                }
                else if (cols[0] == "@")
                {
                    try
                    {
                        limits = new Rectangle(
                        int.Parse(cols[1]),
                        int.Parse(cols[2]),
                        int.Parse(cols[3]),
                        int.Parse(cols[4]));
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Inccorect limit format in: " + layerName, cols[0]);
                    }
                    continue;
                }
                else if (cols[0] == "$")
                {
                    if (!int.TryParse(cols[1], out cycles))
                        throw new ArgumentException("Incorrect cycles format in: " + layerName, cols[1]);
                    if (!int.TryParse(cols[2], out incX))
                        throw new ArgumentException("Incorrect incX format in: " + layerName, cols[2]);
                    if (!int.TryParse(cols[3], out incY))
                        throw new ArgumentException("Incorrect incY format in: " + layerName, cols[3]);
                    continue;
                }
                else if (cols.Length == 3)
                {
                    if (sprites != null && sprites.Count != 0)
                        layers.Add(new Layer(camera, sprites, parallax, LayerState));

                    var x = .0f;
                    var y = .0f;

                    if (!Enum.TryParse(cols[0], true, out LayerState))
                        throw new ArgumentException("Incorrect LayerState format in: " + layerName, cols[0]);
                    if(!float.TryParse(cols[1], out x))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[1]);
                    if (!float.TryParse(cols[2], out y))
                        throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[2]);

                    parallax = new Vector2(x, y);
                    sprites = new List<Sprite>();
                    continue;
                }
                else if (cols.Length != 5 && cols.Length != 7)
                    throw new InvalidDataException("Incorrect format data in file: " + layerName);

                for (int i = 0; i < cycles; i++)
                {
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

                    if (cols.Length == 5)
                    {
                        var scale = 1.0f;

                        if (!float.TryParse(cols[5], out scale))
                            throw new ArgumentException("Incorrect scale format in: " + layerName, cols[5]);

                        sprites.Add(new Sprite(rectangle));
                    }
                    else if (cols.Length == 6 || cols.Length == 7)
                    {
                        var position = new Vector2();
                        float scale = 1.0f;
                        var xcor = true;
                        var ycor = true;

                        if (cols[4] == "$")
                        {
                            xcor = false;
                            position.X = GetRandomCoordinate();
                        }
                        else if (!float.TryParse(cols[4], out position.X))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[4]);
                        if (xcor)
                            position.X += incX * i;

                        if (cols[5] == "$")
                        {
                            ycor = false;
                            position.Y = GetRandomCoordinate();
                        }
                        else if (!float.TryParse(cols[5], out position.Y))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[5]);
                        if (ycor)
                            position.Y += incY * i;

                        if (cols.Length == 7)
                            if (!float.TryParse(cols[6], out scale))
                                throw new ArgumentException("Incorrect scale format in: " + layerName, cols[6]);

                        sprites.Add(new Sprite(rectangle, position, scale));
                    }
                    layers.Add(new Layer(camera, sprites, parallax, LayerState));
                }
                cycles = 1;
            }
            return new Tuple<Dictionary<LevelState, Texture2D>, List<Layer>, Rectangle>(textures, layers, limits);
        }

        public float GetRandomCoordinate()
        {
            return 1;
        }

        public float GetRandomCoordinate(int limit)
        {
            return 1;
        }

        public float GetRandomCoordinate(int minimal, int limit)
        {
            return 1;
        }
    }
}
