﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Shapes
{
    public class IShape : Shape
    {
        public IShape(Texture2D texture)
            : base(texture)
        {
            Color = new Color(0, 255, 255);

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
