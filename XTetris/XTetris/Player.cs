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

        public bool Rotated { get; private set; }

        public Player(Board board)
        {
            Board = board;
            Score = 0;
            Rotated = false;
        }

        public void Update(GameTime gameTime)
        {
            Rotated = false;

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
    }
}
