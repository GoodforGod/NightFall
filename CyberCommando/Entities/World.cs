using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Controllers;
using CyberCommando.Animations;

namespace CyberCommando.Entities
{
    public class World
    {
        public readonly Game Game;

        public readonly int FrameWidth;
        public readonly int FrameHeight;

        public readonly float Gravity = 9.8f;
        public int WorldOffset = 20;

        List<Entity> Entities = new List<Entity>();
        List<Entity> EntitiesToKill = new List<Entity>();

        internal AnimationLoader AniLoader { get; }
        InputHandler handler;

        public World(Game game, int height, int width)
        {
            FrameWidth = width;
            FrameHeight = height;
            Game = game;
            AniLoader = new AnimationLoader(game.Content.RootDirectory);
            handler = new InputHandler();
        }

        public virtual void Initialize()
        {
            Spawn(typeof(Character).FullName);
        }

        public virtual Entity Spawn(string className) { return Spawn(className, Vector2.Zero); }
        
        public virtual Entity Spawn(string className, Vector2 position)
        {
            var prms = new object[] { this };
            var entity = (Entity)Activator.CreateInstance(Type.GetType(className), prms);
            entity.Position = position;
            entity.handler = handler;
            Entities.Add(entity);
            return entity;
        }
        
        public virtual void Update(GameTime gameTime)
        { 
            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
            }

            foreach (var a in Entities)
            {
                foreach (var b in Entities)
                {
                    if (!ReferenceEquals(a, b))
                    {
                        if (a.boundingBox.Intersects(b.boundingBox))
                        {
                            a.Touch(b);
                        }
                    }
                }
            }
            
            Entities.RemoveAll(e => EntitiesToKill.Contains(e));
            EntitiesToKill.Clear();
        }
        
        public virtual void Kill(Entity entity) { EntitiesToKill.Add(entity); }

        public virtual void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            foreach (var entity in Entities)
            {
                entity.Draw(gameTime, batcher);
            }
        }
    }
}
