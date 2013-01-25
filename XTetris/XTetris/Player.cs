using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.Shapes;

namespace XTetris
{
    public class Player
    {
        // player needs to clear [line count] * 5 to advance to next lvl
        private const int LineCountLevelModifier = 5;

        private const double KeyDownDelay = 0.15d;

        private double _softDropTimeLeft = 0d;

        // # of lines left to clear until the level advances
        private int _linesToNextLevel;

        // stored as (# of lines cleared, back-to-back count)
        private Tuple<int, int> _lastLineClearCount;

        public Board Board { get; private set; }

        public int Score { get; private set; }
        public int Level { get; private set; }

        public int TotalLinesCleared { get; private set; }

        public bool Rotated { get; private set; }

        public bool GhostShapeEnabled { get; private set; }

        public int LinesToNextLevel
        {
            get { return _linesToNextLevel; }
        }

        public Player(Board board)
        {
            Board = board;

            Score = 0;
            Level = 1;

            TotalLinesCleared = 0;

            _linesToNextLevel = Level * 5;

            _lastLineClearCount = new Tuple<int, int>(0, 0);

            Rotated = false;
            GhostShapeEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            // Advance to next level?
            if (_linesToNextLevel <= 0)
                NextLevel();

            Rotated = false;

            if (_softDropTimeLeft > 0d)
                _softDropTimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;

            if (Board.HasActiveShape)
            {
                // Move left/right
                if (InputHandler.KeyPressed(Keys.Left))
                    Board.ActiveShape.Move(Direction.Left);
                else if (InputHandler.KeyPressed(Keys.Right))
                    Board.ActiveShape.Move(Direction.Right);

                // Soft drop
                if (InputHandler.KeyPressed(Keys.Down))
                    Board.ActiveShape.Move(Direction.Down);

                if (_softDropTimeLeft <= 0d)
                {
                    if (InputHandler.KeyDown(Keys.Down))
                        Board.ActiveShape.Move(Direction.Down);
                    _softDropTimeLeft = KeyDownDelay;
                }

                // Rotate left
                if (InputHandler.KeyPressed(Keys.LeftControl) || InputHandler.KeyPressed(Keys.LeftControl) ||
                    InputHandler.KeyPressed(Keys.Z))
                {
                    Board.ActiveShape.Rotate(Direction.Left);
                    Rotated = true;
                }

                // Rotate right
                if (InputHandler.KeyPressed(Keys.Up) ||
                    InputHandler.KeyPressed(Keys.X))
                {
                    Board.ActiveShape.Rotate(Direction.Right);
                    Rotated = true;
                }

                // Drop shape
                if (InputHandler.KeyPressed(Keys.Space))
                    Board.ActiveShape.Drop();

                // Hold shape
                if (InputHandler.KeyPressed(Keys.LeftShift) || InputHandler.KeyPressed(Keys.RightShift) ||
                    InputHandler.KeyPressed(Keys.C))
                {
                    Board.Hold();
                }
            }
        }

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

                _linesToNextLevel -= lineClearValue;

                TotalLinesCleared += lineCount;

                Score += points * Level;
            }
        }

        #endregion

        #region Level System

        private void NextLevel()
        {
            if (_linesToNextLevel <= 0)
            {
                Level++;

                _linesToNextLevel = Level * LineCountLevelModifier + _linesToNextLevel;
            }
        }

        #endregion
    }
}
