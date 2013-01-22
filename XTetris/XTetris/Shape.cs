using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public enum RotationDirection { West, East, North, South }

    public abstract class Shape
    {
        private Vector2 _position;

        protected List<Block[,]> _rotations;

        #region Properties

        public Texture2D Texture { get; private set; }
        public Color Color { get; private set; }

        public RotationDirection RotationPosition { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        public Shape(Texture2D texture, Color color)
        {
            Texture = texture;
            Color = color;
            RotationPosition = RotationDirection.West;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Block[,] blocks = _rotations[(int) RotationPosition];
            for (int row = 0; row < blocks.GetUpperBound(0) + 1; row++)
                for (int col = 0; col < blocks.GetUpperBound(1) + 1; col++)
                    if (blocks[row, col] != null)
                        blocks[row, col].Draw(gameTime, spriteBatch);
        }

        protected void GenerateBlocks(List<int[,]> rotations)
        {
            _rotations = new List<Block[,]>();

            foreach (int[,] rotation in rotations)
            {
                Block[,] blocks = new Block[rotation.GetUpperBound(0) + 1, rotation.GetUpperBound(1) + 1];
                for (int row = 0; row < rotation.GetUpperBound(0) + 1; row++)
                    for (int col = 0; col < rotation.GetUpperBound(1) + 1; col++)
                        if (rotation[row, col] == 1)
                            blocks[row, col] = new Block(this, Color, row, col);
                _rotations.Add(blocks);
            }
        }

        public void MoveLeft()
        {
        }

        public void MoveRight()
        {
        }

        public void MoveDown()
        {
        }

        public void RotateRight()
        {
            RotationPosition = GetNextRotationPosition();
        }

        public void RotateLeft()
        {
            RotationPosition = GetPreviousRotationPosition();
        }

        protected RotationDirection GetNextRotationPosition()
        {
            int pos = (int) RotationPosition;
            pos++;
            if (pos > 3)
                pos = 0;
            return (RotationDirection) pos;
        }

        protected RotationDirection GetPreviousRotationPosition()
        {
            int pos = (int)RotationPosition;
            pos--;
            if (pos < 0)
                pos = 3;
            return (RotationDirection)pos;
        }
    }
}
