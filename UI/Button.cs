using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarSmuggler.UI
{
    public class Button
    {
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }

        public Color TextColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.LightGray;

        private MouseState previousMouse;
        public bool IsHovered { get; private set; }
        public bool WasClicked { get; private set; }

        public Button(Rectangle bounds, string text, SpriteFont font, Texture2D texture = null, Color? textColor = null)
        {
            Bounds = bounds;
            Text = text;
            Font = font;
            TextColor = textColor ?? Color.Black; // Default to black if no color is provided
            Texture = texture;
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

            if (Texture != null)
                spriteBatch.Draw(Texture, Bounds, drawColor);
            else
                spriteBatch.Draw(GetDefaultTexture(spriteBatch), Bounds, drawColor);

            // Center the text
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 textPosition = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(Font, Text, textPosition, TextColor);
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
