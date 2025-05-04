using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarSmuggler.UI
{
    public class BackButton
    {
        private Button button;

        public BackButton(SpriteFont font, Texture2D texture, int x, int y, int sizeX, int sizeY)
        {
            Rectangle rect = new Rectangle(x, y, sizeX, sizeY);
            button = new Button(rect, "Back", font, texture, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            button.Update(gameTime);

            if (button.WasClicked)
            {
                GameManager.Instance.ReturnToPreviousState();
                Game1.AudioManager?.PlaySfx("click");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            button.Draw(spriteBatch);
        }
    }
}
