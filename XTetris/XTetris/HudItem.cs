using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Valekhz.Tetris
{
    public class HudItem
    {
        public readonly Color TitleColor = Color.Crimson;
        public readonly Color ValueColor = Color.Yellow;

        private const float _valueScale = 1.0f;

        public SpriteFont Font { get; private set; }
        public Vector2 Position { get; set; }
        public string Title { get; private set; }

        public HudItem(SpriteFont spriteFont, string title)
        {
            Font = spriteFont;
            Title = title;
        }

        public void Draw(SpriteBatch spriteBatch, int value)
        {
            Rectangle bounds = new Rectangle((int) Position.X, (int) Position.Y, 150, 100);

            Vector2 size = Font.MeasureString(Title);
            Vector2 origin = new Vector2(size.X/2f, 0);
            Vector2 pos = new Vector2(
                bounds.X + bounds.Width/2f,
                bounds.Y + bounds.Height/2f);

            spriteBatch.DrawString(Font, Title, pos, TitleColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            // Draw value
            string num = value.ToString("d");

            size = Font.MeasureString(num);
            origin.X = size.X / 2f;
            pos.Y += 50;

            spriteBatch.DrawString(Font, num, pos, ValueColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
