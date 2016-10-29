﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CyberCommando.Animations
{
    internal class AnimationLoader
    {
        private string ContentRoot;

        public AnimationLoader(string contentRoot) { this.ContentRoot = contentRoot; }

        public Dictionary<TEnum, Animation> LoadAll<TEnum>(string spritesheetName)
             where TEnum : struct, IConvertible
        {
            //var type = Type.GetType(keyType);
            TEnum state = default(TEnum);

            var animationCollection = new Dictionary<TEnum, Animation>();
            var animation = new Animation();

            var dataFile = Path.Combine(ContentRoot, Path.ChangeExtension(spritesheetName, "txt"));
            var dataFileLines = File.ReadAllLines(dataFile);

            // Line starts with #, is comment, 2 cols = new animation, 7 cols = frame in animation
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
                    throw new ArgumentException("Inccorect rectangle format in: " + spritesheetName, cols[0]);
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
