using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Services.Utils
{
    /// <summary>
    /// 
    /// </summary>
    class TitleScreen : Screen
    {
        SpriteFont Font;
        DateTime Begin;
        float scale;
        string titles;
        Vector2 origin;
        Vector2 position;
        double time;
        double startTime = 2.03;

        public override void Initialize(GraphicsDevice graphdev, Game game, params object[] param)
        {
            base.Initialize(graphdev, game);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            Font = Content.Load<SpriteFont>(ServiceLocator.Instance.PLManager.N);
            scale = 1.0f;
            Begin = DateTime.Now;
            titles = "Titles\nStart in ";
            origin = new Vector2(Font.MeasureString(titles).X / 2, Font.MeasureString(titles).Y);
            position = new Vector2(GraphDev.Viewport.Width / 2, GraphDev.Viewport.Height / 2);
        }

        public override void UnloadContent()
        {
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            scale += 0.02f;
            time = (DateTime.Now - Begin).TotalSeconds;
            if (time > startTime)
                ScreenManager.Instance.SwitchScreen(ScreenState.Menu);

        }

        public override void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            batcher.Begin();
            batcher.DrawString(Font,
                                titles + (int)time, 
                                position, 
                                Color.Black, 
                                .0f, 
                                origin,
                                scale * SScale, 
                                SpriteEffects.None, 
                                0.0f);
            batcher.End();
        }
    }
}
