using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public class Board
    {
        #region Properties & Fields

        private int _blockWidth;
        private int _blockHeight;

        private Vector2 _position;
        private Rectangle _screenRectangle;

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

        #endregion

        public Board(Texture2D texture, int blockWidth, int blockHeight)
        {
            _blockWidth = blockWidth;
            _blockHeight = blockHeight;

            Texture = texture;

            ScreenRectangle = new Rectangle(10, 10, blockWidth * 10, blockHeight * 20);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw board
            spriteBatch.Draw(Texture, ScreenRectangle, Color.Black);
        }
    }
}
