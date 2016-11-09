using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Services;
using CyberCommando.Entities.Weapons;
using CyberCommando.Engine;

namespace CyberCommando.Entities.Enviroment
{
    /// <summary>
    /// Represents ingame world
    /// </summary>
    class World
    {
        public readonly Game CoreGame;

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        private readonly string level_one_name = "cybertown";

        public readonly float Gravity = 9.8f;
        public readonly int WorldOffset = 20;
        float CameraShadowEntityDepth = 1.05f;

        public float ResScale { get; private set; }

        /// <summary>
        /// All live entities in the world
        /// </summary>
        List<Entity> Entities = new List<Entity>();
        /// <summary>
        /// Entities whome will be killed
        /// </summary>
        List<Entity> EntitiesToKill = new List<Entity>();
        /// <summary>
        /// Entities whome will be spawn in game
        /// </summary>
        List<Tuple<string, Vector2, float, Vector2>> EntitiesToAdd = new List<Tuple<string, Vector2, float, Vector2>>();

        Level Level;
        internal Rectangle LevelLimits { get; private set; }
        internal Rectangle LevelLimitsOrigin { get; private set; }
        internal Character Player { get; private set; }
        internal ServiceLocator Services { get; }
        public ResolutionState ResolutionCurr { get; private set; }

        public World(Game game, int height, int width, Viewport viewport)
        {
            this.CoreGame = game;

            ResScale = 1f;
            FrameWidth = width;
            FrameHeight = height;

            ServiceLocator.Instance.Initialize(game.Content, game.GraphicsDevice, width, height);
            Services = ServiceLocator.Instance;

            Level = new Level(Services.PLManager.NLevel_1, Services.LManager);
            LevelLimits = LevelLimitsOrigin = Level.Limits;
            Services.Camera.Limits = LevelLimits;
        }

        public virtual void Initialize() { Player = (Character) Spawn(typeof(Character).FullName); }

        public virtual Entity Spawn(string className) { return Spawn(className, Vector2.Zero); }
        public virtual Entity Spawn(string className, Vector2 position)
        {
            var prms = new object[] { this };
            if (className == typeof(Projectile).FullName)
                prms = new object[] { this, position, GunState.LASER_BULLET, Services.PLManager.SGun, new Rectangle(652, 102, 100, 184) };
            var entity = (Entity)Activator.CreateInstance(Type.GetType(className), prms);
            entity.WPosition = position;
            entity.Handler = Services.IOHandler;
            Entities.Add(entity);
            return entity;
        }
        public virtual Entity Spawn(string className, Vector2 position, float angle)
        {
            var entity = Spawn(className, position);
            entity.Angle = angle;
            return entity; 
        }
        public virtual Entity Spawn(string className, Vector2 position, float angle, Vector2 velocity)
        {
            var entity = Spawn(className, position);
            entity.Angle = angle;
            entity.CVelocity = velocity;
            return entity; 
        }

