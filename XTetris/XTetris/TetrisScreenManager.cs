using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Valekhz.ScreenManagement;

namespace Valekhz.Tetris
{
    class TetrisScreenManager : ScreenManager
    {
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
