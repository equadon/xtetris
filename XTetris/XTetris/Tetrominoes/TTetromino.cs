using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Tetrominoes
{
    public class TTetromino : Tetromino
    {
        public TTetromino(Texture2D texture, Color color)
            : base(texture, color, 3)
        {
            // Build shape
            AddMino(0, 1);
            AddMino(1, 0);
            AddMino(1, 1);
            AddMino(1, 2);
        }
    }
}
