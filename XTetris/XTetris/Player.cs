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
        #region Properties

        public Shape ActiveTetromino { get; set; }

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

            if (InputHandler.KeyPressed(Keys.Right))
                ActiveTetromino.MoveRight();

            if (InputHandler.KeyPressed(Keys.Left))
                ActiveTetromino.MoveLeft();

            if (InputHandler.KeyPressed(Keys.Up) || InputHandler.KeyPressed(Keys.X))
                ActiveTetromino.RotateRight();

            if (InputHandler.KeyPressed(Keys.LeftControl) ||
                InputHandler.KeyPressed(Keys.RightControl) ||
                InputHandler.KeyPressed(Keys.Z))
                ActiveTetromino.RotateLeft();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (HasActiveTetromino)
                ActiveTetromino.Draw(gameTime, spriteBatch);
        }
    }
}
