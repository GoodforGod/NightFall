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
using CyberCommando.Animations;
using CyberCommando.Engine;

namespace CyberCommando.Services
{
    /// <summary>
    /// Contain methods to load level
    /// </summary>
    class LoadManager
    {
        private static LoadManager _Instance;
        public static LoadManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new LoadManager();
                return _Instance;
               
            }
        }

        ContentManager  Content { get; set; }
        Camera          Cam { get; set; }

        private LoadManager() { }

        public void Initialize(ContentManager content, Viewport viewport)
        {
            this.Cam = new Camera(viewport);
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
        public Tuple<Dictionary<LevelState, Texture2D>, 
                        Dictionary<LevelState, Layer>, 
                        List<LightSpot>, 
                        Rectangle> LoadLevel(string layerName, GraphicsDevice graphdev)
        {
            LevelState tState;
            LevelState lState = LevelState.NONE;
            Rectangle limits = new Rectangle(0, 0, 1280, 720);

            var lights = new List<LightSpot>();
            var layers = new Dictionary<LevelState, Layer>();
            var textures = new Dictionary<LevelState, Texture2D>();
            var sprites = new List<Sprite>();

            string textureName = "";
            Texture2D texture = Content.Load<Texture2D>("q");

            var LColor = Color.White;
            var parallax = new Vector2();

            var dataFile = Path.Combine(Content.RootDirectory, layerName + ".txt");
            var dataFileLines = File.ReadAllLines(dataFile);
            var cycles = 1;
            var innerCycles = cycles;
            var xInc = 0;
            var yInc = 0;

            var rand = new Random(Guid.NewGuid().GetHashCode());

            // Line starts with #, is comment
            foreach (var cols in from row in dataFileLines
                                 where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                                 select row.Split(';'))
            {
                innerCycles = cycles;
                for (int step = 0; step < innerCycles; step++)
                {
                    if (cycles > 1)
                        cycles--;

                    if (cols[0] == "!")
                    {
                        if (!Enum.TryParse(cols[1], true, out tState))
                            throw new ArgumentException("Incorrect TextureState format in: " + layerName, cols[0]);
                        for (int i = 1; i < cols.Length - 1; i++)
                        {
                            if (textureName != cols[i + 1])
                            {
                                textureName = cols[i + 1];
                                texture = Content.Load<Texture2D>(cols[i + 1]);
                            }
                            textures.Add(tState, texture);
                        }

                        continue;
                    }
                    else if (cols[0] == "^")
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
                        if (!int.TryParse(cols[2], out xInc))
                            throw new ArgumentException("Incorrect incX format in: " + layerName, cols[2]);
                        if (!int.TryParse(cols[3], out yInc))
                            throw new ArgumentException("Incorrect incY format in: " + layerName, cols[3]);
                        continue;
                    }
                    else if(cols[0] == "@")
                    {
                        var position = new Vector2();
                        var state = ShadowMapSize.Size256;
                        var colorLight = Color.White;

                        if (!float.TryParse(cols[1], out position.X))
                            throw new ArgumentException("Incorrect position format in: " + layerName, cols[1]);
                        if (!float.TryParse(cols[2], out position.Y))
                            throw new ArgumentException("Incorrect position format in: " + layerName, cols[2]);
                        if (!Enum.TryParse(cols[3], true, out state))
                            throw new ArgumentException("Incorrect shadowmapsize format in: " + layerName, cols[3]);

                        position.X += xInc * step;
                        position.Y += yInc * step;

                        switch (rand.Next(0, 9))
                        {
                            case 0: colorLight = Color.Tomato; break;
                            case 1: colorLight = Color.LightPink; break;
                            case 2: colorLight = Color.LightGreen; break;
                            case 3: colorLight = Color.LightGoldenrodYellow; break;
                            case 4: colorLight = Color.Blue; break;
                            case 5: colorLight = Color.White; break;
                            case 6: colorLight = Color.MediumPurple; break;
                            case 7: colorLight = Color.SeaGreen; break;
                            case 8: colorLight = Color.Orange; break;
                            case 9: colorLight = Color.LightCyan; break;
                            default: colorLight = Color.LightGreen; break;
                        }

                        lights.Add(new LightSpot(graphdev, state, position, colorLight));

                        continue;
                    }
                    else if (cols[0] == "*")
                    {
                        Rectangle rect;

                        try
                        {
                            rect = new Rectangle(
                            int.Parse(cols[2]),
                            int.Parse(cols[3]),
                            int.Parse(cols[4]),
                            int.Parse(cols[5]));
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Inccorect rectangle special format in: " + layerName, cols[0]);
                        }
                        ServiceLocator.Instance.LVLManager.SSources.Add(cols[1], rect);

                        continue;
                    }
                    else if (cols[0] == "+")
                    {
                        if (sprites != null && sprites.Count != 0)
                            layers.Add(lState, new Layer(Cam, sprites, lState, LColor, parallax));

                        var x = .0f;
                        var y = .0f;

                        if (!Enum.TryParse(cols[1], true, out lState))
                            throw new ArgumentException("Incorrect LayerState format in: " + layerName, cols[1]);
                        if (!float.TryParse(cols[2], out x))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[2]);
                        if (!float.TryParse(cols[3], out y))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[3]);

                        var r = 255;
                        var g = 255;
                        var b = 255;
                        var alpha = 1.0f;

                        if (!int.TryParse(cols[4], out r))
                            throw new ArgumentException("Incorrect color Red format in: " + layerName, cols[4]);
                        if (!int.TryParse(cols[5], out g))
                            throw new ArgumentException("Incorrect color Green format in: " + layerName, cols[5]);
                        if (!int.TryParse(cols[6], out b))
                            throw new ArgumentException("Incorrect color Blue format in: " + layerName, cols[6]);
                        if (!float.TryParse(cols[7], out alpha))
                            throw new ArgumentException("Incorrect color Alpha format in: " + layerName, cols[7]);

                        LColor = new Color(new Color(r, g, b), alpha);
                        parallax = new Vector2(x, y);
                        sprites = new List<Sprite>();
                        continue;
                    }
                    else if (cols.Length != 5 && cols.Length != 7)
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

                        if (cols[4] == "~")
                        {
                            xcor = false;
                            position.X = GetRandomCoordinate();
                        }
                        else if (!float.TryParse(cols[4], out position.X))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[4]);
                        if (xcor)
                            position.X += xInc * step;

                        if (cols[5] == "~")
                        {
                            ycor = false;
                            position.Y = GetRandomCoordinate();
                        }
                        else if (!float.TryParse(cols[5], out position.Y))
                            throw new ArgumentException("Incorrect parallax format in: " + layerName, cols[5]);
                        if (ycor)
                            position.Y += yInc * step;

                        if (cols.Length == 7)
                            if (!float.TryParse(cols[6], out scale))
                                throw new ArgumentException("Incorrect scale format in: " + layerName, cols[6]);

                        sprites.Add(new Sprite(rectangle, position, scale));
                    }
                }
            }
            layers.Add(lState, new Layer(Cam, sprites, lState, LColor, parallax));
            return new Tuple<Dictionary<LevelState, Texture2D>, Dictionary<LevelState, Layer>, List<LightSpot>, Rectangle>(textures, layers, lights, limits);
        }

        public LevelNames LoadLevelName()
        {
            return LevelNames.CYBERTOWN;
        }

        private float GetRandomCoordinate()
        {
            return 1;
        }

        private float GetRandomCoordinate(int limit)
        {
            return 1;
        }

        private float GetRandomCoordinate(int minimal, int limit)
        {
            return 1;
        }

        /// <summary>
        /// Uses .txt file in the ContentRoot directory to load up all animations <see cref="Animation"/>
        /// </summary>
        /// <typeparam name="TEnum">
        /// Represent flexible enum for animations states 
        /// (each animation type could have specific Enum State for it) <see cref="AnimationState"/>
        /// </typeparam>
        /// <param name="spritesheetName">
        /// Name of the sprite, so animation texture/sprite and file for that animation, should have same name
        /// </param>
        /// <returns>
        /// Collection where each state like <see cref="AnimationState"/> represents
        /// with the specific Animation for it <see cref="Animation"/>
        /// </returns>
        public Dictionary<TEnum, Animation> LoadAnimations<TEnum>(string spritesheetName)
             where TEnum : struct, IConvertible
        {
            //var type = Type.GetType(keyType);
            TEnum state = default(TEnum);

            var animationCollection = new Dictionary<TEnum, Animation>();
            var animation = new Animation();

            // Uses animation texture/sprite name and changes the file extention with .txt
            var dataFile = Path.Combine(Content.RootDirectory, Path.ChangeExtension(spritesheetName, "txt"));
            var dataFileLines = File.ReadAllLines(dataFile);

            // Select all NonNullable rows and rows starting not with symbol '#', separate rows by symbol ';'
            // Line starts with # is comment, 
            // 2 cols = new Animation, 
            // 7 cols = Frame in Animation
            foreach (var cols in from row in dataFileLines
                                 where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                                 select row.Split(';'))
            {
                if (cols.Length == 2)
                {
                    if (animation.FrameList.Count != 0)
                        animationCollection.Add(state, animation);
                    animation = new Animation();

                    continue;
                }
                else if (cols.Length != 7)
                    throw new InvalidDataException("Incorrect format data in file: " + spritesheetName);

                if (!Enum.TryParse(cols[0], true, out state))
                    throw new ArgumentException("Incorrect name format in: " + spritesheetName, cols[0]);

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
                    throw new Exception("Inccorect rectangle format in: " + spritesheetName, ex);
                }

                double duration;
                if (!double.TryParse(cols[5], out duration))
                    throw new ArgumentException("Inccorect duration format in: " + spritesheetName, cols[5]);

                int direction;
                if (!int.TryParse(cols[6], out direction))
                    throw new ArgumentException("Inccorect direction format in: " + spritesheetName, cols[6]);

                var effect = Convert.ToBoolean(direction);

                animation.AddFrame(rectangle, TimeSpan.FromSeconds(duration));
            }

            animationCollection.Add(state, animation);

            return animationCollection;
        }
    }
}
