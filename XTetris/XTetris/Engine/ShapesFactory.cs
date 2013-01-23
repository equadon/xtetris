using System;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Shapes;

namespace XTetris
{
    public enum ShapeTypes { I, J, L, O, S, T, Z }
}

namespace XTetris.Engine
{
    public static class ShapesFactory
    {
        public static BaseShape CreateShape(Board board, ShapeTypes shapeType)
        {
            var blockTexture = board.GameState.BlockTexture;

            switch (shapeType)
            {
                case ShapeTypes.I:
                    return new IShape(blockTexture, board);
                case ShapeTypes.J:
                    return new JShape(blockTexture, board);
                case ShapeTypes.L:
                    return new LShape(blockTexture, board);
                case ShapeTypes.O:
                    return new OShape(blockTexture, board);
                case ShapeTypes.S:
                    return new SShape(blockTexture, board);
                case ShapeTypes.T:
                    return new TShape(blockTexture, board);
                case ShapeTypes.Z:
                    return new ZShape(blockTexture, board);
                default:
                    return null;
            }
        }
    }
}
