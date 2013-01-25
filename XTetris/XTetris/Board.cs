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

        private double _cancelIconDuration = 0d;

        private readonly Random _random;

        #endregion

        #region Properties

        public GamePlayState GameState { get; set; }

        public Player Player { get; private set; }

        public Block[,] Cells { get; private set; }

        public Queue<BaseShape> ShapesQueue { get; private set; }
        public BaseShape ActiveShape { get; set; }

        public bool AllowHold { get; set; }
        public BaseShape HoldShape { get; private set; }

        public double TotalTime { get; private set; }

        // HUD Items
        public HudItem ScoreHud { get; private set; }
        public HudItem LevelHud { get; private set; }
        public HudItem TimeHud { get; private set; }

        public bool IsGameOver { get; private set; }

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
            Cells = new Block[TetrisGame.BlocksHigh, TetrisGame.BlocksWide];

            AllowHold = true;

            IsGameOver = false;

            ShapesQueue = new Queue<BaseShape>();

            SpawnShape();
        }

        public void LoadContent()
        {
            Vector2 pos = new Vector2(10, 190);

            ScoreHud = new HudItem(GameState.GameFont, "Score");
            ScoreHud.Position = pos;

            pos.Y += 150;
            LevelHud = new HudItem(GameState.GameFont, "Level");
            LevelHud.Position = pos;

            pos.Y += 150;
            TimeHud = new HudItem(GameState.GameFont, "Time");
            TimeHud.Position = pos;
        }

        public void Update(GameTime gameTime)
        {
            if (IsGameOver)
                return;

            TotalTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (_cancelIconDuration > 0d)
                _cancelIconDuration -= gameTime.ElapsedGameTime.TotalSeconds;

            // Handle player input
            Player.Update(gameTime);

            if (HasActiveShape)
                CheckCollisions(ActiveShape);

            // See if there are any lines we can clear
            ClearLines();
        }

        #region Draw Methods

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsGameOver)
                return;

            spriteBatch.Draw(GameState.FillTexture, Bounds, BackgroundColor);

            // Draw cells
            DrawCells(spriteBatch);

            // Draw active shape
            if (HasActiveShape)
                DrawShape(spriteBatch, ActiveShape, new Vector2(Bounds.Left, Bounds.Top), 1.0f);

            // Draw border
            if (!TetrisGame.Debug)
                spriteBatch.Draw(GameState.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);

            // Labels
            spriteBatch.DrawString(GameState.GameFont, "Hold",
                new Vector2(
                    TetrisGame.BoardPaddingSide + 20,
                    TetrisGame.BoardPaddingTop + 15),
                Color.Crimson);

            spriteBatch.DrawString(GameState.GameFont, "Next",
                new Vector2(
                    Bounds.Right + 25,
                    TetrisGame.BoardPaddingTop + 15),
                Color.Crimson);

            // Draw queue
            DrawQueue(spriteBatch);

            // Draw shape in hold queue
            const float scale = 0.85f;

            if (HoldShape != null)
                DrawShape(
                    spriteBatch,
                    HoldShape,
                    new Vector2(
                        TetrisGame.BoardPaddingSide - HoldShape.Bounds.X * TetrisGame.BlockSize * scale + 10 + 60 - HoldShape.Origin.X * scale,
                        TetrisGame.BoardPaddingTop - HoldShape.Bounds.Y * TetrisGame.BlockSize * scale + 73 + 60 - HoldShape.Origin.Y * scale),
                    scale);

            // User tried to hold when hold was not allowed
            if (_cancelIconDuration > 0d)
                spriteBatch.Draw(
                    GameState.CancelTexture,
                    new Vector2(
                        TetrisGame.BoardPaddingSide + 9 + 62 - GameState.CancelTexture.Width/2f,
                        TetrisGame.BoardPaddingTop + 72 + 65 - GameState.CancelTexture.Height / 2f), 
                    Color.White);

            // Draw HUD items
            DrawHud(gameTime, spriteBatch);
        }

        public void DrawCells(SpriteBatch spriteBatch)
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
                        spriteBatch.Draw(GameState.BlockTexture, cellPos, block.Color);
                }
            }
        }

        private void DrawShape(SpriteBatch spriteBatch, BaseShape shape, Vector2 position, float scale)
        {
            Vector2 ghostPos = GhostPosition(shape);

            Block[,] blocks = shape.Rotations[(int)shape.Direction];
            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    Block block = blocks[row, col];
                    if (block != null)
                    {
                        // Ghost shape
                        if (Player.GhostShapeEnabled && shape.Equals(ActiveShape))
                        {
                            Vector2 ghostBlockPos = new Vector2(
                                position.X + (block.BoardPosition.X * TetrisGame.BlockSize) * scale,
                                position.Y + (ghostPos.Y + block.Position.Y) * TetrisGame.BlockSize * scale);

                            spriteBatch.Draw(GameState.BlockTexture,
                                ghostBlockPos,
                                null,
                                Color.Gray,
                                0f,
                                Vector2.Zero,
                                scale,
                                SpriteEffects.None,
                                0f);
                        }

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

        private void DrawHud(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Score
            ScoreHud.Draw(spriteBatch, Player.Score);

            // Time
            LevelHud.Draw(spriteBatch, Player.Level);

            // Time
            TimeHud.Draw(spriteBatch, Convert.ToInt32(TotalTime));
        }

        #endregion

        // TODO: Implement a better game over checking
        public void GameOver()
        {
            IsGameOver = true;
            GameState.StateManager.ChangeState(GameState.GameRef.GameOverState);
        }

        private Vector2 GhostPosition(BaseShape shape)
        {
            var checkList = new List<Block>();
            var blocks = shape.Rotations[(int) shape.Direction];

            for (int col = 0; col <= blocks.GetUpperBound(1); col++)
            {
                for (int row = blocks.GetUpperBound(0); row >= 0; row--)
                {
                    Block block = blocks[row, col];
                    if (block != null)
                    {
                        checkList.Add(block);
                        break;
                    }
                }
            }

            Vector2 shapePos = shape.Position;

            for (int row = (int)shapePos.Y; row <= TetrisGame.BlocksHigh; row++)
            {
                bool collision = false;

                foreach (var block in checkList)
                {
                    float newX = shapePos.X + block.Position.X;
                    float newY = shapePos.Y + block.Position.Y;

                    if (newX < 0 || newX > TetrisGame.BlocksWide - 1 ||
                        newY < 0 || newY > TetrisGame.BlocksHigh - 1 ||
                        Cells[(int)newY, (int)newX] != null)
                        collision = true;
                }

                if (collision)
                    break;

                shapePos.Y++;
            }

            return new Vector2(shape.Position.X, shapePos.Y - 1);
        }

        #region Collision Detection and Clearing Lines

        private void ClearLines()
        {
            int linesCleared = 0;

            int firstClearedLine = -1;

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

                    if (firstClearedLine == -1)
                        firstClearedLine = row;

                    linesCleared++;
                }
                else if (columnCount == 0)
                {
                    // no need to check anymore
                    row = -1;
                    break;
                }
            }

            if (linesCleared > 0)
            {
                Player.LineClear(linesCleared);

                HandlePhysics(firstClearedLine, linesCleared);
            }
        }

        // Make "flying" blocks fall down
        private void HandlePhysics(int firstClearedRow, int linesCleared)
        {
            for (int row = firstClearedRow - linesCleared; row >= 0; row--)
            {
                int columnCount = 0;

                for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                {
                    Block block = Cells[row, col];
                    if (block != null)
                    {
                        Cells[row, col] = null;
                        Cells[row + linesCleared, col] = block;

                        columnCount++;
                    }
                }

                if (columnCount == 0)
                    return;
            }
        }

        public void CheckCollisions(BaseShape shape)
        {
            var blocks = shape.Rotations[(int) shape.Direction];

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
                                shape.Direction = shape.LastDirection;
                            else
                                shape.Move(Direction.Right);

                            return;
                        }

                        // Right wall
                        if (block.BoardPosition.X >= TetrisGame.BlocksWide)
                        {
                            if (Player.Rotated)
                                shape.Direction = shape.LastDirection;
                            else
                                shape.Move(Direction.Left);

                            return;
                        }

                        // Top wall
                        if (block.BoardPosition.Y < 0)
                        {
                            if (Player.Rotated)
                                shape.Direction = shape.LastDirection;
                            else
                                shape.Move(Direction.Down);

                            return;
                        }

                        // Bottom wall
                        if (block.BoardPosition.Y >= TetrisGame.BlocksHigh)
                        {
                            shape.Move(Direction.Up);

                            shape.Save();

                            return;
                        }

                        // Other blocks
                        if (Cells[(int) block.BoardPosition.Y, (int) block.BoardPosition.X] != null)
                        {
                            // We don't want to save to board if there only were horizontal movement
                            bool save = !((int) shape.Position.Y == (int) shape.LastPosition.Y &&
                                          (int) shape.Position.X != (int) shape.LastPosition.X);

                            if (Player.Rotated)
                            {
                                shape.Direction = shape.LastDirection;
                            }
                            else
                            {
                                shape.ResetPosition();

                                if (save)
                                    shape.Save();
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
                    ShapesQueue.Enqueue(ShapesFactory.CreateShape(this, (ShapeTypes) _random.Next(7)));
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
