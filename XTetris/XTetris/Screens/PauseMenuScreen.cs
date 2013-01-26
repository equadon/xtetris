using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Valekhz.ScreenManagement;
using Valekhz.ScreenManagement.Menu;
using Valekhz.Tetris.Engine;

namespace Valekhz.Tetris.Screens
{
    class PauseMenuScreen : MenuScreen
    {
        public PauseMenuScreen()
            : base("Paused")
        {
            var resumeMenuEntry = new MenuEntry("Resume Game");
            var quitMenuEntry = new MenuEntry("Quit Game");

            resumeMenuEntry.Selected += OnCancel;
            quitMenuEntry.Selected += QuitGameEntrySelected;

            MenuEntries.Add(resumeMenuEntry);
            MenuEntries.Add(quitMenuEntry);
        }

        protected void QuitGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ((TetrisScreenManager)ScreenManager).Game.Exit();
        }
    }
}
