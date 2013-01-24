using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Engine;

namespace XTetris.GameStates
{
    public class GameOverState : BaseGameState
    {
        public SpriteFont GameOverFont { get; private set; }

        public GameOverState(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameOverFont = Content.Load<SpriteFont>(@"Fonts\GameOver");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.GraphicsDevice.Clear(new Color(50, 50, 50));

            //string text = "GAME OVER";
            string text = "GAME OVER";

            Vector2 stringSize = GameOverFont.MeasureString(text);

            var pos = new Vector2(
                TetrisGame.ScreenWidth/2f - stringSize.X/2f,
                TetrisGame.ScreenHeight/8f);

            SpriteBatch.Begin();
            SpriteBatch.DrawString(GameOverFont, text, pos, new Color(206, 0, 0));
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
