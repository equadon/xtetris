using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.Shapes;

namespace XTetris
{
    public class Player
    {
        #region Properties

        public Board Board { get; private set; }

        #endregion

        public Player(Board board)
        {
            Board = board;
        }

        public void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Right))
                Board.ActiveShape.MoveRight();

            if (InputHandler.KeyPressed(Keys.Left))
                Board.ActiveShape.MoveLeft();

            if (InputHandler.KeyPressed(Keys.Up) || InputHandler.KeyPressed(Keys.X))
                Board.ActiveShape.RotateRight();

            if (InputHandler.KeyPressed(Keys.Down))
                Board.ActiveShape.MoveDown();

            if (InputHandler.KeyPressed(Keys.LeftControl) ||
                InputHandler.KeyPressed(Keys.RightControl) ||
                InputHandler.KeyPressed(Keys.Z))
                Board.ActiveShape.RotateLeft();
        }
    }
}
