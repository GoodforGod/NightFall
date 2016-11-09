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
        double time;
        double startTime = 1.03;

        public override void Initialize(GraphicsDevice graphdev, Game game)
        {
            base.Initialize(graphdev, game);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            Font = Content.Load<SpriteFont>("f-test");
            scale = 1.0f;
            Begin = DateTime.Now;
        }

        public override void UnloadContent()
        {
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            scale += 0.05f;
            time = (DateTime.Now - Begin).TotalSeconds;
            if (time > startTime)
                ScreenManager.Instance.SwitchScreen(ScreenState.Game);

        }

        public override void Draw(SpriteBatch batcher, GameTime gameTime)
        {
            batcher.Begin();
            batcher.DrawString(Font,
                                "Titles\nStart in " + (int)time, 
                                new Vector2(100, 100), 
                                Color.Green, 
                                .0f, 
                                Vector2.One, 
                                scale, 
                                SpriteEffects.None, 
                                0.0f);
            batcher.End();
        }
    }
}
