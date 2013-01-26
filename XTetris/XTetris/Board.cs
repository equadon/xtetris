using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Valekhz.Tetris.Engine;
using Valekhz.Tetris.Screens;
using Valekhz.Tetris.Shapes;

namespace Valekhz.Tetris
{
    public class Board
    {
        #region Fields

        public static readonly Color BackgroundColor = new Color(255, 255, 255);

        #endregion

        #region Properties

        public GameplayScreen Screen { get; set; }
        public Player Player { get; private set; }

        public Block[,] Cells { get; private set; }

        public double TotalTime { get; private set; }

        // HUD Items
        public HudItem ScoreHud { get; private set; }
        public HudItem LevelHud { get; private set; }
        public HudItem TimeHud { get; private set; }

        public bool IsGameOver { get; private set; }

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

        public Board(GameplayScreen gameScreen, Player player)
        {
            Screen = gameScreen;
            Player = player;
            Cells = new Block[TetrisGame.BlocksHigh, TetrisGame.BlocksWide];

            IsGameOver = false;
        }

        public void LoadContent()
        {
            Vector2 pos = new Vector2(10, 190);

            ScoreHud = new HudItem(Screen.GameFont, "Score");
            ScoreHud.Position = pos;

            pos.Y += 150;
            LevelHud = new HudItem(Screen.GameFont, "Level");
            LevelHud.Position = pos;

            pos.Y += 150;
            TimeHud = new HudItem(Screen.GameFont, "Time");
            TimeHud.Position = pos;
        }

        public void Update(GameTime gameTime)
        {
            if (IsGameOver)
                return;

            TotalTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (Screen.Player.HasShape)
                CheckCollisions(Player.Shape);

            // See if there are any lines we can clear
            ClearLines();
        }

        #region Draw Methods

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsGameOver)
                return;

            spriteBatch.Draw(Screen.ScreenManager.BlankTexture, Bounds, BackgroundColor);

            // Draw cells
            DrawCells(spriteBatch);

            // Labels
            spriteBatch.DrawString(Screen.GameFont, "Hold",
                new Vector2(
                    TetrisGame.BoardPaddingSide + 20,
                    TetrisGame.BoardPaddingTop + 15),
                Color.Crimson);

            spriteBatch.DrawString(Screen.GameFont, "Next",
                new Vector2(
                    Bounds.Right + 25,
                    TetrisGame.BoardPaddingTop + 15),
                Color.Crimson);

            Player.Draw(gameTime, spriteBatch);

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
                    spriteBatch.Draw(Screen.EmptyBlockTexture, cellPos, Color.White);

                    if (block != null)
                        spriteBatch.Draw(Screen.BlockTexture, cellPos, block.Color);
                }
            }
        }

        public void DrawShape(SpriteBatch spriteBatch, BaseShape shape, Vector2 position, float scale)
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
                        if (Screen.Player.GhostShapeEnabled && shape.Equals(Player.Shape))
                        {
                            Vector2 ghostBlockPos = new Vector2(
                                position.X + (block.BoardPosition.X * TetrisGame.BlockSize) * scale,
                                position.Y + (ghostPos.Y + block.Position.Y) * TetrisGame.BlockSize * scale);

                            spriteBatch.Draw(Screen.BlockTexture,
                                ghostBlockPos,
                                null,
                                Color.Gray,
                                0f,
                                Vector2.Zero,
                                scale,
                                SpriteEffects.None,
                                0f);
                        }

                        spriteBatch.Draw(Screen.BlockTexture,
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

        private void DrawHud(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Score
            ScoreHud.Draw(spriteBatch, Screen.Player.Score);

            // Time
            LevelHud.Draw(spriteBatch, Screen.Player.Level);

            // Time
            TimeHud.Draw(spriteBatch, Convert.ToInt32(TotalTime));
        }

        #endregion

        // TODO: Implement a better game over checking
        public void GameOver()
        {
            IsGameOver = true;
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
                Screen.Player.LineClear(linesCleared);

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
                            if (Screen.Player.Rotated)
                                shape.Direction = shape.LastDirection;
                            else
                                shape.Move(Direction.Right);

                            return;
                        }

                        // Right wall
                        if (block.BoardPosition.X >= TetrisGame.BlocksWide)
                        {
                            if (Screen.Player.Rotated)
                                shape.Direction = shape.LastDirection;
                            else
                                shape.Move(Direction.Left);

                            return;
                        }

                        // Top wall
                        if (block.BoardPosition.Y < 0)
                        {
                            if (Screen.Player.Rotated)
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

                            if (Screen.Player.Rotated)
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
    }
}
