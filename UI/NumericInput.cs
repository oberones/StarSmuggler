using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarSmuggler.UI
{
    public class NumericInput
    {
        public int Value { get; private set; }
        public Rectangle Bounds { get; set; }

        private Button plusButton;
        private Button minusButton;
        private SpriteFont font;

        public NumericInput(Rectangle bounds, SpriteFont font, Texture2D buttonTexture, int initialValue = 1)
        {
            Bounds = bounds;
            Value = initialValue;
            this.font = font;

            var buttonWidth = 30;
            var buttonHeight = bounds.Height;

            minusButton = new Button(
                new Rectangle(bounds.X + 397, bounds.Y + 60, buttonWidth, buttonHeight), "-", font, buttonTexture);

            plusButton = new Button(
                new Rectangle(bounds.X + 397 - (buttonWidth + 15 ), bounds.Y + 60 , buttonWidth, buttonHeight), "+", font, buttonTexture);
        }

        public void Update(GameTime gameTime)
        {
            minusButton.Update(gameTime);
            plusButton.Update(gameTime);

            if (minusButton.WasClicked && Value > 1)
                Value--;

            if (plusButton.WasClicked)
                Value++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            minusButton.Draw(spriteBatch);
            plusButton.Draw(spriteBatch);

            string displayValue = Value.ToString();
            Vector2 textSize = font.MeasureString(displayValue);
            Vector2 textPos = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(font, displayValue, textPos, Color.White);
        }
    }
}
