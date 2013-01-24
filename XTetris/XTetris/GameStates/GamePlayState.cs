using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Engine;

namespace XTetris.GameStates
{
    public class GamePlayState : BaseGameState
    {
        public SpriteFont DebugFont { get; private set; }

        public Texture2D BorderTexture { get; private set; }
        public Texture2D BlockTexture { get; private set; }
        public Texture2D EmptyBlockTexture { get; private set; }
        public Texture2D FillTexture { get; private set; }
        public Texture2D CancelTexture { get; private set; }

        public Board Board { get; private set; }

        public GamePlayState(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            Board = new Board(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            BorderTexture = Content.Load<Texture2D>(@"Textures\border");
            BlockTexture = Content.Load<Texture2D>(@"Textures\block");
            EmptyBlockTexture = Content.Load<Texture2D>(@"Textures\empty");

            FillTexture = Content.Load<Texture2D>(@"Textures\fill");

            CancelTexture = Content.Load<Texture2D>(@"Textures\cancel");

            DebugFont = Content.Load<SpriteFont>(@"Fonts\Debug");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Board.Update(gameTime);

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
