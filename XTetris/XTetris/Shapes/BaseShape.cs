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

        public List<Block[,]> Rotations { get; private set; }

        public Direction Direction { get; set; }

        public Vector2 LastPosition { get; set; }
        public Direction LastDirection { get; set; }

        public Rectangle Bounds { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(Bounds.Width / 2f * TetrisGame.BlockSize, Bounds.Height / 2f * TetrisGame.BlockSize); }
        }

        #endregion

        public BaseShape(Texture2D texture, Board board, Color color)
        {
            Texture = texture;
            Board = board;
            Direction = Direction.Right;
            Color = color;

            Position = new Vector2(0, 0);

            LastPosition = Position;
            LastDirection = Direction;
        }

        #region Move & Rotate

        public void Move(Direction direction)
        {
            LastPosition = Position;

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

            CalculateBounds();
        }

        public void Drop()
        {
            LastDirection = Direction;
            LastPosition = Position;

            while (Board.ActiveShape == this)
            {
                Move(Direction.Down);

                Board.CheckCollisions();
            }
        }

        public void Rotate(Direction direction)
        {
            LastDirection = Direction;

            switch (direction)
            {
                case Direction.Left:
                    Direction = PreviousDirection();
                    break;

                case Direction.Right:
                    Direction = NextDirection();
                    break;
            }

            CalculateBounds();
        }

        private Direction NextDirection()
        {
            int dir = (int) Direction;
            dir++;
            if (dir > 3)
                dir = 0;
            return (Direction) dir;
        }

        private Direction PreviousDirection()
        {
            int dir = (int)Direction;
            dir--;
            if (dir < 0)
                dir = 3;
            return (Direction)dir;
        }

        #endregion

        private void CalculateBounds()
        {
            // Find min/max for both X and Y
            var blocks = Rotations[(int) Direction];

            int minX = blocks.GetUpperBound(1) + 1;
            int maxX = -1;

            int minY = blocks.GetUpperBound(1) + 1;
            int maxY = -1;

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                    if (blocks[row, col] != null)
                    {
                        minX = Math.Min(minX, col);
                        maxX = Math.Max(maxX, col);

                        minY = Math.Min(minY, row);
                        maxY = Math.Max(maxY, row);
                    }

            Bounds = new Rectangle(
                (int) Position.X + minX,
                (int) Position.Y + minY,
                1 + maxX - minX,
                1 + maxY - minY);
        }

        public void ResetPosition()
        {
            Vector2 newPos = Position;

            if ((int) Position.X != (int) LastPosition.X)
                newPos.X = LastPosition.X;
            if ((int) Position.Y != (int) LastPosition.Y)
                newPos.Y = LastPosition.Y;

            Position = newPos;
            LastPosition = Position;

            Direction = LastDirection;
        }

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

            Board.ActiveShape = null;
            Board.AllowHold = true;
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

            CalculateBounds();
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
