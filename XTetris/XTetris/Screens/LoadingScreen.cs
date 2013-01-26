using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Valekhz.ScreenManagement;
using Valekhz.Tetris.Engine;

namespace Valekhz.Tetris.Screens
{
    class LoadingScreen : GameScreen
    {
        private bool _loadingIsSlow;
        private bool _otherScreensAreGone;

        private GameScreen[] _screensToLoad;

        private Texture2D _backgroundTexture;

        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {
            // Tell all screens to transition off
            foreach (var screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen
            var loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                _backgroundTexture = ((TetrisScreenManager)ScreenManager).Content.Load<Texture2D>(@"Backgrounds\loading");
            }
        }

        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                    if (screen != null)
                        ScreenManager.AddScreen(screen, ControllingPlayer);
            }
        }
        
        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                Color color = Color.White * TransitionAlpha;

                // Draw the background.
                spriteBatch.Begin();
                spriteBatch.Draw(_backgroundTexture, Vector2.Zero, color);
                spriteBatch.End();
            }
        }
    }
}
