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
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }

        public Color TextColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;
        private float TextScale { get; set; } = 1.0f;
        private int TextPadding { get; set; } = 20; // Padding from the edges
        private static Texture2D cachedPixelTexture = null;

        public InfoPanel(Rectangle bounds, string titleText = null, string descriptionText = null, string eventName = null, string eventDescription = null, SpriteFont font = null, Texture2D texture = null, Color? textColor = null, float textScale = 1.0f, int textPadding = 60)
        {
            Bounds = bounds;
            TitleText = titleText ?? "";
            DescriptionText = descriptionText ?? "";
            EventName = eventName ?? "";
            EventDescription = eventDescription ?? "";
            Font = font;
            Texture = texture;
            TextColor = textColor ?? Color.White;
            TextScale = textScale;
            TextPadding = textPadding;
        }

        public void UpdateText(string newTitleText, string newDescriptionText, string newEventName, string newEventDescription)
        {
            TitleText = newTitleText ?? "";
            DescriptionText = newDescriptionText ?? "";
            EventName = newEventName ?? "";
            EventDescription = newEventDescription ?? "";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background (either with a texture or a solid color)
            if (Texture != null)
                spriteBatch.Draw(Texture, Bounds, Color.White);
            else
                spriteBatch.Draw(GetDefaultTexture(spriteBatch), Bounds, new Color(0, 0, 0, 180)); // Semi-transparent black background

            // Draw the title text with a cyan color to make it stand out
            Vector2 fontSize = Font.MeasureString(TitleText);
            Vector2 titlePosition = new Vector2(Bounds.X + TextPadding, Bounds.Y + TextPadding);
            spriteBatch.DrawString(Font, TitleText, titlePosition, Color.Cyan, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // Draw the description text in a softer white
            if (!string.IsNullOrEmpty(DescriptionText) && Font != null)
            {
                Vector2 descriptionPosition = new Vector2(Bounds.X + TextPadding, Bounds.Y + TextPadding + fontSize.Y * 2);
                string wrappedDescription = WrapText(Font, DescriptionText, Bounds.Width - 2 * TextPadding);
                spriteBatch.DrawString(Font, wrappedDescription, descriptionPosition, Color.LightGray, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
            }
            
            // Draw the event information if available - make it eye-catching!
            if (!string.IsNullOrEmpty(EventName) && !string.IsNullOrEmpty(EventDescription))
            {
                // Calculate position for event section
                Vector2 descHeight = Font.MeasureString(WrapText(Font, DescriptionText, Bounds.Width - 2 * TextPadding));
                Vector2 eventPosition = new Vector2(Bounds.X + TextPadding, Bounds.Y + TextPadding + fontSize.Y * 2 + descHeight.Y + 20);
                
                // Draw a separator line for the event section
                string separator = "--- RECENT EVENT ---";
                spriteBatch.DrawString(Font, separator, eventPosition, Color.Orange, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
                
                // Draw the event name in bright yellow to grab attention
                Vector2 separatorHeight = Font.MeasureString(separator);
                Vector2 eventNamePosition = eventPosition + new Vector2(0, separatorHeight.Y + 10);
                spriteBatch.DrawString(Font, EventName, eventNamePosition, Color.Yellow, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
                
                // Draw the event description in light red to indicate impact
                Vector2 eventNameHeight = Font.MeasureString(EventName);
                Vector2 eventDescPosition = eventNamePosition + new Vector2(0, eventNameHeight.Y + 5);
                string wrappedEventDescription = WrapText(Font, EventDescription, Bounds.Width - 2 * TextPadding);
                spriteBatch.DrawString(Font, wrappedEventDescription, eventDescPosition, Color.LightCoral, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
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
