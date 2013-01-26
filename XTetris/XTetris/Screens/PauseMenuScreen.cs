using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Screens
{
    class PauseMenuScreen : MenuScreen
    {
        public PauseMenuScreen()
            : base("Paused")
        {
            TitlePosition = new Vector2(150, 300);
        }
    }
}
