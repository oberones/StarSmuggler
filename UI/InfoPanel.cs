using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using System;

namespace StarSmuggler.UI
{
    public class InfoPanel
    {
        public Rectangle Bounds { get; set; }
        public string TitleText { get; set; }
        public string DescriptionText { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }

        public Color TextColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;
        private float TextScale { get; set; } = 1.0f;
        private int TextPadding { get; set; } = 20; // Padding from the edges
        private static Texture2D cachedPixelTexture = null;

        public InfoPanel(Rectangle bounds, string titleText = null, string descriptionText = null, SpriteFont font = null, Texture2D texture = null, Color? textColor = null, float textScale = 1.0f, int textPadding = 60)
        {
            Bounds = bounds;
            TitleText = titleText ?? "";
            DescriptionText = descriptionText ?? "";
            Font = font;
            Texture = texture;
            TextColor = textColor ?? Color.White;
            TextScale = textScale;
            TextPadding = textPadding;
        }

        public void UpdateText(string newTitleText, string newDescriptionText)
        {
            TitleText = newTitleText ?? "";
            DescriptionText = newDescriptionText ?? "";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background (either with a texture or a solid color)
            if (Texture != null)
                spriteBatch.Draw(Texture, Bounds, Color.White);
            else
                spriteBatch.Draw(GetDefaultTexture(spriteBatch), Bounds, new Color(0, 0, 0, 180)); // Semi-transparent black background

            // Draw the title text
            Vector2 fontSize = Font.MeasureString(TitleText);
            Vector2 titlePosition = new Vector2(Bounds.X + TextPadding, Bounds.Y + TextPadding);
            spriteBatch.DrawString(Font, TitleText, titlePosition, TextColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // Draw the description text
            if (!string.IsNullOrEmpty(DescriptionText) && Font != null)
            {
                Vector2 descriptionPosition = new Vector2(Bounds.X + TextPadding, Bounds.Y + TextPadding + fontSize.Y * 2);
                string wrappedDescription = WrapText(Font, DescriptionText, Bounds.Width - 2 * TextPadding);
                spriteBatch.DrawString(Font, wrappedDescription, descriptionPosition, TextColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
            }
        }

        // Helper method to wrap text
        private string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder wrappedText = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);
                if (lineWidth + size.X < maxLineWidth)
                {
                    wrappedText.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    wrappedText.AppendLine();
                    wrappedText.Append(word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return wrappedText.ToString();
        }

        // Generates a 1x1 white pixel if no texture is supplied
        private static Texture2D GetDefaultTexture(SpriteBatch spriteBatch)
        {
            if (cachedPixelTexture == null)
            {
                cachedPixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                cachedPixelTexture.SetData(new[] { Color.White });
            }
            return cachedPixelTexture;
        }
    }
}
