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
using CyberCommando.Entities.Weapons;
using Microsoft.Xna.Framework.Input;

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
        List<Tuple<string, Vector2, float>> EntitiesToAdd = new List<Tuple<string, Vector2, float>>();

        Level Level;
        Character Player;

        internal ServiceLocator Services { get; }

        public World(Game game, int height, int width, Viewport viewport)
        {
            Game = game;

            FrameWidth = width;
            FrameHeight = height;

            Services = new ServiceLocator(game.Content, viewport, FrameWidth, FrameHeight);
            Level = new Level(level_1, Services.LayLoader);
        }

        public virtual void Initialize()
        {
            Player = (Character) Spawn(typeof(Character).FullName);
        }

        public virtual Entity Spawn(string className)
        {
            return Spawn(className, Vector2.Zero);
        }

        public virtual Entity Spawn(string className, Vector2 position)
        {
            var prms = new object[] { this };
            if (className == typeof(Projectile).FullName)
                prms = new object[] { this, position, GunState.LASER_BULLET, Game.Content.Load<Texture2D>("gun-sprite-2"), new Rectangle(652, 102, 100, 184) };
            var entity = (Entity)Activator.CreateInstance(Type.GetType(className), prms);
            entity.WorldPosition = position;
            entity.handler = Services.IOHandler;
            Entities.Add(entity);
            return entity;
        }

        public virtual Entity Spawn(string className, Vector2 position, float angle)
        {
            var entity = Spawn(className, position);
            entity.Angle = angle;
            return entity; 
        }

        public void AddToSpawnQueue(string className)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float>(className, Vector2.One, .0f));
        }

        public void AddToSpawnQueue(string className, Vector2 position)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float>(className, position, .0f));
        }

        public void AddToSpawnQueue(string className, Vector2 position, float angle)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float>(className, position, angle));  
        }

        public virtual void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();

            foreach (var entity in EntitiesToAdd)
            {
                var ent = Spawn(entity.Item1, entity.Item2, entity.Item3);
                ent.VelocityCurrent = new Vector2(state.X - ent.WorldPosition.X, state.Y - ent.WorldPosition.Y);
            }

            EntitiesToAdd.Clear();

            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
            }

            Services.Camera.LookAt(Player.WorldPosition);
            Level.LayersLookAt(Player.WorldPosition);

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

            Level.Draw(batcher);

            batcher.Begin(SpriteSortMode.Deferred,
                            null, null, null, null, null,
                            Services.Camera.GetViewMatrix(new Vector2(0.9f)));

            foreach (var entity in Entities)
            {
                entity.Draw(gameTime, batcher);
            }

            batcher.End();
        }
    }
}
