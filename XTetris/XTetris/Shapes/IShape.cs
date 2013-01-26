using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valekhz.Tetris.Shapes
{
    public class IShape : BaseShape
    {
        public IShape(Texture2D texture, Board board)
            : base(texture, board, TetrisGame.ShapeIColor)
        {
            var rotations = new List<int[,]>
            {
                new int[4, 4]
                {
                    {0,0,0,0},
                    {1,1,1,1},
                    {0,0,0,0},
                    {0,0,0,0}
                },
                new int[4, 4]
                {
                    {0,0,1,0},
                    {0,0,1,0},
                    {0,0,1,0},
                    {0,0,1,0}
                }
            };

            GenerateBlocks(rotations);
        }
    }
}
