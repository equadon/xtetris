using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Valekhz.Tetris.Screens;
using Valekhz.Tetris.Shapes;

namespace Valekhz.Tetris
{
    public class Player
    {
        private const float HoldShapeScale = 0.85f;

        // player needs to clear [line count] * 5 to advance to next lvl
        private const int LineCountLevelModifier = 5;

        // stored as (# of lines cleared, back-to-back count)
        private Tuple<int, int> _lastLineClearCount;

        // Auto move ActiveShape down
        private const double MoveDownDelay = 1.0d;
        private double _moveDelayDuration = MoveDownDelay;

        public GameplayScreen Screen { get; private set; }

        // Shape currently attached to player
        public BaseShape Shape { get; set; } // TODO: change back to private setter when hold and queue is in Player

        public bool HasShape
        {
            get { return Shape != null; }
        }

        // Hold shape
        public bool AllowHold { get; set; }
        public BaseShape HoldShape { get; private set; }

        private double _cancelIconDuration = 0d;

        public bool GhostShapeEnabled { get; private set; }

        // Score + Levels
        public int Score { get; private set; }
        public int Level { get; private set; }

        public int TotalLinesCleared { get; private set; }

        public bool Rotated { get; private set; }

        public int LinesToNextLevel { get; private set; }

        public Player(GameplayScreen screen)
        {
            Screen = screen;

            Score = 0;
            Level = 1;

            TotalLinesCleared = 0;

            LinesToNextLevel = Level * 5;

            _lastLineClearCount = new Tuple<int, int>(0, 0);

            AllowHold = true;
            Rotated = false;
            GhostShapeEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            // Advance to next level?
            if (LinesToNextLevel <= 0)
                NextLevel();

            if (_cancelIconDuration > 0d)
                _cancelIconDuration -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw active shape
            if (HasShape)
                Screen.Board.DrawShape(spriteBatch, Shape, new Vector2(Screen.Board.Bounds.Left, Screen.Board.Bounds.Top), 1.0f);

            // Draw border
            if (!TetrisGame.Debug)
                spriteBatch.Draw(Screen.BorderTexture, new Rectangle(TetrisGame.BoardPaddingSide, TetrisGame.BoardPaddingTop, 604, 707), Color.White);

            if (HoldShape != null)
                Screen.Board.DrawShape(
                    spriteBatch,
                    HoldShape,
                    new Vector2(
                        TetrisGame.BoardPaddingSide - HoldShape.Bounds.X * TetrisGame.BlockSize * HoldShapeScale + 10 + 60 - HoldShape.Origin.X * HoldShapeScale,
                        TetrisGame.BoardPaddingTop - HoldShape.Bounds.Y * TetrisGame.BlockSize * HoldShapeScale + 73 + 60 - HoldShape.Origin.Y * HoldShapeScale),
                    HoldShapeScale);

            // User tried to hold when hold was not allowed
            if (_cancelIconDuration > 0d)
                spriteBatch.Draw(
                    Screen.CancelTexture,
                    new Vector2(
                        TetrisGame.BoardPaddingSide + 9 + 62 - Screen.CancelTexture.Width / 2f,
                        TetrisGame.BoardPaddingTop + 72 + 65 - Screen.CancelTexture.Height / 2f),
                    Color.White);
        }

        #region Hold Shape

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
                HoldShape = Shape;
                Shape = null;
            }
            else
            {
                BaseShape tmp = Shape;
                Shape = HoldShape;
                HoldShape = tmp;
            }

            HoldShape.Position = new Vector2(0, 0);

            Screen.Board.SpawnShape(Shape);

            AllowHold = false;
        }

        #endregion

        #region Score System

        public void SoftDrop(int dropHeight)
        {
            Score += 1 * dropHeight;
        }

        public void HardDrop(int dropHeight)
        {
            Score += 2 * dropHeight;
        }

        /// <summary>
        /// Calculate points for a Line Clear and subtract the number of lines
        /// from _linesToNextLevel.
        /// </summary>
        /// <param name="lineCount"></param>
        public void LineClear(int lineCount)
        {
            if (lineCount > 0)
            {
                if (_lastLineClearCount.Item1 == lineCount)
                    _lastLineClearCount = Tuple.Create(_lastLineClearCount.Item1, _lastLineClearCount.Item2 + 1);
                else
                    _lastLineClearCount = Tuple.Create(lineCount, 0);

                int points = lineCount * 200 - 100;

                if (lineCount >= 4)
                    points += 100; // bonus points for tetris

                // TODO: Calculate new level before adding points?
                int lineClearValue = points / 100;

                LinesToNextLevel -= lineClearValue;

                TotalLinesCleared += lineCount;

                Score += points * Level;
            }
        }

        #endregion

        #region Level System

        private void NextLevel()
        {
            if (LinesToNextLevel <= 0)
            {
                Level++;

                LinesToNextLevel = Level * LineCountLevelModifier + LinesToNextLevel;
            }
        }

        #endregion
    }
}
