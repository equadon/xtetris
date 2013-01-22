﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Shapes
{
    public class OShape : Shape
    {
        public OShape(Texture2D texture)
            : base(texture)
        {
            Color = new Color(255, 255, 0);

            List<int[,]> rotations = new List<int[,]>
                {
                    new int[3, 4]
                    {
                        {0,1,1,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    },
                    new int[3, 4]
                    {
                        {0,1,1,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    },
                    new int[3, 4]
                    {
                        {0,1,1,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    },
                    new int[3, 4]
                    {
                        {0,1,1,0},
                        {0,1,1,0},
                        {0,0,0,0}
                    }
                };

            GenerateBlocks(rotations);
        }
    }
}
