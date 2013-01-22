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

        public Shape ActiveShape { get; set; }

        public bool HasActiveTetromino
        {
            get { return ActiveShape != null; }
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
                ActiveShape.MoveRight();

            if (InputHandler.KeyPressed(Keys.Left))
                ActiveShape.MoveLeft();

            if (InputHandler.KeyPressed(Keys.Up) || InputHandler.KeyPressed(Keys.X))
                ActiveShape.RotateRight();

            if (InputHandler.KeyPressed(Keys.Down))
                ActiveShape.MoveDown();

            if (InputHandler.KeyPressed(Keys.LeftControl) ||
                InputHandler.KeyPressed(Keys.RightControl) ||
                InputHandler.KeyPressed(Keys.Z))
                ActiveShape.RotateLeft();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (HasActiveTetromino)
                ActiveShape.Draw(gameTime, spriteBatch);
        }
    }
}
