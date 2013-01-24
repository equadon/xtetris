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

        private double _cancelIconDuration = 0d;

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

        public bool AllowHold { get; set; }
        public BaseShape HoldShape { get; private set; }

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

            AllowHold = true;

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

            if (_cancelIconDuration > 0d)
                _cancelIconDuration -= gameTime.ElapsedGameTime.TotalSeconds;

            // Handle player input
            Player.Update(gameTime);

            if (HasActiveShape)
                CheckCollisions();

            // See if there are any lines we can clear
            ClearLines();
        }

        #region Draw Methods

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.FillTexture, Bounds, BackgroundColor);

            // Draw cells
            DrawCells(spriteBatch);

            // Draw active shape
            if (HasActiveShape)
            {
                DrawShape(spriteBatch, ActiveShape, new Vector2(Bounds.Left, Bounds.Top), 1.0f);

                // Print debug info
                if (TetrisGame.Debug)
                {
                    string text = "\n\nDelay left: " + _moveDelayDuration.ToString("N5") +
                                  "\n\nActive Shape: (" + ActiveShape.Position.X + "," + ActiveShape.Position.Y + ")" +
                                  "\n    Direction: " + ActiveShape.Direction +
                                  "\n    Bounds:" +
                                  "\n       position: (" + ActiveShape.Bounds.X + "," + ActiveShape.Bounds.Y + ")" +
                                  "\n       size: " + ActiveShape.Bounds.Width + "x" + ActiveShape.Bounds.Height +
                                  "\n       top left: (" + ActiveShape.Bounds.Left + "," + ActiveShape.Bounds.Top + ")" +
                                  "\n       bot right: (" + ActiveShape.Bounds.Right + "," + ActiveShape.Bounds.Bottom + ")" +
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
            }

            // Draw border
            if (!TetrisGame.Debug)
                spriteBatch.Draw(GameState.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);

            // Draw queue
            DrawQueue(spriteBatch);

            // Draw shape in hold queue
            int blockSize = TetrisGame.BlockSize;
            float scale = 0.85f;

            if (HoldShape != null)
                DrawShape(
                    spriteBatch,
                    HoldShape,
                    new Vector2(
                        TetrisGame.BoardPaddingSide - HoldShape.Bounds.X * blockSize * scale + 10 + 60 - HoldShape.Origin.X * scale,
                        TetrisGame.BoardPaddingTop - HoldShape.Bounds.Y * blockSize * scale + 73 + 60 - HoldShape.Origin.Y * scale),
                    scale);

            // User tried to hold when hold was not allowed
            if (_cancelIconDuration > 0d)
                spriteBatch.Draw(
                    GameState.CancelTexture,
                    new Vector2(
                        TetrisGame.BoardPaddingSide + 9 + 62 - GameState.CancelTexture.Width/2f,
                        TetrisGame.BoardPaddingTop + 72 + 65 - GameState.CancelTexture.Height / 2f), 
                    Color.White);
        }

        private void DrawCells(SpriteBatch spriteBatch)
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

        private void DrawShape(SpriteBatch spriteBatch, BaseShape shape, Vector2 position, float scale)
        {
            Block[,] blocks = shape.Rotations[(int)shape.Direction];
            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    Block block = blocks[row, col];
                    if (block != null)
                    {
                        spriteBatch.Draw(GameState.BlockTexture,
                            new Vector2(
                                position.X + (block.BoardPosition.X * TetrisGame.BlockSize) * scale,
                                position.Y + (block.BoardPosition.Y * TetrisGame.BlockSize) * scale),
                            null,
                            block.Color,
                            0f,
                            Vector2.Zero,
                            scale,
                            SpriteEffects.None,
                            0f);

                        // Debug block positions
                        if (TetrisGame.Debug)
                        {
                            string text = block.BoardPosition.Y + "," + block.BoardPosition.X;
                            var pos = new Vector2(
                                Bounds.Left + block.BoardPosition.X * 30 + 5,
                                Bounds.Top + block.BoardPosition.Y * 30 + 5);

                            spriteBatch.DrawString(GameState.DebugFont, text, pos, Color.Red, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }

        private void DrawQueue(SpriteBatch spriteBatch)
        {
            int blockSize = TetrisGame.BlockSize;

            // Draw first shape in queue
            BaseShape firstShape = ShapesQueue.Peek();

            float scale = 0.85f;
            DrawShape(spriteBatch,
                firstShape,
                new Vector2(
                    Bounds.Right - firstShape.Bounds.X * blockSize * scale + 21 + 60 - firstShape.Origin.X * scale,
                    TetrisGame.BoardPaddingTop - firstShape.Bounds.Y * blockSize * scale + 74 + 60 - firstShape.Origin.Y * scale),
                scale);

            // Draw the rest
            const int queueAreaWidth = 47;
            const int queueAreaHeight = 49;

            float offsetY = 0f;
            scale = 0.65f;

            int i = 0;

            foreach (var shape in ShapesQueue)
            {
                i++;

                // Skip the first shape
                if (i == 1)
                    continue;

                Vector2 pos = new Vector2(
                    Bounds.Right - shape.Bounds.X * blockSize * scale + 34 + queueAreaWidth - shape.Origin.X * scale,
                    TetrisGame.BoardPaddingTop + offsetY - shape.Bounds.Y * blockSize * scale + 236 + queueAreaHeight - shape.Origin.Y * scale);

                DrawShape(spriteBatch,
                    shape,
                    pos,
                    scale);

                offsetY += queueAreaHeight * 2;
            }
        }

        #endregion

        public void GameOver()
        {
            GameState.StateManager.ChangeState(GameState.GameRef.GameOverState);
        }

        #region Collision Detection and Clearing Lines

        private void ClearLines()
        {
            int linesCleared = 0;

            for (int row = Cells.GetUpperBound(0); row >= 0; row--)
            {
                int columnCount = 0;

                for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                {
                    if (Cells[row, col] != null)
                    {
                        columnCount++;
                    }
                }

                // Clear lines if the whole row is filled
                if (columnCount == Cells.GetUpperBound(1) + 1)
                {
                    for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                        Cells[row, col] = null;

                    HandlePhysics(row);

                    linesCleared++;
                }
                else if (columnCount == 0)
                {
                    // no need to check anymore
                    return;
                }
            }
        }

        // Make "flying" blocks fall down
        private void HandlePhysics(int currentRow)
        {
            for (int row = currentRow - 1; row >= 0; row--)
            {
                int columnCount = 0;

                for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                {
                    Block block = Cells[row, col];
                    if (block != null)
                    {
                        Cells[row, col] = null;
                        Cells[row + 1, col] = block;

                        columnCount++;
                    }
                }

                if (columnCount == 0)
                    return;
            }
        }

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

                            return;
                        }

                        // Other blocks
                        if (Cells[(int) block.BoardPosition.Y, (int) block.BoardPosition.X] != null)
                        {
                            // We don't want to save to board if there only were horizontal movement
                            bool save = !((int) ActiveShape.Position.Y == (int) ActiveShape.LastPosition.Y &&
                                          (int) ActiveShape.Position.X != (int) ActiveShape.LastPosition.X);

                            if (Player.Rotated)
                            {
                                ActiveShape.Direction = ActiveShape.LastDirection;
                            }
                            else
                            {
                                ActiveShape.ResetPosition();

                                if (save)
                                    ActiveShape.Save();
                            }

                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region Hold and Spawn Shape Methods

        // Place the active shape in the hold queue
        public void Hold()
        {
            // Show cancel texture if hold is not allowed
            if (!AllowHold)
            {
                _cancelIconDuration = 0.3d;
                return;
            }

            if (HoldShape == null)
            {
                HoldShape = ActiveShape;
                ActiveShape = null;
            }
            else
            {
                BaseShape tmp = ActiveShape;
                ActiveShape = HoldShape;
                HoldShape = tmp;
            }

            HoldShape.Position = new Vector2(0, 0);

            SpawnShape(ActiveShape);

            AllowHold = false;
        }

        /// <summary>
        /// Spawns the next shape in queue or the provided `shape`.
        /// </summary>
        public void SpawnShape(BaseShape shape = null)
        {
            // Fill the queue if needed
            if (ShapesQueue.Count < MaxShapesInQueue)
            {
                do
                {
                    //ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, (ShapeTypes) _random.Next(7)));
                    ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, ShapeTypes.I));
                } while (ShapesQueue.Count < MaxShapesInQueue);
            }

            if (shape == null)
                shape = ShapesQueue.Dequeue();

            shape.Position = new Vector2(3, 1);
            ActiveShape = shape;

            if (ShapesQueue.Count < MaxShapesInQueue)
            {
                shape = ShapesFactory.CreateShape(this, (ShapeTypes) _random.Next(7));
                ShapesQueue.Enqueue(shape);
            }
        }

        #endregion
    }
}
