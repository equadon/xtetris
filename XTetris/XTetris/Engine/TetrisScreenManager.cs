using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Engine
{
    class TetrisScreenManager : ScreenManager
    {
        /// <summary>
        /// Only needed if this ScreenManager is loaded with the game.
        /// </summary>
        public TetrisGame Game { get; set; }

        public SpriteFont DebugFont { get; private set; }

        public TetrisScreenManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void LoadContent()
        {
            Font = Content.Load<SpriteFont>(@"Fonts\Menu");
            DebugFont = Content.Load<SpriteFont>(@"Fonts\Debug");
            BlankTexture = Content.Load<Texture2D>(@"Textures\blank");

            base.LoadContent();
        }
    }
}
