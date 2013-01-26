﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Valekhz.Tetris.Engine;
using Valekhz.Tetris.Screens;
using Valekhz.Tetris.Shapes;

namespace Valekhz.Tetris
{
    public class Player
    {
        // player needs to clear [line count] * 5 to advance to next lvl
        private const int LineCountLevelModifier = 5;

        // stored as (# of lines cleared, back-to-back count)
        private Tuple<int, int> _lastLineClearCount;

        // Auto move ActiveShape down
        private const double MoveDownDelay = 1.0d;
        private double _moveDelayDuration = MoveDownDelay;

        public GameplayScreen Screen { get; private set; }

        public int Score { get; private set; }
        public int Level { get; private set; }

        public int TotalLinesCleared { get; private set; }

        public bool Rotated { get; private set; }

        public bool GhostShapeEnabled { get; private set; }

        public int LinesToNextLevel { get; private set; }

        // Shape currently attached to player
        public BaseShape Shape { get; set; } // TODO: change back to private setter when hold and queue is in Player

        public bool HasShape
        {
            get { return Shape != null; }
        }

        public Player(GameplayScreen screen)
        {
            Screen = screen;

            Score = 0;
            Level = 1;

            TotalLinesCleared = 0;

            LinesToNextLevel = Level * 5;

            _lastLineClearCount = new Tuple<int, int>(0, 0);

            Rotated = false;
            GhostShapeEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            // Advance to next level?
            if (LinesToNextLevel <= 0)
                NextLevel();
        }

        #region Active Shape

        public void ClearShape()
        {
            Shape = null;
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
