using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CyberCommando.Controllers;
using CyberCommando.Animations;
using CyberCommando.Services;

namespace CyberCommando.Entities
{
    public class World
    {
        public readonly Game Game;

        public readonly int FrameWidth;
        public readonly int FrameHeight;

        private readonly static string level_1 = "level-1";

        public readonly float Gravity = 9.8f;
        public int WorldOffset = 20;

        List<Entity> Entities = new List<Entity>();
        List<Entity> EntitiesToKill = new List<Entity>();

        Level level;
        Camera camera;
        Character character;
        float paral = 0.001f;

        internal LayerLoader LayLoader { get; } 
        internal AnimationLoader AniLoader { get; }
        InputHandler handler = new InputHandler();

        public World(Game game, int height, int width, Viewport viewport)
        {
            Game = game;

            FrameWidth = width;
            FrameHeight = height;

            LayLoader = new LayerLoader(game.Content, viewport);
            AniLoader = new AnimationLoader(game.Content.RootDirectory);
            level = new Level(level_1, LayLoader);

            camera = new Camera(viewport);
            camera.Position = new Vector2(0f, FrameHeight / 2);
            camera.Zoom = 1.0f;

        }

        public virtual void Initialize()
        {
            character = (Character) Spawn(typeof(Character).FullName);
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

            camera.LookAt(character.Position);
            level.LayersLookAt(character.Position);

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
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,

            level.Draw(batcher);

            batcher.Begin(SpriteSortMode.Deferred,
                            null, null, null, null, null,
                            camera.GetViewMatrix(new Vector2(0.5f)));

            foreach (var entity in Entities)
            {
                entity.Draw(gameTime, batcher);
            }

            batcher.End();
        }
    }
}
