using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Shapes;

namespace XTetris.Shapes
{
    public enum Direction { Right, Up, Left, Down }

    public class BaseShape
    {
        public readonly Color Color;

        private Vector2 _position;

        #region Properties

        public Board Board { get; private set; }
        public Texture2D Texture { get; private set; }
        public Direction Direction { get; private set; }

        public List<Block[,]> Rotations { get; private set; }

        public Vector2 PreviousPosition { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        public BaseShape(Texture2D texture, Board board, Color color)
        {
            Texture = texture;
            Board = board;
            Direction = Direction.Right;
            Color = color;

            Position = new Vector2(3, 0);
        }

        #region Move & Rotate

        public void Move(Direction direction)
        {
            PreviousPosition = Position;

            switch (direction)
            {
                case Direction.Left:
                    _position.X--;
                    break;
                case Direction.Right:
                    _position.X++;
                    break;
                case Direction.Up:
                    _position.Y--;
                    break;
                case Direction.Down:
                    _position.Y++;
                    break;
            }
        }

        public void Rotate(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    break;

                case Direction.Right:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Save shape to board.
        /// </summary>
        public void Save()
        {
            var blocks = Rotations[(int)Direction];

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    var block = blocks[row, col];
                    if (block != null)
                    {
                        int x = (int) block.BoardPosition.X;
                        int y = (int)block.BoardPosition.Y;
                        if (Board.Cells[y, x] != null)
                            throw new Exception("This cell should be empty!");
                        Board.Cells[y, x] = block;
                    }
                }
            }
        }

        #region Generate Blocks

        protected void GenerateBlocks(List<int[,]> rotations)
        {
            Rotations = new List<Block[,]>();

            // Append inverted rotations unless all 4 are already specified
            if (rotations.Count != 4)
            {
                var invertedRotations = new List<int[,]>();
                foreach (var rotation in rotations)
                    invertedRotations.Add(InvertRotation(rotation));

                rotations.AddRange(invertedRotations);
            }

            foreach (int[,] rotation in rotations)
            {
                var blocks = new Block[rotation.GetUpperBound(0) + 1,rotation.GetUpperBound(1) + 1];
                for (int row = 0; row <= rotation.GetUpperBound(0); row++)
                    for (int col = 0; col <= rotation.GetUpperBound(1); col++)
                        if (rotation[row, col] == 1)
                            blocks[row, col] = new Block(this, Color, new Vector2(col, row));
                Rotations.Add(blocks);
            }
        }

        protected int[,] InvertRotation(int[,] rotation)
        {
            var inverted = new int[rotation.GetUpperBound(0) + 1,rotation.GetUpperBound(1) + 1];

            int rows = rotation.GetUpperBound(0);
            int cols = rotation.GetUpperBound(1);

            for (int row = 0; row <= rows; row++)
                for (int col = 0; col <= cols; col++)
                    if (rotation[row, col] == 1)
                        inverted[rows - row, cols - col] = 1;

            return inverted;
        }

        #endregion
    }
}
