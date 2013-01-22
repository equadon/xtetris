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

        public static Shape CreateRandom(Texture2D blockTexture, ShapeTypes shapeType = ShapeTypes.None)
        {
            if (shapeType == ShapeTypes.None)
                shapeType = (ShapeTypes) _random.Next(7);

            switch (shapeType)
            {
                case ShapeTypes.I:
                    return new IShape(blockTexture);
                case ShapeTypes.J:
                    return new JShape(blockTexture);
                case ShapeTypes.T:
                    return new TShape(blockTexture);
                default:
                    return new JShape(blockTexture);
            }
        }
    }
}
