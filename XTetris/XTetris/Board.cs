using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private const double MoveDownDelay = 1.0d;
        private const double SpawnDelay = 1.0d;

        private readonly Random _random;

        private double _moveDelayDuration = MoveDownDelay;

        #endregion

        #region Properties

        public GamePlayState GameState { get; set; }

        public Player Player { get; private set; }
        public int Level { get; private set; }

        public Block[,] Cells { get; private set; }

        public Queue<BaseShape> ShapesQueue { get; private set; }
        public BaseShape ActiveShape { get; set; }

        public bool HasActiveShape
        {
            get { return ActiveShape != null; }
        }

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

            SpawnShape();
        }

        public void Update(GameTime gameTime)
        {
            if (!HasActiveShape)
                _moveDelayDuration = MoveDownDelay;
            else
                _moveDelayDuration -= gameTime.ElapsedGameTime.TotalSeconds;

            if (HasActiveShape && _moveDelayDuration <= 0d)
            {
                _moveDelayDuration = MoveDownDelay;
                ActiveShape.Move(Direction.Down);
            }

            // Handle player input
            Player.Update(gameTime);

            if (HasActiveShape)
                CheckCollisions();
        }

        #region Draw Methods

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.FillTexture, Bounds, BackgroundColor);

            // Draw cells
            DrawCells(gameTime, spriteBatch);

            // Draw active shape
            if (HasActiveShape)
            {
                DrawShape(ActiveShape, gameTime, spriteBatch);

                // Print debug info
                string text = "Delay left: " + _moveDelayDuration.ToString("N5") +
                              "\n\nActive Shape: (" + ActiveShape.Position.X + "," + ActiveShape.Position.Y + ")" +
                              "\n    Direction: " + ActiveShape.Direction +
                              "\n\nElapsed: " + gameTime.TotalGameTime.ToString() +
                              "\n\nBoard: " +
                              "\n    size: " + Width + "x" + Height +
                              "\n    top left: (" + Bounds.Left + "," + Bounds.Top + ")" +
                              "\n    bot right:  (" + Bounds.Right + "," + Bounds.Bottom + ")";

                Vector2 textSize = GameState.DebugFont.MeasureString(text);

                Vector2 pos = new Vector2(Convert.ToInt32(Bounds.Right - textSize.X) - 15,
                                          Convert.ToInt32(Bounds.Bottom - textSize.Y) - 15);

                spriteBatch.DrawString(GameState.DebugFont, text, pos, Color.White);
            }

            // Draw border
            spriteBatch.Draw(GameState.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);
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
                            block.Color);

                        //// Debug block positions
                        //string text = block.BoardPosition.Y + "," + block.BoardPosition.X;
                        //var pos = new Vector2(
                        //    Bounds.Left + block.BoardPosition.X * 30 + 5,
                        //    Bounds.Top + block.BoardPosition.Y * 30 + 5);

                        //spriteBatch.DrawString(GameState.DebugFont, text, pos, Color.Red, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
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
                    Block block = Cells[row, col];

                    Vector2 cellPos = new Vector2(
                        Bounds.Left + col * TetrisGame.BlockSize,
                        Bounds.Top + row * TetrisGame.BlockSize);

                    // Draw empty cell for now to fix the white corners
                    spriteBatch.Draw(GameState.EmptyBlockTexture, cellPos, Color.White);

                    if (block != null)
                    {
                        spriteBatch.Draw(GameState.BlockTexture, cellPos, block.Color);
                    }
                }
            }
        }

        #endregion

        public void CheckCollisions()
        {
            var blocks = ActiveShape.Rotations[(int) ActiveShape.Direction];

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    var block = blocks[row, col];
                    if (block != null)
                    {
                        // Left wall
                        if (block.BoardPosition.X < 0)
                        {
                            if (Player.Rotated)
                                ActiveShape.Direction = ActiveShape.LastDirection;
                            else
                                ActiveShape.Move(Direction.Right);

                            return;
                        }

                        // Right wall
                        if (block.BoardPosition.X >= TetrisGame.BlocksWide)
                        {
                            if (Player.Rotated)
                                ActiveShape.Direction = ActiveShape.LastDirection;
                            else
                                ActiveShape.Move(Direction.Left);

                            return;
                        }

                        // Top wall
                        if (block.BoardPosition.Y < 0)
                        {
                            if (Player.Rotated)
                                ActiveShape.Direction = ActiveShape.LastDirection;
                            else
                                ActiveShape.Move(Direction.Down);

                            return;
                        }

                        // Bottom wall
                        if (block.BoardPosition.Y >= TetrisGame.BlocksHigh)
                        {
                            ActiveShape.Move(Direction.Up);

                            ActiveShape.Save();
                            SpawnShape();

                            return;
                        }

                        // Other blocks
                        if (Cells[(int)block.BoardPosition.Y, (int)block.BoardPosition.X] != null)
                        {
                            // We don't want to save to board if there only were horizontal movement
                            bool save = !((int)ActiveShape.Position.Y == (int)ActiveShape.LastPosition.Y &&
                                          (int)ActiveShape.Position.X != (int)ActiveShape.LastPosition.X);

                            ActiveShape.ResetPosition();

                            if (save)
                            {
                                ActiveShape.Save();
                                SpawnShape();
                            }

                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Spawns the next shape in queue.
        /// </summary>
        public void SpawnShape()
        {
            // Fill the queue if needed
            if (ShapesQueue.Count < MaxShapesInQueue)
            {
                do
                {
                    ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, (ShapeTypes) _random.Next(7)));
                } while (ShapesQueue.Count < MaxShapesInQueue);
            }

            ActiveShape = ShapesQueue.Dequeue();
            ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, (ShapeTypes)_random.Next(7)));
        }
    }
}
