using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Valekhz.Tetris.Engine
{
    public enum MouseButton { Left, Middle, Right }

    public class InputHandler : GameComponent
    {
        #region Properties

        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState LastKeyboardState { get; private set; }

        public static MouseState MouseState { get; private set; }
        public static MouseState LastMouseState { get; private set; }

        public static GamePadState[] GamePadStates { get; private set; }
        public static GamePadState[] LastGamePadStates { get; private set; }

        #endregion

        public InputHandler(Game game)
            : base(game)
        {
            KeyboardState = Keyboard.GetState();

            MouseState = Mouse.GetState();

            GamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                GamePadStates[(int)index] = GamePad.GetState(index);
        }

        #region XNA Methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();

            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            LastGamePadStates = (GamePadState[]) GamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof (PlayerIndex)))
                GamePadStates[(int) index] = GamePad.GetState(index);

            base.Update(gameTime);
        }

        #endregion

        public static void Flush()
        {
            LastKeyboardState = KeyboardState;
            LastMouseState = MouseState;
        }

        #region Keyboard Methods

        public static bool KeyReleased(Keys key)
        {
            return KeyboardState.IsKeyUp(key) &&
                   LastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key) &&
                   LastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        #endregion

        #region Mouse Methods

        public static bool MouseReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Released &&
                           LastMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Released &&
                           LastMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Released &&
                           LastMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool MousePressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Pressed &&
                           LastMouseState.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Pressed &&
                           LastMouseState.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Pressed &&
                           LastMouseState.RightButton == ButtonState.Released;
            }
            return false;
        }

        public static bool MouseDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }

        #endregion

        #region GamePad Methods

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return GamePadStates[(int) index].IsButtonUp(button) &&
                   LastGamePadStates[(int) index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return GamePadStates[(int) index].IsButtonDown(button) &&
                   LastGamePadStates[(int) index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return GamePadStates[(int) index].IsButtonDown(button);
        }

        #endregion
    }
}
