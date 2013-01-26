using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valekhz.Tetris.Shapes
{
    public class TShape : BaseShape
    {
        public TShape(Texture2D texture, Board board)
            : base(texture, board, TetrisGame.TShapeColor)
        {
            var rotations = new List<int[,]>
            {
                new int[3, 3]
                {
                    {0,1,0},
                    {1,1,1},
                    {0,0,0}
                },
                new int[3, 3]
                {
                    {0,1,0},
                    {0,1,1},
                    {0,1,0}
                }
            };

            GenerateBlocks(rotations);
        }
    }
}
