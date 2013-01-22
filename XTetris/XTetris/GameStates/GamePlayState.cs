using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.Shapes;

namespace XTetris.GameStates
{
    public class GamePlayState : BaseGameState
    {
        public static Texture2D BlockTexture { get; private set; }
        public static SpriteFont DebugFont { get; private set; }
        public Texture2D FillTexture { get; private set; }

        public Board Board { get; private set; }
        public Player Player { get; private set; }

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
            BlockTexture = Content.Load<Texture2D>(@"Textures\block");
            FillTexture = Content.Load<Texture2D>(@"Textures\fill");

            DebugFont = Content.Load<SpriteFont>(@"Fonts\Debug");

            Board = new Board(FillTexture);
            Player = new Player(Board);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Board.Update(gameTime);
            Player.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Board.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);

            SpriteBatch.End();
        }
    }
}
