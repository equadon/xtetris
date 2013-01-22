using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTetris.Engine
{
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
