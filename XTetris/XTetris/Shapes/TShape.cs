using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Tetrominoes
{
    public class TShape : Shape
    {
        public TShape(Texture2D texture, Color color)
            : base(texture, color)
        {
            // Build
            List<int[,]> rotations = new List<int[,]>
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
                    },
                    new int[3, 3]
                    {
                        {0,0,0},
                        {1,1,1},
                        {0,1,0}
                    },
                    new int[3, 3]
                    {
                        {0,1,0},
                        {1,1,0},
                        {0,1,0}
                    }
                };

            GenerateBlocks(rotations);
        }
    }
}
