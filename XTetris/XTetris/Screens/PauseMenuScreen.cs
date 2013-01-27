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
            var optionsMenuEntry = new MenuEntry("Options");
            var quitMenuEntry = new MenuEntry("Quit Game");

            resumeMenuEntry.Selected += OnCancel;
            optionsMenuEntry.Selected += OptionsEntrySelected;
            quitMenuEntry.Selected += QuitGameEntrySelected;

            MenuEntries.Add(resumeMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitMenuEntry);
        }

        protected void OptionsEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen((TetrisScreenManager)ScreenManager), null);
        }

        protected void QuitGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ((TetrisScreenManager)ScreenManager).Game.Exit();
        }
    }
}
