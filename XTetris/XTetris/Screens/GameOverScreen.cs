using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Screens
{
    class GameOverScreen : GameScreen
    {
        public GameplayScreen Screen { get; private set; }

        public SpriteFont GameOverFont { get; private set; }

        public InputAction MainMenuAction { get; private set; }

        public GameOverScreen(GameplayScreen screen)
        {
            Screen = screen;
            MainMenuAction = new InputAction(null, new Keys[] { Keys.Enter }, true);
        }

        public override void Activate(bool instancePreserved)
        {
            GameOverFont = ScreenManager.Content.Load<SpriteFont>(@"Fonts\GameOver");
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // Return to main menu
            if (MainMenuAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Rectangle bounds = new Rectangle(50, 50, TetrisGame.ScreenWidth - 100, TetrisGame.ScreenHeight - 100);

            spriteBatch.Begin();

            spriteBatch.Draw(ScreenManager.BlankTexture,
                bounds,
                new Color(10, 10, 10, 200));

            const string gameOverText = "GAME OVER";
            Vector2 textSize = GameOverFont.MeasureString(gameOverText);

            spriteBatch.DrawString(GameOverFont, gameOverText,
                new Vector2(
                    TetrisGame.ScreenWidth/2f - textSize.X/2f,
                    TetrisGame.ScreenHeight * 0.15f),
                Color.Red);

            string text = "Score: " + Screen.Player.Score;
            textSize = Screen.GameFont.MeasureString(text);

            spriteBatch.DrawString(Screen.GameFont, text, new Vector2(TetrisGame.ScreenWidth/2f - textSize.X/2f, 300), Color.White);

            text = "Level: " + Screen.Player.Level;
            textSize = Screen.GameFont.MeasureString(text);

            spriteBatch.DrawString(Screen.GameFont, text, new Vector2(TetrisGame.ScreenWidth/2f - textSize.X/2f, 400), Color.White);

            text = "Time: " + Screen.Board.TotalTime.ToString("N1") + " s";
            textSize = Screen.GameFont.MeasureString(text);

            spriteBatch.DrawString(Screen.GameFont, text, new Vector2(TetrisGame.ScreenWidth/2f - textSize.X/2f, 500), Color.White);

            float scale = 0.5f;
            text = "Press ENTER to return to the main menu.";
            textSize = Screen.GameFont.MeasureString(text);
            Vector2 textPos = new Vector2(
                bounds.Right - textSize.X * scale - 5,
                bounds.Bottom - textSize.Y * scale);

            spriteBatch.DrawString(Screen.GameFont, text, textPos, Color.Gray, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            
            spriteBatch.End();
        }
    }
}
