using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCommando.Entities.Enviroment;
using CyberCommando.Animations;

namespace CyberCommando.Services.Utils
{
    enum MenuAnimations
    {
        M, E, N, U, W, L, O, A, D, P, X, I, T, S, R, B, C, K, RAIN
    }

    enum MenuState
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
        int         FCamRightPos;
        int         FCamLeftPos;

        string MLable;

        AnimationManager<MenuAnimations> AniManager;

        KeyboardState   KState;
        SpriteFont      Font;
        Vector2         MCamPosition;
        LevelManager    LVLManager;

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);
            this.Direction = false;
            this.FWidthHalf = FCamLeftPos = FWidth / 2;
            this.FCamRightPos = (int) (FWidth / 1.5);

            this.LVLManager = LevelManager.Instance;
            this.MCamPosition = new Vector2(GraphDev.Viewport.Width / 2, GraphDev.Viewport.Height / 2);

            this.MState = MenuState.NEW_GAME;
            this.MLable = MState.ToString();
            this.LName = LevelNames.CYBERTOWN;
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
            LVLManager.Unload();
        }

        public override void Resize(ResolutionState res, int width, int height)
        {
            base.Resize(res, width, height);

            this.FCamRightPos = (int)(FWidth / 1.5);
            this.FCamLeftPos = FWidth / 2;

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
                if (MCamPosition.X > FCamRightPos)
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
                                    LevelState.NONE);

            LVLManager.DrawSpecific(batcher, 
                                    new Vector2(MCamPosition.X + FWidthHalf, MCamPosition.X - FWidthHalf), 
                                    LevelState.FRONT_NOT_EFFECTED, 
                                    LevelState.FRONT_NOT_EFFECTED);

            batcher.DrawString(Font, MLable, MCamPosition, Color.White);

            LVLManager.EndDrawLastLayer(batcher);
        }
    }
}
