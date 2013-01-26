using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Valekhz.ScreenManagement;

namespace Valekhz.Tetris
{
    class TetrisScreenManager : ScreenManager
    {
        /// <summary>
        /// Only needed if this ScreenManager is loaded with the game.
        /// </summary>
        public TetrisGame Game { get; set; }

        public TetrisScreenManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void LoadContent()
        {
            Font = Content.Load<SpriteFont>(@"Fonts\Menu");
            BlankTexture = Content.Load<Texture2D>(@"Textures\blank");

            base.LoadContent();
        }
    }
}
