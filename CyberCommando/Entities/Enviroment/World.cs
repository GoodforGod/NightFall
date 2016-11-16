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
using CyberCommando.Entities.Enemies;

namespace CyberCommando.Entities.Enviroment
{
    /// <summary>
    /// Represents ingame world
    /// </summary>
    class World
    {
        public readonly Game    CoreGame;
        LevelManager            LVLManager  { get; }
        ServiceLocator          Services    { get; }

        MouseState MStatePrev;

        public int              FWidth   { get; set; }
        public int              FHeight  { get; set; }

        public readonly float   Gravity = 9.8f;
        public readonly int     WorldOffset = 20;
        float                   CameraShadowEntityDepth = 1.05f;

        public float            ResScale { get; private set; }

        /// <summary>
        /// Platforms in the world
        /// </summary>
        List<Platform>  Platforms       = new List<Platform>();

        /// <summary>
        /// All live entities in the world
        /// </summary>
        List<Entity>    Entities        = new List<Entity>();

        /// <summary>
        /// Entities whome will be killed
        /// </summary>
        List<Entity>    EntitiesToKill  = new List<Entity>();

        /// <summary>
        /// Entities whome will be spawn in game
        /// </summary>
        List<Tuple<string, Vector2, float, Vector2>> EntitiesToAdd = new List<Tuple<string, Vector2, float, Vector2>>();

        /// <summary>
        /// Collections of the lights in the world
        /// </summary>
        public List<LightSpot>  LevelLight          { get { return LVLManager.CLights; } }
        internal Rectangle      LevelLimits         { get { return LVLManager.CLimits; } }
        ResolutionState         CResolution   { get; set; }

        /// <summary>
        /// Charater entity
        /// </summary>
        internal Character Player { get; private set; }

        public World(Game game, int height, int width, Viewport viewport)
        {
            this.CoreGame = game;

            MStatePrev = Mouse.GetState();

            ResScale = 1f;
            FWidth = width;
            FHeight = height;

            Services = ServiceLocator.Instance;
            LVLManager = Services.LVLManager;

            Services.Camera.Limits = LVLManager.CLimits;

            Platforms = new List<Platform>();
            Platforms.Add(new Platform(new Vector2(), 
                                        LVLManager.CLevel.Layers[LevelState.BACKGROUND].Texture, 
                                        new Rectangle(1000, 750, 100, 40)));
        }

        public virtual void Initialize()
        {
            Player = (Character) Spawn(typeof(Character).FullName);
            Spawn(typeof(Enemy).FullName);
        }

