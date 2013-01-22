using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.Tetrominoes;

namespace XTetris.GameStates
{
    public class GamePlayState : BaseGameState
    {
        private TTetromino _t;

        private readonly Color TTetrominoColor = Color.Purple;

        public GamePlayState(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture2D texture = Content.Load<Texture2D>(@"Textures\mino");
            _t = new TTetromino(texture, TTetrominoColor);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _t.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            _t.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
