using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;

namespace XTetris
{
    public class Board
    {
        #region Fields

        private const float StartingShapeSpeed = 0.8f;

        private float _shapeSpeedIncrease = 0.1f;
        private int _level = 1;

        private Vector2 _position;
        private Rectangle _screenRectangle;

        private readonly Color _backgroundColor = new Color(255, 255, 255);

        public Block[,] Cells { get; private set; }

        #endregion

        #region Properties

        public double ShapeMoveDelay { get; private set; }

        public bool StopMove { get; private set; }

        public BaseShape ActiveShape { get; private set; }

        public Texture2D Texture { get; private set; }

        public Rectangle ScreenRectangle
        {
            get { return _screenRectangle; }
            set { _screenRectangle = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool HasActiveShape
        {
            get { return ActiveShape != null; }
        }

        public static int BoardWidth
        {
            get { return TetrisGame.BlocksWide * TetrisGame.BlockSize; }
        }

        public static int BoardHeight
        {
            get { return TetrisGame.BlocksHigh * TetrisGame.BlockSize; }
        }

        public float Delay
        {
            get { return StartingShapeSpeed + _level * _shapeSpeedIncrease; }
        }

        #endregion

        public Board(Texture2D texture)
        {
            Texture = texture;

            ScreenRectangle = new Rectangle(TetrisGame.BoardPadding/2, TetrisGame.BoardPadding/2, TetrisGame.BlockSize * 10, TetrisGame.BlockSize * 20);

            SpawnShape();

            ShapeMoveDelay = StartingShapeSpeed + _level * _shapeSpeedIncrease;

            Cells = new Block[TetrisGame.BlocksHigh, TetrisGame.BlocksWide];
        }

        public void Update(GameTime gameTime)
        {
            if (!StopMove)
                ShapeMoveDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (HasActiveShape && ShapeMoveDelay <= 0d)
            {
                ActiveShape.MoveDown();
                ShapeMoveDelay = Delay;
            }

            CheckCollisions();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw board
            spriteBatch.Draw(Texture, ScreenRectangle, _backgroundColor);

            // Draw active shape
            if (HasActiveShape)
            {
                ActiveShape.Draw(gameTime, spriteBatch);

                string text = "Pos: (" + ActiveShape.Position.X + "," + ActiveShape.Position.Y + ")" +
                              "\nRect: (" + ActiveShape.Bounds.X + "," + ActiveShape.Bounds.Y + ")" +
                              "\n   Size: " + ActiveShape.Bounds.Width + "x" + ActiveShape.Bounds.Height +
                              "\n   Left/right: " + ActiveShape.Bounds.Left + "/" + ActiveShape.Bounds.Right +
                              "\n   Top/bottom: " + ActiveShape.Bounds.Top + "/" + ActiveShape.Bounds.Bottom;

                spriteBatch.DrawString(XTetris.GameStates.GamePlayState.DebugFont,
                                       text,
                                       new Vector2(0, 200),
                                       Color.Black);

                Vector2 coords = PositionToCoordinates(ActiveShape.Position);
                text = "Pos: (" + ActiveShape.Position.X + "," + ActiveShape.Position.Y + ")" +
                       "\nhas coordinates: (" + coords.X + "," + coords.Y + ")";

                spriteBatch.DrawString(XTetris.GameStates.GamePlayState.DebugFont,
                                       text,
                                       new Vector2(0, 400),
                                       Color.Black);
            }


            for (int row = 0; row <= Cells.GetUpperBound(0); row++)
                for (int col = 0; col <= Cells.GetUpperBound(1); col++)
                    if (Cells[row, col] != null)
                        Cells[row, col].Draw(gameTime, spriteBatch);
        }

        public void SpawnShape()
        {
            ActiveShape = ShapesFactory.CreateRandom(this);

            ActiveShape.Position = new Vector2(
                TetrisGame.BoardPadding / 2f + ScreenRectangle.Width / 2f - ActiveShape.Bounds.Width / 2f - ActiveShape.Bounds.X,
                ActiveShape.Position.Y + TetrisGame.BoardPadding / 2f);

            ActiveShape.PreviousPosition = ActiveShape.Position;
        }

        #region Collision Methods

        public void CheckCollisions()
        {
            if (!HasActiveShape)
                return;

            // Left/right wall
            if (IsCollidingWithLeftWall(ActiveShape) ||
                IsCollidingWithRightWall(ActiveShape))
            {
                ActiveShape.Position = ActiveShape.PreviousPosition;
            }

            // Only store shape if movement is downward and collides with a cell
            if ((int)ActiveShape.Position.Y - (int)ActiveShape.PreviousPosition.Y == 0 &&
                IsCollidingWithCell(ActiveShape))
            {
                ActiveShape.Position = new Vector2(ActiveShape.PreviousPosition.X, ActiveShape.Position.Y);
            }
            if ((int)ActiveShape.Position.Y != (int)ActiveShape.PreviousPosition.Y &&
                IsCollidingWithCell(ActiveShape))
            {
                ActiveShape.Position = ActiveShape.PreviousPosition;

                StoreActiveShape();

                ShapeMoveDelay = Delay;
            }

            // Bottom
            if (IsCollidingWithBottom(ActiveShape))
            {
                ActiveShape.Position = new Vector2(
                    ActiveShape.Position.X,
                    TetrisGame.BoardPadding/2f + BoardHeight - ActiveShape.Bounds.Bottom);

                // Store active shape in Cells
                StoreActiveShape();

                ShapeMoveDelay = Delay;
            }
        }

        public bool IsCollidingWithCell(BaseShape shape)
        {
            var blocks = shape.Rotations[(int) shape.CurrentRotation];

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    var block = blocks[row, col];
                    if (block != null)
                    {
                        Vector2 coords = PositionToCoordinates(block.AbsolutePosition);
                        if (coords.X - 1 < 0 || coords.Y - 1 < 0)
                            continue;

                        if (Cells[(int)coords.Y - 1, (int)coords.X - 1] != null)
                                return true;
                    }
                }
            }

            return false;
        }

        public bool IsCollidingWithLeftWall(BaseShape shape)
        {
            if (shape.Position.X + shape.Bounds.Left < TetrisGame.BoardPadding / 2f)
                return true;
            return false;
        }

        public bool IsCollidingWithRightWall(BaseShape shape)
        {
            if (shape.Position.X + shape.Bounds.Right > TetrisGame.BoardPadding / 2f + Board.BoardWidth)
                return true;
            return false;
        }

        public bool IsCollidingWithBottom(BaseShape shape)
        {
            if (TetrisGame.BoardPadding/2f + shape.Position.Y > TetrisGame.BoardPadding/2f + BoardHeight - shape.Bounds.Bottom)
                return true;
            return false;
        }

        #endregion

        private void StoreActiveShape()
        {
            var blocks = ActiveShape.Rotations[(int) ActiveShape.CurrentRotation];

            for (int row = 0; row <= blocks.GetUpperBound(0); row++)
            {
                for (int col = 0; col <= blocks.GetUpperBound(1); col++)
                {
                    var block = blocks[row, col];
                    if (block != null)
                    {
                        Vector2 coords = PositionToCoordinates(block.AbsolutePosition);
                        Cells[(int)coords.Y - 1, (int)coords.X - 1] = block;
                    }
                }
            }

            SpawnShape();
        }

        private Vector2 PositionToCoordinates(Vector2 position)
        {
            return new Vector2(
                position.X / TetrisGame.BlockSize,
                position.Y / TetrisGame.BlockSize);
        }
    }
}
