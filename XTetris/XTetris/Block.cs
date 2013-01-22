using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public class Block
    {
        private Vector2 _position;

        #region Properties

        public BaseShape Parent { get; private set; }
        public Color Color { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 AbsolutePosition
        {
            get
            {
                return new Vector2(
                    Parent.Position.X + Position.X * Width,
                    Parent.Position.Y + Position.Y * Height);
            }
        }

        public Texture2D Texture
        {
            get { return Parent.Texture; }
        }

        public int Width
        {
            get { return Texture.Width; }
        }

        public int Height
        {
            get { return Texture.Height; }
        }

        #endregion

        public Block(BaseShape parent, Color color, int row, int col)
        {
            Color = color;
            Parent = parent;
            Position = new Vector2(col, row);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, AbsolutePosition, Color);
        }
    }
}
