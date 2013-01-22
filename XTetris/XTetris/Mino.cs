using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public class Mino
    {
        private Texture2D _texture;
        private Vector2 _position;

        #region Properties

        public Tetromino Parent { get; private set; }
        public Color Color { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X * Width, (int)Position.Y * Height, Width, Height); }
        }

        public int Width
        {
            get { return _texture.Width; }
        }

        public int Height
        {
            get { return _texture.Height; }
        }

        #endregion

        public Mino(Texture2D texture, Tetromino parent, Color color, int row, int col)
        {
            _texture = texture;
            Color = color;
            Parent = parent;
            Position = new Vector2(col, row);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Bounds, Color);
        }
    }
}
