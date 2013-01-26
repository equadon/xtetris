using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Valekhz.ScreenManagement;
using Valekhz.Tetris.Shapes;

namespace Valekhz.Tetris.Screens
{
    public class GameplayScreen : GameScreen
    {
        private float _pauseAlpha;

        // Variables to handle auto move shape downward
        private const double MoveDownDelay = 1.0d;
        private double _moveDelayDuration = MoveDownDelay;

        // Easier access to the content manager instance
        public ContentManager Content
        {
            get { return ScreenManager.Content; }
        }

        // Input actions
        public InputAction PauseAction { get; private set; }

        public InputAction MoveLeftAction { get; private set; }
        public InputAction MoveRightAction { get; private set; }

        public InputAction RotateLeftAction { get; private set; }
        public InputAction RotateRightAction { get; private set; }

        public InputAction SoftDropAction { get; private set; }
        public InputAction HardDropAction { get; private set; }

        public InputAction HoldAction { get; private set; }

        // Gameplay contents
        public SpriteFont GameFont { get; private set; }

        public Texture2D BorderTexture { get; private set; }
        public Texture2D BlockTexture { get; private set; }
        public Texture2D EmptyBlockTexture { get; private set; }
        public Texture2D CancelTexture { get; private set; }

        public Board Board { get; private set; }
        public Player Player { get; private set; }

        public bool PlayerRotated { get; private set; }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            PauseAction = new InputAction(null, new Keys[] { Keys.Escape }, true);

            MoveLeftAction = new InputAction(null, new Keys[] { Keys.Left, Keys.A }, true);
            MoveRightAction = new InputAction(null, new Keys[] { Keys.Right, Keys.D }, true);

            RotateLeftAction = new InputAction(null, new Keys[] { Keys.LeftControl, Keys.RightControl, Keys.Z }, true);
            RotateRightAction = new InputAction(null, new Keys[] { Keys.Up, Keys.X }, true);

            SoftDropAction = new InputAction(null, new Keys[] { Keys.Down }, true);
            HardDropAction = new InputAction(null, new Keys[] { Keys.Space }, true);

            HoldAction = new InputAction(null, new Keys[] { Keys.LeftShift, Keys.RightShift, Keys.C }, true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                GameFont = Content.Load<SpriteFont>(@"Fonts\Game");

                BorderTexture = Content.Load<Texture2D>(@"Textures\border");
                BlockTexture = Content.Load<Texture2D>(@"Textures\block");
                EmptyBlockTexture = Content.Load<Texture2D>(@"Textures\empty");
                CancelTexture = Content.Load<Texture2D>(@"Textures\cancel");

                NewGame();
            }
        }

        public override void Unload()
        {
            Content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (!Player.HasShape)
                    _moveDelayDuration = MoveDownDelay;
                else
                    _moveDelayDuration -= gameTime.ElapsedGameTime.TotalSeconds;

                if (Player.HasShape && _moveDelayDuration <= 0d)
                {
                    _moveDelayDuration = 0.5 + (11 - Player.Level) * 0.05;
                    Player.Shape.Move(Direction.Down);
                }

                Player.Update(gameTime);
                Board.Update(gameTime);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player
            int activePlayerIndex = (int) ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[activePlayerIndex];

            PlayerIndex playerIndex;

            // Pause the game if user presses the pause key
            if (PauseAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

            if (Player.HasShape)
            {
                PlayerRotated = false;

                // Move left
                if (MoveLeftAction.Evaluate(input, ControllingPlayer, out playerIndex))
                    Player.Shape.Move(Direction.Left);

                // Move right
                if (MoveRightAction.Evaluate(input, ControllingPlayer, out playerIndex))
                    Player.Shape.Move(Direction.Right);

                // Rotate left
                if (RotateLeftAction.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    Player.Shape.Rotate(Direction.Left);
                    PlayerRotated = true;
                }

                // Rotate right
                if (RotateRightAction.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    Player.Shape.Rotate(Direction.Right);
                    PlayerRotated = true;
                }

                // Soft drop
                if (SoftDropAction.Evaluate(input, ControllingPlayer, out playerIndex))
                    Player.Shape.Move(Direction.Down);

                // Hard drop
                if (HardDropAction.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    int distance = Player.Drop();
                }

                // Hold shape
                if (HoldAction.Evaluate(input, ControllingPlayer, out playerIndex))
                    Player.Hold();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            Board.Draw(gameTime, ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public void NewGame()
        {
            Player = new Player(this);

            Board = new Board(this, Player);
            Board.LoadContent();
        }
    }
}