        public void AddToSpawnQueue(string className)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float, Vector2>(className, Vector2.One, .0f, Vector2.Zero));
        }
        public void AddToSpawnQueue(string className, Vector2 position)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float, Vector2>(className, position, .0f, Vector2.Zero));
        }
        public void AddToSpawnQueue(string className, Vector2 position, float angle)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float, Vector2>(className, position, angle, Vector2.Zero));  
        }
        public void AddToSpawnQueue(string className, Vector2 position, float angle, Vector2 velocity)
        {
            EntitiesToAdd.Add(new Tuple<string, Vector2, float, Vector2>(className, position, angle, velocity));  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void UpdateResolution(int width, int height, ResolutionState res)
        {
            switch (ResolutionCurr = res)
            {
                case ResolutionState.R1280x720: ResScale = 1; break;
                case ResolutionState.R1600x900: ResScale = 1.25f; break;
                case ResolutionState.R1920x1080: ResScale = 1.5f; break;
            }

            foreach (var entity in Entities)
                entity.ResScale = ResScale * entity.Scale;
            Level.LayersUpdateScale(ResScale);

            FrameWidth = width;
            FrameHeight = height;
            var frameW = width * 0.5f;
            var frameH = height * 0.5f;
            Services.Camera.Origin = new Vector2(frameW, frameH);
            Services.Camera.viewport = CoreGame.GraphicsDevice.Viewport;
            Level.LayersUpdateCameras(new Vector2(frameW, frameH), CoreGame.GraphicsDevice.Viewport);
        }

        public void Spawner()
        {
            MouseState state = Mouse.GetState();

            // Spawn all entities in the list
            foreach (var entity in EntitiesToAdd)
            {
                var ent = Spawn(entity.Item1, entity.Item2, entity.Item3, entity.Item4);
                if (ent is Projectile)
                    ent.CVelocity = new Vector2(state.X - Player.DPosition.X,
                                                        state.Y - Player.DPosition.Y);
            }
            // Wipe list
            EntitiesToAdd.Clear();
        }

        public virtual void Update(GameTime gameTime)
        {
            // Check for the need in spawner
            if (EntitiesToAdd.Count != 0)
                Spawner();
            // Updates all entities in world
            foreach (var entity in Entities)
                entity.Update(gameTime);

            CamInput();

            // Move camera position
            Services.Camera.LookAt(Player.WPosition);
            Level.LayersLookAt(Player.WPosition);
            Level.LayersUpdate((int)Player.WPosition.X, FrameWidth / 2 + FrameWidth / (int)ResolutionCurr);

            // Collide all entities
            foreach (var a in Entities)
            {
                foreach (var b in Entities)
                {
                    if (!ReferenceEquals(a, b))
                    {
                        if (a.BoundingBox.Intersects(b.BoundingBox))
                        {
                            a.Touch(b);
                        }
                    }
                }
            }
            
            Entities.RemoveAll(e => EntitiesToKill.Contains(e));
            EntitiesToKill.Clear();
        }
        
        public void CamInput()
        {
            KeyboardState kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Y))
            {
                Services.Camera.Zoom += 0.003f;
                Level.LayersZoom(Services.Camera.Zoom);
            }
            if (kstate.IsKeyDown(Keys.U))
            {
                Services.Camera.Zoom -= 0.003f;
                Level.LayersZoom(Services.Camera.Zoom);
            }
            if (kstate.IsKeyDown(Keys.R))
            {
                Services.Camera.Zoom = 1.0f;
                Level.LayersZoom(Services.Camera.Zoom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Kill(Entity entity) { EntitiesToKill.Add(entity); }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DrawLevelBackground(GameTime gameTime, SpriteBatch batcher)
        {
            Level.DrawBackground(batcher, Player.WPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DrawLevelFrontground(GameTime gameTime, SpriteBatch batcher)
        {
            Level.DrawFrontground(batcher, Player.WPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawLevelEntities(GameTime gameTime, SpriteBatch batcher)
        {
            foreach (var entity in Entities)
            {
                if (entity.GetType() != typeof(Character))
                    entity.Draw(gameTime, batcher);
            }

            Level.EndDrawFrontground(batcher);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DrawCharacter(GameTime gameTime, SpriteBatch batcher)
        {
            batcher.Begin(SpriteSortMode.Deferred,
                                null, null, null, null, null,
                                Services.Camera.GetViewMatrix(Vector2.One));

            Player.Draw(gameTime, batcher);

            batcher.End();
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<LightSpot> GetLevelLights()
        {
            return Level.Lights;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightArea"></param>
        /// <param name="color"></param>
        public virtual void DrawEntitiesShadowCasters(GameTime gameTime, SpriteBatch batcher, LightSpot lightArea, Color color)
        {
            Services.Camera.Zoom = CameraShadowEntityDepth;

            batcher.Begin(SpriteSortMode.Deferred,
                                null, null, null, null, null,
                                Services.Camera.GetViewMatrix(Vector2.One));

            foreach (var entity in Entities)
            {
                entity.DrawRelativelyToLightSpot(gameTime, batcher, lightArea, color);
            }

            batcher.End();

            Services.Camera.Zoom = 1.0f;
        }
    }
}
