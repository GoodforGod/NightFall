﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities.Enviroment;

namespace CyberCommando.Services.Utils
{
    public enum MenuState
    {
        MAIN,
        NEW_GAME,
        LOAD_GAME,
        OPTIONS,
        EXIT,
        SOUND,
        RESOLUTION,
        BACK
    }

    /// <summary>
    /// 
    /// </summary>
    class MenuScreen : Screen
    {
        LevelNames  LName { get; set; }
        MenuState   MState { get; set; }
        bool        IsSoundOn;
        bool        Direction;
        int         FWidthHalf;

        string MLable;

        KeyboardState   KState;
        SpriteFont      Font;
        Vector2         MCamPosition;
        LevelManager    LVLManager;

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);
            Direction = false;
            FWidthHalf = FWidth / 2;
            LVLManager = LevelManager.Instance;
            MCamPosition = new Vector2(GraphDev.Viewport.Width / 2, GraphDev.Viewport.Height / 2);
            MState = MenuState.NEW_GAME;
            MLable = MState.ToString();
            LName = LevelNames.CYBERTOWN;
        }

        private void InitMenuArray()
        {
            /*
            MArrays = new Dictionary<MenuState, int[,]>()
            {
                {MenuState.MAIN, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,1,1,0,1,0,0,1},
                    {1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,0,1,1,0}} },
                {MenuState.NEW_GAME, new int[,] {
                    {1,1,1,0,1,0,0,0,0,1,0,0,1,0,0,1},
                    {1,0,1,0,1,0,0,0,1,0,1,0,0,1,0,1},
                    {1,1,1,0,1,0,0,0,1,1,1,0,0,0,1,0},
                    {1,0,0,0,1,0,0,0,1,0,1,0,0,1,0,0},
                    {1,0,0,0,1,1,1,0,1,0,1,0,1,0,0,0}} },
                {MenuState.LOAD_GAME, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },
                {MenuState.OPTIONS, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },
                {MenuState.EXIT, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },
                {MenuState.SOUND, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },
                {MenuState.RESOLUTION, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },
                {MenuState.BACK, new int[,] {
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,1,1,0,1,0,0,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1},
                    {1,0,1,0,1,1,1,0,1,0,1,0,1,0,0,1}} },

            };
            */
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            LVLManager.Initialize();
            LVLManager.LoadLevel(LevelNames.MENU);

            Font = Content.Load<SpriteFont>(ServiceLocator.Instance.PLManager.NFTitleFont);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Resize(ResolutionState res, int width, int height)
        {
            base.Resize(res, width, height);
            LVLManager.UpdateScale(SScale, new Vector2(width, height), CoreGame.GraphicsDevice.Viewport);
        }

        private void ChangeState(bool up)
        {
            switch (MState)
            {
                case MenuState.NEW_GAME:
                    if (!up)
                        MState = MenuState.LOAD_GAME;
                    else MState = MenuState.EXIT;
                    break;
                case MenuState.LOAD_GAME:
                    if (!up)
                        MState = MenuState.OPTIONS;
                    else MState = MenuState.NEW_GAME;
                    break;
                case MenuState.OPTIONS:
                    if (!up)
                        MState = MenuState.EXIT;
                    else MState = MenuState.LOAD_GAME;
                    break;
                case MenuState.EXIT:
                    if (!up)
                        MState = MenuState.NEW_GAME;
                    else MState = MenuState.OPTIONS;
                    break;
                case MenuState.SOUND:
                    if (!up)
                        MState = MenuState.RESOLUTION;
                    else MState = MenuState.BACK;
                    break;
                case MenuState.RESOLUTION:
                    if (!up)
                        MState = MenuState.BACK;
                    else MState = MenuState.SOUND;
                    break;
                case MenuState.BACK:
                    if (!up)
                        MState = MenuState.SOUND;
                    else MState = MenuState.RESOLUTION;
                    break;
            }

            MLable = MState.ToString();

            switch (MState)
            {
                case MenuState.SOUND: MLable += " : " + IsSoundOn.ToString(); break;
                case MenuState.RESOLUTION: MLable += " : " + ResolutionCurrent.ToString(); break;
                default: break;
            }
        }

        private void UpdateState()
        {
            KeyboardState currentKState = Keyboard.GetState();

            if((currentKState.IsKeyDown(Keys.Down) && KState.IsKeyUp(Keys.Down))
                || (currentKState.IsKeyDown(Keys.S) && KState.IsKeyUp(Keys.S)))
                ChangeState(false);

            if((currentKState.IsKeyDown(Keys.Up) && (KState.IsKeyUp(Keys.Up)) 
                || (currentKState.IsKeyDown(Keys.W)) && KState.IsKeyUp(Keys.W)))
                ChangeState(true);

            if ((currentKState.IsKeyDown(Keys.D) && KState.IsKeyUp(Keys.D)) 
                || (currentKState.IsKeyDown(Keys.Right) && KState.IsKeyUp(Keys.Left)) 
                || (currentKState.IsKeyDown(Keys.Enter) && KState.IsKeyUp(Keys.Enter))
                || (currentKState.IsKeyDown(Keys.Space) && KState.IsKeyUp(Keys.Space)))
            {
                switch (MState)
                {
                    case MenuState.NEW_GAME:
                        ScreenManager.Instance.SwitchScreen(ScreenState.Game, LName);
                        break;

                    case MenuState.LOAD_GAME:
                        break;

                    case MenuState.OPTIONS:
                        MState = MenuState.SOUND;
                        MLable = MState.ToString() + " : " + IsSoundOn.ToString();
                        break;

                    case MenuState.EXIT:
                        ScreenManager.Instance.Exit();
                        break;

                    case MenuState.SOUND:
                        IsSoundOn = !IsSoundOn; 
                        MLable = MState.ToString() + " : " + IsSoundOn.ToString();
                        break;

                    case MenuState.RESOLUTION:
                        switch(ResolutionCurrent)
                        {
                            case ResolutionState.R1280x720: ResolutionCurrent = ResolutionState.R1600x900; break;
                            case ResolutionState.R1600x900: ResolutionCurrent = ResolutionState.R1920x1080; break;
                            case ResolutionState.R1920x1080: ResolutionCurrent = ResolutionState.R1280x720; break;
                        }
                        MLable = MState.ToString() + " : " + ResolutionCurrent.ToString();
                        ScreenManager.Instance.Resize(ResolutionCurrent);
                        break;

                    case MenuState.BACK:
                        MState = MenuState.OPTIONS;
                        MLable = MState.ToString();
                        break;
                }
            }
            KState = currentKState;
        }

        private void UpdateCamPosition()
        {
            if (Direction)
            {
                if (MCamPosition.X < FWidthHalf)
                    Direction = !Direction;
                MCamPosition.X -= 0.9f;
            }
            else
            {
                if (MCamPosition.X > FWidth)
                    Direction = !Direction;
                MCamPosition.X += 0.9f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateState();

            UpdateCamPosition();

            LVLManager.Update(MCamPosition, (int)MCamPosition.X, FWidthHalf);
        }

        public override void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            LVLManager.DrawSpecific(batcher, 
                                    new Vector2(MCamPosition.X + FWidthHalf, MCamPosition.X - FWidthHalf), 
                                    LevelState.BACKGROUND | LevelState.BACK | LevelState.MIDDLE, 
                                    true);
            LVLManager.DrawSpecific(batcher, 
                                    new Vector2(MCamPosition.X + FWidthHalf, MCamPosition.X - FWidthHalf), 
                                    LevelState.FRONT, 
                                    false);
            batcher.Begin();
            batcher.DrawString(Font, MLable, MCamPosition, Color.White);
            batcher.End();
            LVLManager.EndDrawFront(batcher);
        }
    }
}
