using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            if (InputHandler.KeyPressed(Keys.Enter))
            {
                GameRef.GamePlayState.NewGame();
                GameRef.StateManager.ChangeState(GameRef.GamePlayState);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.GraphicsDevice.Clear(Board.BackgroundColor);

            const string gameOverText = "GAME OVER";

            Vector2 stringSize = GameOverFont.MeasureString(gameOverText);

            var pos = new Vector2(
                TetrisGame.ScreenWidth/2f - stringSize.X/2f,
                TetrisGame.ScreenHeight/8f);

            SpriteBatch.Begin();

            var board = GameRef.GamePlayState.Board;

            // Draw game board
            board.DrawCells(SpriteBatch);
            
            SpriteBatch.Draw(GameRef.GamePlayState.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);

            SpriteBatch.Draw(GameRef.GamePlayState.FillTexture, GameRef.ScreenRectangle, new Color(0, 0, 0, 245));

            SpriteBatch.DrawString(GameOverFont, gameOverText, pos, new Color(206, 0, 0));

            string text = "Score: " + board.Player.Score;
            Vector2 textSize = GameRef.GamePlayState.GameFont.MeasureString(text);
            SpriteBatch.DrawString(GameRef.GamePlayState.GameFont, text, new Vector2(TetrisGame.ScreenWidth/2f - textSize.X/2f, 250), Color.White);

            text = "Level: " + board.Player.Level;
            textSize = GameRef.GamePlayState.GameFont.MeasureString(text);
            SpriteBatch.DrawString(GameRef.GamePlayState.GameFont, text, new Vector2(TetrisGame.ScreenWidth / 2f - textSize.X / 2f, 350), Color.White);

            text = "Time: " + board.TotalTime.ToString("N1") + " s";
            textSize = GameRef.GamePlayState.GameFont.MeasureString(text);
            SpriteBatch.DrawString(GameRef.GamePlayState.GameFont, text, new Vector2(TetrisGame.ScreenWidth / 2f - textSize.X / 2f, 450), Color.White);

            float scale = 0.4f;
            text = "Press ENTER to restart the game.";
            textSize = GameRef.GamePlayState.GameFont.MeasureString(text);
            Vector2 restartPos = new Vector2(
                TetrisGame.ScreenWidth - (textSize.X * scale),
                TetrisGame.ScreenHeight - (textSize.Y * scale));
            SpriteBatch.DrawString(GameRef.GamePlayState.GameFont, text, restartPos, new Color(200, 200, 200), 0f, Vector2.Zero, scale,
                                   SpriteEffects.None, 0f);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
