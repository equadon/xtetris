using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris
{
    public class Tetromino
    {
        private Texture2D _texture;
        private readonly Mino[,] _minos;

        public Color Color { get; private set; }

        public Tetromino(Texture2D texture, Color color, int size)
        {
            _texture = texture;
            _minos = new Mino[size, size];
            Color = color;
        }

        public void AddMino(int row, int col)
        {
            Mino mino = new Mino(_texture, this, Color, row, col);

            _minos[row, col] = mino;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var mino in _minos)
                if (mino != null)
                    mino.Draw(gameTime, spriteBatch);
        }
    }
}
