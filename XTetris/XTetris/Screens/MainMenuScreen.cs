using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Screens
{
    class MainMenuScreen : MenuScreen
    {
        private InputAction _menuCancel;

        public MainMenuScreen()
            : base("Main Menu")
        {
            _menuCancel = new InputAction(
                null,
                new Keys[] { Keys.Enter },
                true);
        }

        public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, ScreenManagement.InputState input)
        {
            PlayerIndex playerIndex;

            if (_menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                LoadingScreen.Load(ScreenManager, true, playerIndex, new GameplayScreen());
            }
        }
    }
}
