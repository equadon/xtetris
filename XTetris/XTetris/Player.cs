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
        public Board Board { get; private set; }

        public int Score { get; private set; }

        public Player(Board board)
        {
            Board = board;
            Score = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Left))
                Board.ActiveShape.Move(Direction.Left);

            if (InputHandler.KeyPressed(Keys.Right))
                Board.ActiveShape.Move(Direction.Right);

            if (InputHandler.KeyPressed(Keys.Up))
                Board.ActiveShape.Move(Direction.Up);

            if (InputHandler.KeyPressed(Keys.Down))
                Board.ActiveShape.Move(Direction.Down);
        }
    }
}
