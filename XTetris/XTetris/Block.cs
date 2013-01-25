using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Shapes;

namespace XTetris
{
    public class Block
    {
        private Vector2 _position;

        #region Properties

        public BaseShape ParentShape { get; private set; }
        public Color Color { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 BoardPosition
        {
            get { return ParentShape.Position + Position; }
        }

        #endregion

        public Block(BaseShape parentShape, Color color, Vector2 position)
        {
            ParentShape = parentShape;
            Color = color;
            Position = position;
        }
    }
}
