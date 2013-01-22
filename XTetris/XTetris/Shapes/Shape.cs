using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public enum Direction { West, East, North, South }

    public abstract class Shape
    {
        private Vector2 _position;

        protected List<Block[,]> _rotations;

        #region Properties

        public Texture2D Texture { get; private set; }
        public Color Color { get; protected set; }

        public Direction CurrentRotation { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        public Shape(Texture2D texture)
        {
            Texture = texture;
            CurrentRotation = Direction.West;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Block[,] blocks = _rotations[(int) CurrentRotation];
            for (int row = 0; row < blocks.GetUpperBound(0) + 1; row++)
                for (int col = 0; col < blocks.GetUpperBound(1) + 1; col++)
                    if (blocks[row, col] != null)
                        blocks[row, col].Draw(gameTime, spriteBatch);
        }

        #region Move & Rotate

        public void MoveLeft()
        {
            _position.X -= Texture.Width;
        }

        public void MoveRight()
        {
            _position.X += Texture.Width;
        }

        public void MoveDown()
        {
            _position.Y += Texture.Height;
        }

        public void RotateRight()
        {
            int pos = (int) CurrentRotation;
            pos++;
            if (pos > 3)
                pos = 0;
            CurrentRotation = (Direction) pos;
        }

        public void RotateLeft()
        {
            int pos = (int) CurrentRotation;
            pos--;
            if (pos < 0)
                pos = 3;
            CurrentRotation = (Direction) pos;
        }

        #endregion

        protected void GenerateBlocks(List<int[,]> rotations)
        {
            _rotations = new List<Block[,]>();

            // Add two inverted rotations
            List<int[,]> invertedRotations = new List<int[,]>();
            foreach (var rotation in rotations)
                invertedRotations.Add(InvertRotation(rotation));

            // Append inverted rotations
            rotations.AddRange(invertedRotations);

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

        protected int[,] InvertRotation(int[,] rotation)
        {
            return rotation;
        }
    }
}
