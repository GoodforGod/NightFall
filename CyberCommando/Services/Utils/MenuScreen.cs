using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CyberCommando.Services.Utils
{
    public enum MenuState
    {
        NEW_GAME,
        LOAD_GAME,
        OPTIONS,
        EXIT
    }

    /// <summary>
    /// 
    /// </summary>
    class MenuScreen : Screen
    {
        private MenuState State { get; set; }
        private ResolutionState ResolutionCurrent { get; set; }

        private KeyboardState KState;

        private Dictionary<MenuState, List<Vector2>> MenuVectors;
        //private Texture2D Sprite;
        private SpriteFont Font;
        private bool Direction;

        private string menuLable;
        private string optText;

        private Vector2 MCamPosition;
        private LevelManager LVLManager;

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);
            Direction = false;
            LVLManager = LevelManager.Instance;
            MCamPosition = new Vector2(GraphDev.Viewport.Width / 2, GraphDev.Viewport.Height / 2);
            State = MenuState.NEW_GAME;
            menuLable = State.ToString();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            //Sprite = Content.Load<Texture2D>(ServiceLocator.Instance.PLManager.NMenu);

            LVLManager.Initialize();
            LVLManager.LoadLevel(LevelNames.MENU);

            Font = Content.Load<SpriteFont>(ServiceLocator.Instance.PLManager.NTitleFont);

            MenuVectors = new Dictionary<MenuState, List<Vector2>>() {
                { MenuState.NEW_GAME, new List<Vector2>()
                {// M
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(0,2), new Vector2(0,3), new Vector2(0,4),
                new Vector2(1, 1),
                 new Vector2(2, 0), new Vector2(2, 1), new Vector2(2,2), new Vector2(2,3), new Vector2(2,4),
                 // E
                  new Vector2(4, 0), new Vector2(4, 1), new Vector2(4,2), new Vector2(4,3), new Vector2(4,4),
                new Vector2(5, 0), new Vector2(5, 2), new Vector2(5, 4),
                new Vector2(6, 0), new Vector2(6, 2), new Vector2(6, 4),
                 // N
                  new Vector2(8, 0), new Vector2(8, 1), new Vector2(8,2), new Vector2(8,3), new Vector2(8,4),
                new Vector2(9, 2),
                new Vector2(10, 3),
                 new Vector2(11, 0), new Vector2(11, 1), new Vector2(11,2), new Vector2(11,3), new Vector2(11,4),
                    //U
                  new Vector2(13, 0), new Vector2(13, 1), new Vector2(13,2), new Vector2(13,3), 
                new Vector2(14, 4),
                new Vector2(15, 4),
                 new Vector2(16, 0), new Vector2(16, 1), new Vector2(16,2), new Vector2(16,3)
                } },

                { MenuState.LOAD_GAME, new List<Vector2>() {

                } },

                { MenuState.OPTIONS, new List<Vector2>() { } },

                { MenuState.EXIT, new List<Vector2>() { }}
            };
        }

        public override void Resize(ResolutionState res, int width, int height)
        {
            base.Resize(res, width, height);

            LVLManager.UpdateScale(SScale, new Vector2(width, height), CoreGame.GraphicsDevice.Viewport);
        }

        private void ChangeState(bool up)
        {
            switch (State)
            {
                case MenuState.NEW_GAME:
                    if (!up)
                        State = MenuState.LOAD_GAME;
                    else State = MenuState.EXIT;
                    break;
                case MenuState.LOAD_GAME:
                    if (!up)
                        State = MenuState.OPTIONS;
                    else State = MenuState.NEW_GAME;
                    break;
                case MenuState.OPTIONS:
                    if (!up)
                        State = MenuState.EXIT;
                    else State = MenuState.LOAD_GAME;
                    break;
                case MenuState.EXIT:
                    if (!up)
                        State = MenuState.NEW_GAME;
                    else State = MenuState.OPTIONS;
                    break;
            }
            menuLable = State.ToString();
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
                switch (State)
                {
                    case MenuState.NEW_GAME:
                        ScreenManager.Instance.SwitchScreen(ScreenState.Game, "CYBERTOWN");
                        break;
                    case MenuState.LOAD_GAME:
                        break;
                    case MenuState.OPTIONS:
                        break;
                    case MenuState.EXIT:
                        ScreenManager.Instance.Exit();
                        break;
                }
            }

            KState = currentKState;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateState();
            if (Direction)
            {
                if (MCamPosition.X < FWidth / 2)
                    Direction = !Direction;
                MCamPosition.X -= 0.9f;
            }
            else
            {
                if (MCamPosition.X > FWidth / 1.5)
                    Direction = !Direction;
                MCamPosition.X += 0.9f;
            }

            LVLManager.Update(MCamPosition, (int)MCamPosition.X, FWidth / 2);
        }

        public override void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            LVLManager.DrawBack(batcher);
            LVLManager.DrawFront(batcher, new Vector2(MCamPosition.X + FWidth / 2, MCamPosition.X - FWidth / 2));
            batcher.DrawString(Font, menuLable, MCamPosition, Color.White);
            LVLManager.EndDrawFront(batcher);
        }
    }
}
