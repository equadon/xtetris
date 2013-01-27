using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Valekhz.ScreenManagement.Menu;
using Valekhz.Tetris.Engine;

namespace Valekhz.Tetris.Screens
{
    class OptionsMenuScreen : MenuScreen
    {
        private MenuEntry _ghostShapeEntry;
        private MenuEntry _back;

        private TetrisScreenManager _screenManager;

        public OptionsMenuScreen(TetrisScreenManager screenManager)
            : base("Options")
        {
            _ghostShapeEntry = new MenuEntry(string.Empty);
            _back = new MenuEntry("Back");

            // Events
            _ghostShapeEntry.Selected += GhostShapeEntrySelected;
            _back.Selected += OnCancel;

            MenuEntries.Add(_ghostShapeEntry);
            MenuEntries.Add(_back);

            _screenManager = screenManager;

            UpdateText();
        }

        public void UpdateText()
        {
            _ghostShapeEntry.Text = "Ghost Shape: " + (_screenManager.Settings.DisplayGhostPiece ? "Yes" : "No");
        }

        public void GhostShapeEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _screenManager.Settings.DisplayGhostPiece = !_screenManager.Settings.DisplayGhostPiece;

            _screenManager.SaveSettings();

            UpdateText();
        }
    }
}
