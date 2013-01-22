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
            BlockTexture = Content.Load<Texture2D>(@"Textures\block");

            Player.ActiveShape = ShapesFactory.CreateRandom(BlockTexture);
            Player.ActiveShape.Position = new Vector2(200, 200);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Enter))
                Player.ActiveShape = ShapesFactory.CreateRandom(BlockTexture);

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
