﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Shapes
{
    public class LShape : BaseShape
    {
        public LShape(Texture2D texture, Board board)
            : base(texture, board)
        {
            Color = new Color(255, 170, 0);

            List<int[,]> rotations = new List<int[,]>
                {
                    new int[3, 3]
                    {
                        {0,0,1},
                        {1,1,1},
                        {0,0,0}
                    },
                    new int[3, 3]
                    {
                        {0,1,0},
                        {0,1,0},
                        {0,1,1}
                    }
                };

            GenerateBlocks(rotations);
        }
    }
}
