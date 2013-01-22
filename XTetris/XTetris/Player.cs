using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XTetris.Engine;
using XTetris.Tetrominoes;

namespace XTetris
{
    public class Player
    {
        private const double DefaultSpeedDown = 0.6d;

        private double _moveDelay = DefaultSpeedDown;

        #region Properties

        public Tetromino ActiveTetromino { get; set; }

        public bool HasActiveTetromino
        {
            get { return ActiveTetromino != null; }
        }

        #endregion

        public Player()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!HasActiveTetromino)
                return;

            _moveDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_moveDelay <= 0d)
            {
                ActiveTetromino.MoveDown();
                _moveDelay = DefaultSpeedDown;
            }

            if (InputHandler.KeyPressed(Keys.Right))
                ActiveTetromino.MoveRight();

            if (InputHandler.KeyPressed(Keys.Left))
                ActiveTetromino.MoveLeft();

            // Update active tetromino
            ActiveTetromino.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (HasActiveTetromino)
                ActiveTetromino.Draw(gameTime, spriteBatch);
        }
    }
}
