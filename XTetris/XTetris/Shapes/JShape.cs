using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Shapes
{
    public class JShape : Shape
    {
        public JShape(Texture2D texture)
            : base(texture)
        {
            Color = new Color(0, 0, 255);

            List<int[,]> rotations = new List<int[,]>
                {
                    new int[3, 3]
                    {
                        {1,0,0},
                        {1,1,1},
                        {0,0,0}
                    },
                    new int[3, 3]
                    {
                        {0,1,1},
                        {0,1,0},
                        {0,1,0}
                    },
                    new int[3, 3]
                    {
                        {0,0,0},
                        {1,1,1},
                        {0,0,1}
                    },
                    new int[3, 3]
                    {
                        {0,1,0},
                        {0,1,0},
                        {1,1,0}
                    }
                };

            GenerateBlocks(rotations);
        }
    }
}
