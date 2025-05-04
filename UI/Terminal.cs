using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarSmuggler.UI
{
    public class Terminal
    {
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }

        public Color TextColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.LightGray;
        private float TextScale { get; set; } = 1.0f; // Default text scale

        private MouseState previousMouse;
        public bool IsHovered { get; private set; }
        public bool WasClicked { get; private set; }
    
        public Terminal(Rectangle bounds, string text = null, SpriteFont font = null, Texture2D texture = null, Color? textColor = null, float textScale = 1.0f)
        {
            Bounds = bounds;
            Text = text; // Text can now be null
            Font = font; // Font can now be null if no text is provided
            Texture = texture;
            TextColor = textColor ?? Color.Black; // Default to black if no color is provided
            TextScale = textScale; // Set the text scale
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentMouse = Mouse.GetState();
            Point mousePos = currentMouse.Position;
            IsHovered = Bounds.Contains(mousePos);

            WasClicked = false;
            if (IsHovered &&
                currentMouse.LeftButton == ButtonState.Pressed &&
                previousMouse.LeftButton == ButtonState.Released)
            {
                WasClicked = true;
            }

            previousMouse = currentMouse;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color drawColor = IsHovered ? HoverColor : BackgroundColor;

            // Draw the background (either with a texture or a solid color)
            if (Texture != null)
                spriteBatch.Draw(Texture, Bounds, drawColor);
            else
                spriteBatch.Draw(GetDefaultTexture(spriteBatch), Bounds, drawColor);

            // Only draw text if it is provided
            if (!string.IsNullOrEmpty(Text) && Font != null)
            {
                // Center the text horizontally and align it to the top
                Vector2 textSize = Font.MeasureString(Text) * TextScale;
                Vector2 textPosition = new Vector2(
                    Bounds.X + (Bounds.Width - textSize.X) / 2, // Center horizontally
                    Bounds.Y + 10 // Align to the top with some padding
                );

                spriteBatch.DrawString(Font, Text, textPosition, TextColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
            }
        }

        // Generates a 1x1 white pixel if no texture is supplied
        private static Texture2D GetDefaultTexture(SpriteBatch spriteBatch)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        }
    }
}
