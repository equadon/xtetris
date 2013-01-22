using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.Tetrominoes;

namespace XTetris.GameStates
{
    public class GamePlayState : BaseGameState
    {
        public static Texture2D MinoTexture { get; private set; }

        public Player Player { get; private set; }

        public GamePlayState(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            Player = new Player();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            MinoTexture = Content.Load<Texture2D>(@"Textures\mino");

            Player.ActiveTetromino = new TTetromino(MinoTexture, TetrisGame.TTetrominoColor);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Player.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
