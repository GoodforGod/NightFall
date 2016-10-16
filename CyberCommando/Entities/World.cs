using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Entities
{
    public class World
    {
        public readonly Game GAME;

        List<Entity> Entities = new List<Entity>();
        List<Entity> EntitiesToKill = new List<Entity>();

        public World(Game game) { GAME = game; }

        public virtual void Initialize() { }

        public virtual Entity Spawn(string className) { return Spawn(className, Vector2.Zero); }
        
        public virtual Entity Spawn(string className, Vector2 position)
        {
            var prms = new object[] { this };
            var entity = (Entity)Activator.CreateInstance(Type.GetType(className), prms);
            entity.Position = position;
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
