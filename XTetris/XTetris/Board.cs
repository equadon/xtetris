using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XTetris.Engine;

namespace XTetris
{
    public class Board
    {
        #region Properties & Fields

        private const float StartingShapeSpeed = 0.8f;

        private float _shapeSpeedIncrease = 0.1f;
        private int _level = 1;

        public double ShapeMoveDelay { get; private set; }

        private Vector2 _position;
        private Rectangle _screenRectangle;

        public bool StopMove { get; private set; }

        private readonly Color _backgroundColor = new Color(255, 255, 255);

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

        #endregion

        public Board(Texture2D texture)
        {
            Texture = texture;

            ScreenRectangle = new Rectangle(TetrisGame.BoardPadding/2, TetrisGame.BoardPadding/2, TetrisGame.BlockSize * 10, TetrisGame.BlockSize * 20);

            SpawnShape();

            ShapeMoveDelay = StartingShapeSpeed + _level * _shapeSpeedIncrease;
        }

        public void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.Space))
                StopMove = !StopMove;

            if (!StopMove)
                ShapeMoveDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (InputHandler.KeyPressed(Keys.Enter))
                SpawnShape();

            if (ShapeMoveDelay <= 0d)
            {
                ActiveShape.MoveDown();
                ShapeMoveDelay = StartingShapeSpeed + _level * _shapeSpeedIncrease;
            }

            CheckCollisionWithWalls();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw board
            spriteBatch.Draw(Texture, ScreenRectangle, _backgroundColor);

            // Draw active shape
            if (HasActiveShape)
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
        }

        public void SpawnShape()
        {
            ActiveShape = ShapesFactory.CreateRandom(this);

            ActiveShape.Position = new Vector2(
                TetrisGame.BoardPadding / 2f + ScreenRectangle.Width / 2f - ActiveShape.Bounds.Width / 2f - ActiveShape.Bounds.X,
                ActiveShape.Position.Y + TetrisGame.BoardPadding / 2f - TetrisGame.BlockSize);
        }

        #region Collision Methods

        public void CheckCollisionWithWalls()
        {
            // Left wall
            if (IsCollidingWithLeftWall(ActiveShape))
            {
                ActiveShape.Position = new Vector2(
                    0 - ActiveShape.Bounds.Left + TetrisGame.BoardPadding/2f,
                    ActiveShape.Position.Y);
            }

            // Right wall
            if (IsCollidingWithRightWall(ActiveShape))
            {
                ActiveShape.Position = new Vector2(
                    TetrisGame.BoardPadding / 2f + BoardWidth - ActiveShape.Bounds.Right,
                    ActiveShape.Position.Y);
            }
        }

        /// <returns>Returns true if shape is colliding with wall.</returns>
        public static bool IsCollidingWithLeftWall(BaseShape shape)
        {
            if (shape.Position.X + shape.Bounds.Left < TetrisGame.BoardPadding / 2f)
                return true;
            return false;
        }

        /// <returns>Returns true if shape is colliding with wall.</returns>
        public static bool IsCollidingWithRightWall(BaseShape shape)
        {
            if (shape.Position.X + shape.Bounds.Right > TetrisGame.BoardPadding / 2f + Board.BoardWidth)
                return true;
            return false;
        }

        #endregion
    }
}
