using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XTetris.Engine;

namespace XTetris
{
    public class Board
    {
        #region Properties & Fields

        private Vector2 _position;
        private Rectangle _screenRectangle;

        private readonly Color _backgroundColor = new Color(255, 255, 255);

        public BaseShape ActiveShape { get; private set; }

        public Texture2D Texture { get; private set; }

        public Rectangle ScreenRectangle
        {
            get { return _screenRectangle; }
            set { _screenRectangle = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool HasActiveShape
        {
            get { return ActiveShape != null; }
        }

        #endregion

        public Board(Texture2D texture)
        {
            Texture = texture;

            ScreenRectangle = new Rectangle(TetrisGame.BoardPadding/2, TetrisGame.BoardPadding/2, TetrisGame.BlockSize * 10, TetrisGame.BlockSize * 20);

            SpawnShape();
        }

        public void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Enter))
                SpawnShape();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw board
            spriteBatch.Draw(Texture, ScreenRectangle, _backgroundColor);

            // Draw active shape
            if (HasActiveShape)
                ActiveShape.Draw(gameTime, spriteBatch);
        }

        private void SpawnShape()
        {
            ActiveShape = ShapesFactory.CreateRandom(this);
            ActiveShape.Position = new Vector2(
                ScreenRectangle.Width/2f - ScreenRectangle.X/2f - ActiveShape.Origin.X + TetrisGame.BlockSize/2f,
                TetrisGame.BoardPadding/2f - ActiveShape.Origin.Y - TetrisGame.BlockSize/2f);
        }
    }
}
