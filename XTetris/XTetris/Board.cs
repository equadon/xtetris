using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Engine;
using XTetris.GameStates;
using XTetris.Shapes;

namespace XTetris
{
    public class Board
    {
        #region Fields

        public static readonly Color BackgroundColor = new Color(255, 255, 255);

        private const int MaxShapesInQueue = 5;

        private readonly Random _random;
        private readonly GamePlayState _gameState;

        #endregion

        #region Properties

        public GamePlayState GameState { get; set; }

        public Player Player { get; private set; }
        public int Level { get; private set; }

        public Block[,] Cells { get; private set; }

        public Queue<BaseShape> ShapesQueue { get; private set; }
        public BaseShape ActiveShape { get; private set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(TetrisGame.BoardPaddingSide + 152, TetrisGame.BoardPaddingTop - 6, Width, Height); }
        }

        public int Width
        {
            get { return TetrisGame.BlocksWide * TetrisGame.BlockSize; }
        }

        public int Height
        {
            get { return TetrisGame.BlocksHigh * TetrisGame.BlockSize; }
        }

        #endregion

        public Board(GamePlayState gameState)
        {
            _random = new Random();
            GameState = gameState;
            Player = new Player(this);
            Level = 1;
            Cells = new Block[TetrisGame.BlocksHigh, TetrisGame.BlocksWide];

            // Fill queue with shapes
            ShapesQueue = new Queue<BaseShape>();

            ActiveShape = ShapesFactory.CreateShape(this, (ShapeTypes)_random.Next(7));

            for (int i = 0; i < MaxShapesInQueue; i++)
            {
                ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, (ShapeTypes) _random.Next(7)));
            }
        }

        #region XNA Draw & Update

        public void Update(GameTime gameTime)
        {
            // Handle player input
            Player.Update(gameTime);

            CheckCollisions();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.FillTexture, Bounds, BackgroundColor);

            // Draw cells
            DrawCells(gameTime, spriteBatch);

            // Draw active shape
            DrawShape(ActiveShape, gameTime, spriteBatch);

            // Print debug info
            string text = "Active Shape: (" + ActiveShape.Position.X + "," + ActiveShape.Position.Y + ")" +
                          "\n\nElapsed: " + gameTime.TotalGameTime.ToString() +
                          "\n\nBoard: " +
                          "\n    size: " + Width + "x" + Height +
                          "\n    top left: (" + Bounds.Left + "," + Bounds.Top + ")" +
                          "\n    bot right:  (" + Bounds.Right + "," + Bounds.Bottom + ")";

            Vector2 textSize = GameState.DebugFont.MeasureString(text);

            Vector2 pos = new Vector2(Convert.ToInt32(Bounds.Right - textSize.X) - 15,
                                      Convert.ToInt32(Bounds.Bottom - textSize.Y) - 15);

            spriteBatch.DrawString(GameState.DebugFont, text, pos, Color.White);

            // Draw border
            //spriteBatch.Draw(GameState.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);
        }

        private void DrawShape(BaseShape shape, GameTime gameTime, SpriteBatch spriteBatch)
        {
            Block[,] blocks = shape.Rotations[(int) shape.Direction];
            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    Block block = blocks[row, col];
                    if (block != null)
                    {
                        spriteBatch.Draw(GameState.BlockTexture,
                            new Vector2(
                                Bounds.Left + block.BoardPosition.X * TetrisGame.BlockSize,
                                Bounds.Top + block.BoardPosition.Y * TetrisGame.BlockSize),
                            ActiveShape.Color);
                    }
                }
            }
        }

        private void DrawCells(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int row = 2; row <= Cells.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                {
                    Block cell = Cells[row, col];
                    Texture2D cellTexture;

                    if (cell == null)
                    {
                        cellTexture = GameState.EmptyBlockTexture;
                    }
                    else
                    {
                        cellTexture = GameState.BlockTexture;
                    }

                    Vector2 cellPos = new Vector2(
                        Bounds.Left + col * TetrisGame.BlockSize,
                        Bounds.Top + row * TetrisGame.BlockSize);
                    spriteBatch.Draw(cellTexture, cellPos, Color.White);
                }
            }
        }

        #endregion

        private void CheckCollisions()
        {
            var blocks = ActiveShape.Rotations[(int) ActiveShape.Direction];

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    var block = blocks[row, col];
                    if (block != null)
                    {
                        // Left + right wall
                        if (block.BoardPosition.X < 0)
                            ActiveShape.Move(Direction.Right);
                        else if (block.BoardPosition.X >= TetrisGame.BlocksWide)
                            ActiveShape.Move(Direction.Left);

                        // Top + bottom wall
                        if (block.BoardPosition.Y < 0)
                            ActiveShape.Move(Direction.Down);
                        else if (block.BoardPosition.Y >= TetrisGame.BlocksHigh)
                            ActiveShape.Move(Direction.Up);
                    }
                }
            }
        }

        /// <summary>
        /// Spawns the next shape in queue on the board.
        /// </summary>
        public void SpawnShape()
        {
        }
    }
}
