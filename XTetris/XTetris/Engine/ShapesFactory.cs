using System;
using Microsoft.Xna.Framework.Graphics;

namespace XTetris.Shapes
{
    public enum ShapeTypes { I, J, L, O, S, T, Z, None }

    public static class ShapesFactory
    {
        private static Random _random;

        static ShapesFactory()
        {
            _random = new Random();
        }

        public static BaseShape CreateRandom(Texture2D blockTexture, Board board, ShapeTypes shapeType = ShapeTypes.None)
        {
            if (shapeType == ShapeTypes.None)
                shapeType = (ShapeTypes) _random.Next(7);

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