        public virtual Entity Spawn(string className) { return Spawn(className, Vector2.Zero); }
        public virtual Entity Spawn(string className, Vector2 position)
        {
            var param = new object[] { this };

            if (className == typeof(Projectile).FullName)
                param = new object[] { this,
                                        position,
                                        GunState.LASER_BULLET,
                                        Services.PLManager.SGun,
                                        Services.PLManager.RProjectileLaser };

            var entity = (Entity)Activator.CreateInstance(Type.GetType(className), param);

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
        /// Update the resolution scale for all inworld entities/lvl/layers
        /// </summary>
        public void UpdateResolution(int width, int height, ResolutionState res)
        {
            switch (CResolution = res)
            {
                case ResolutionState.R1280x720: ResScale = 1; break;
                case ResolutionState.R1600x900: ResScale = 1.25f; break;
                case ResolutionState.R1920x1080: ResScale = 1.5f; break;
            }

            foreach (var entity in Entities)
                entity.ResScale = ResScale * entity.Scale;

            FWidth = width;
            FHeight = height;
            var frameW = width * 0.5f;
            var frameH = height * 0.5f;
            Services.Camera.Origin = new Vector2(frameW, frameH);
            Services.Camera.viewport = CoreGame.GraphicsDevice.Viewport;

            LVLManager.UpdateScale(ResScale, new Vector2(frameW, frameH), CoreGame.GraphicsDevice.Viewport);
        }

        public void Spawner()
        {
            MouseState state = Mouse.GetState();

            if(state.RightButton == ButtonState.Pressed && MStatePrev.RightButton == ButtonState.Released)
            {
                AddToSpawnQueue(typeof(Enemy).FullName);
            }
            // Spawn all entities in the list
            foreach (var entity in EntitiesToAdd)
            {
                var ent = Spawn(entity.Item1, entity.Item2, entity.Item3, entity.Item4);
                if (ent is Projectile)
                    ent.CVelocity = entity.Item4;
            }
            // Wipe list
            EntitiesToAdd.Clear();

            MStatePrev = state;

        }

        public virtual void Update(GameTime gameTime)
        {
            // Check for the need in spawner
            if (EntitiesToAdd.Count != 0)
                Spawner();

            // Updates all entities in world
            foreach (var entity in Entities)
            {
                var oldEntityPosition = entity.WPosition;
                entity.Update(gameTime);

                if (entity.IsGrounded)
                    continue;

                entity.IsOnPlatform = false;

                foreach (var platform in Platforms)
                {
                    if (entity.BoundingBox.Intersects(platform.BoundingBox))
                    {
                        if(entity.BoundingBox.Bottom > platform.BoundingBox.Top)
                        {
                            entity.IsOnPlatform = true;
                        }
                    }
                }
            }

            CamInput();

            // Move camera position
            Services.Camera.LookAt(Player.WPosition);

            /// Update lvl/Layers Cameras prespective
            LVLManager.Update(Player.WPosition, (int)Player.WPosition.X, FWidth / 2 + FWidth / (int)CResolution);

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
                            if (a is Projectile && b is Entity)
                                b.Kill(b);
                            if (b is Projectile && a is Entity)
                                a.Kill(a);
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
                LVLManager.CLevel.LayersZoom(Services.Camera.Zoom);
            }
            if (kstate.IsKeyDown(Keys.U))
            {
                Services.Camera.Zoom -= 0.003f;
                LVLManager.CLevel.LayersZoom(Services.Camera.Zoom);
            }
            if (kstate.IsKeyDown(Keys.R))
            {
                Services.Camera.Zoom = 1.0f;
                LVLManager.CLevel.LayersZoom(Services.Camera.Zoom);
            }
        }

        /// <summary>
        /// Kill entity in the world
        /// </summary>
        public virtual void Kill(Entity entity) { EntitiesToKill.Add(entity); }

        /// <summary>
        /// Draw back and middle layers of the level
        /// </summary>
        public virtual void DrawLVLBack(GameTime gameTime, SpriteBatch batcher)
        {
            LVLManager.DrawSpecific(batcher, 
                                    new Vector2(Services.Camera.Position.X + FWidth, Services.Camera.Position.X), 
                                    LevelState.BACKGROUND | LevelState.BACK | LevelState.MIDDLE, LevelState.NONE);
        }

        /// <summary>
        /// Draw front layers of the level
        /// </summary>
        public virtual void DrawLVLFront(GameTime gameTime, SpriteBatch batcher)
        {
            LVLManager.DrawSpecific(batcher, 
                                    new Vector2(Services.Camera.Position.X + FWidth, Services.Camera.Position.X - FWidth / 2), 
                                    LevelState.FRONT_NOT_EFFECTED, LevelState.FRONT_NOT_EFFECTED);
        }

        /// <summary>
        /// Draw all entities (except player) in the world
        /// </summary>
        public void DrawLVLEntities(GameTime gameTime, SpriteBatch batcher)
        {
            foreach (var entity in Entities)
            {
                if (entity.GetType() != typeof(Character))
                    entity.Draw(gameTime, batcher);
            }

            LVLManager.EndDrawLastLayer(batcher);
        }

        /// <summary>
        /// Draw Character in the world
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
        /// Draw all shadows which entities casts amount the level
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
