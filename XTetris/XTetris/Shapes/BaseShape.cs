﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public enum Direction { West, East, North, South }

    public abstract class BaseShape
    {
        private Vector2 _position;

        protected List<Block[,]> _rotations;

        #region Properties

        public Texture2D Texture { get; private set; }
        public Color Color { get; protected set; }
        public Board Board { get; private set; }

        public Direction CurrentRotation { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Origin
        {
            get
            {
                if (_rotations == null)
                    return Vector2.Zero;

                return new Vector2(
                    ((_rotations[0].GetUpperBound(1) + 1) * TetrisGame.BlockSize) / 2f,
                    ((_rotations[0].GetUpperBound(0) + 1) * TetrisGame.BlockSize) / 2f);
            }
        }

        #endregion

        public BaseShape(Texture2D texture, Board board)
        {
            Texture = texture;
            Board = board;
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
                _rotations.Add(blocks);
            }
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
    }
}