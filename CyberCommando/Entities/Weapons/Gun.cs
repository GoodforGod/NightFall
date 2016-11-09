using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities.Enviroment;
using CyberCommando.Animations;

namespace CyberCommando.Entities.Weapons
{

    /// <summary>
    /// All possible guns and projectiles in the game
    /// </summary>
    public enum GunState
    {
        NONE,
        LASER_BULLET,
        LASER_IDLE,
        LASER_FIRE,
        PISTOL_BULLET,
        PISTOL_IDLE,
        PISTOL_FIRE,
        RIFLE_BILLET,
        RIFLE_IDLE,
        RIFLE_FIRE,
        SHOTGUN_BULLET,
        SHOTGUN_IDLE,
        SHOTGUN_FIRE,
        MINIGUN_BULLET,
        MINIGUN_IDLE,
        MINIGUN_FIRE,
    }

    /// <summary>
    /// Represents gun entity in game (not quite implemented yet)
    /// </summary>
    class Gun 
    {
        private readonly string SpriteSheetName = "gun-sprite-2";

        Texture2D SpriteSheet;

        public Vector2 WorldPosition { get; set; }
        public float Angle { get; set; }

        private AnimationManager<GunState> AniManager;

        public Gun(World world) 
        {
            AniManager = new AnimationManager<GunState>();
            SpriteSheet = world.CoreGame.Content.Load<Texture2D>(SpriteSheetName);
            AniManager.LoadAnimations(world.Services.LManager, SpriteSheetName);
        }

        public void Fire(Vector2 vector)
        {
            /*
            var bullet = new Projectile(world, WorldPosition, Angle);
            bullet.speed = vector;
            bullets.Add(bullet);
            */
        }

        public void Update(GameTime gameTime)
        {
            /*
            MouseState state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
                Fire(new Vector2(state.X - WorldPosition.X, state.Y - WorldPosition.Y));
            
            foreach(var bullet in bullets)
            {
                bullet.Update(gameTime);
                if (!bullet.IsOnScreen())
                    bullet.Kill(bullet);
            }
            */
        }

        public void Draw(GameTime gameTime, SpriteBatch batcher)
        {
            /*
            foreach(var bullet in bullets)
            {
                batcher.Draw(SpriteSheet,
                                bullet.WorldPosition,
                                AniManager.Animations[GunState.LASER_BULLET].CurrentRectangle,
                                Color.White,
                                bullet.Angle,
                                new Vector2(1, 1),
                                bullet.Scale,
                                Direction,
                                .0f);
            }
            */
        }
    }
}
