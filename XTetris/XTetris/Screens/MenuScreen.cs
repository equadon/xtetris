using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        public string Title { get; private set; }
        public Vector2 TitlePosition { get; protected set; }

        public SpriteFont MenuFont { get; private set; }

        public MenuScreen(string title)
        {
            Title = title;
            TitlePosition = new Vector2(20, 20);
        }

        public override void Activate(bool instancePreserved)
        {
            MenuFont = ScreenManager.Content.Load<SpriteFont>(@"Fonts\Menu");
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = ScreenManager.SpriteBatch;

            batch.Begin();

            batch.DrawString(MenuFont, Title, TitlePosition, Color.Black);

            batch.End();
        }
    }
}
