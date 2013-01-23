﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public enum Direction { North, East, South, West, Random }

    public abstract class BaseShape
    {
        private Random _random;
        private Vector2 _position;
        private Vector2 _previousPosition;
        protected Rectangle _bounds;

        public  Vector2 LowestBlock { get; protected set; }

        public List<Block[,]> Rotations { get; private set; }

        #region Properties

        public Texture2D Texture { get; private set; }
        public Color Color { get; protected set; }
        public Board Board { get; private set; }

        public Direction CurrentRotation { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position.X = value.X - value.X % TetrisGame.BlockSize;
                _position.Y = value.Y - value.Y % TetrisGame.BlockSize;
            }
        }

        public Vector2 PreviousPosition
        {
            get { return _previousPosition; }
            set { _previousPosition = value; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    _bounds.X,
                    _bounds.Y,
                    _bounds.Width,
                    _bounds.Height);
            }
            set { _bounds = value; }
        }

        public Vector2 Origin
        {
            get
            {
                if (Rotations == null)
                    return Vector2.Zero;

                return new Vector2(
                    ((Rotations[0].GetUpperBound(1) + 1) * TetrisGame.BlockSize) / 2f,
                    ((Rotations[0].GetUpperBound(0) + 1) * TetrisGame.BlockSize) / 2f);
            }
        }

        #endregion

        public BaseShape(Texture2D texture, Board board, Direction direction = Direction.Random)
        {
            _random = new Random();

            Texture = texture;
            Board = board;

            if (direction == Direction.Random)
                direction = (Direction) _random.Next(4);
            CurrentRotation = direction;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Block[,] blocks = Rotations[(int) CurrentRotation];
            for (int row = 0; row < blocks.GetUpperBound(0) + 1; row++)
                for (int col = 0; col < blocks.GetUpperBound(1) + 1; col++)
                    if (blocks[row, col] != null)
                        blocks[row, col].Draw(gameTime, spriteBatch);
        }

        #region Move & Rotate

        public void MoveLeft()
        {
            PreviousPosition = Position;

            _position.X -= Texture.Width;
        }

        public void MoveRight()
        {
            PreviousPosition = Position;

            _position.X += Texture.Width;
        }

        public void MoveDown()
        {
            PreviousPosition = Position;

            _position.Y += Texture.Height;
        }

        public void MoveUp()
        {
            PreviousPosition = Position;

            _position.Y -= Texture.Height;
        }

        public void RotateRight()
        {
            CurrentRotation = GetNextDirection();

            CalculateBounds(true);

            // Check collision with walls
            if (IsColliding())
            {
                CurrentRotation = GetPreviousDirection();
                CalculateBounds(true);
            }
        }

        public void RotateLeft()
        {
            CurrentRotation = GetPreviousDirection();

            CalculateBounds(true);

            // Check collision with walls
            if (IsColliding())
            {
                CurrentRotation = GetNextDirection();
                CalculateBounds(true);
            }
        }

        public void Drop()
        {
            int tmp;
            while (true)
            {
                MoveDown();

                if (Board.IsCollidingWithBottom(this))
                    break;

                if (Board.IsCollidingWithCell(this))
                    break;
            }
        }

        #endregion

        private bool IsColliding()
        {
            // Check collision with walls
            if (Board.IsCollidingWithLeftWall(this) ||
                Board.IsCollidingWithRightWall(this) ||
                Board.IsCollidingWithBottom(this))
                return true;
            return false;
        }

        private Direction GetNextDirection()
        {
            int pos = (int) CurrentRotation;
            pos++;
            if (pos > 3)
                pos = 0;
            return (Direction) pos;
        }

        private Direction GetPreviousDirection()
        {
            int pos = (int) CurrentRotation;
            pos--;
            if (pos < 0)
                pos = 3;
            return (Direction)pos;
        }

        protected void GenerateBlocks(List<int[,]> rotations)
        {
            Rotations = new List<Block[,]>();

            // Append inverted rotations unless all 4 are already specified
            if (rotations.Count != 4)
            {
                List<int[,]> invertedRotations = new List<int[,]>();
                foreach (var rotation in rotations)
                    invertedRotations.Add(InvertRotation(rotation));

                rotations.AddRange(invertedRotations);
            }

            foreach (int[,] rotation in rotations)
            {
                Block[,] blocks = new Block[rotation.GetUpperBound(0) + 1, rotation.GetUpperBound(1) + 1];
                for (int row = 0; row < rotation.GetUpperBound(0) + 1; row++)
                    for (int col = 0; col < rotation.GetUpperBound(1) + 1; col++)
                        if (rotation[row, col] == 1)
                            blocks[row, col] = new Block(this, Color, row, col);
                Rotations.Add(blocks);
            }

            CalculateBounds();
        }

        protected int[,] InvertRotation(int[,] rotation)
        {
            var inverted = new int[rotation.GetUpperBound(0) + 1, rotation.GetUpperBound(1) + 1];

            int rows = rotation.GetUpperBound(0);
            int cols = rotation.GetUpperBound(1);

            for (int row = 0; row <= rows; row++)
                for (int col = 0; col <= cols; col++)
                    if (rotation[row, col] == 1)
                        inverted[rows - row, cols - col] = 1;

            return inverted;
        }

        // Calculate width & height based on block positions
        protected void CalculateBounds(bool rotating = false)
        {
            int width = 0;
            int height = 0;

            int blockSize = TetrisGame.BlockSize;

            Block[,] blocks = Rotations[(int)CurrentRotation];

            int minW = blocks.GetUpperBound(1) + 1;
            int maxW = -1;

            int minH = blocks.GetUpperBound(0) + 1;
            int maxH = -1;

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (minW > col)
                            minW = col;
                        if (maxW < col)
                            maxW = col;

                        if (minH > row)
                            minH = row;
                        if (maxH < row)
                        {
                            maxH = row;
                        }
                    }
                }

                if ((maxW - minW + 1) > width)
                    width = maxW - minW + 1;

                if ((maxH - minH + 1) > height)
                    height = maxH - minH + 1;
            }

            CalculateLowestBlock();

            if (!rotating)
                _position.Y -= LowestBlock.Y;

            _bounds.Width = width * blockSize;
            _bounds.Height = height * blockSize;

            _bounds.X = minW * blockSize;
            _bounds.Y = minH * blockSize;
        }

        protected void CalculateLowestBlock()
        {
            Block[,] blocks = Rotations[(int)CurrentRotation];

            for (int row = blocks.GetUpperBound(0); row >= 0; row--)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    if (blocks[row, col] != null)
                    {
                        LowestBlock = new Vector2(col * TetrisGame.BlockSize, row * TetrisGame.BlockSize);
                        return;
                    }
                }
            }
        }
    }
}
