using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
// using StarSmuggler.Screens;
using StarSmuggler.UI;
using System.Collections.Generic;

namespace StarSmuggler.Screens
{
    public class MainMenuScreen : IScreen
    {
        private SpriteFont font;
        private Texture2D buttonTexture;
        private Texture2D backgroundTexture;
        private Texture2D logoTexture;
        private List<Button> buttons;
        private GraphicsDevice graphicsDevice;

        private Song currentSong;
        
        private string[] labels = new[] { "New Game", "Load Game", "Save Game", "Quit" };

        public void Refresh(ContentManager content)
        {

        }
        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            currentSong = content.Load<Song>($"Music/singularity");
            Game1.AudioManager.PlaySong("singularity");
            Game1.AudioManager.LoadSfx("click");
            font = content.Load<SpriteFont>("Fonts/TerminalBold");
            buttonTexture = content.Load<Texture2D>("UI/button");
            backgroundTexture = content.Load<Texture2D>("UI/MainMenu");
            logoTexture = content.Load<Texture2D>("UI/logo1");
            buttons = new List<Button>();
            int startY = 450;
            int spacing = 70;
            this.graphicsDevice = graphics;

            for (int i = 0; i < labels.Length; i++)
            {
                var rect = new Rectangle(700, startY + i * spacing, 200, 50);
                buttons.Add(new Button(rect, labels[i], font, buttonTexture));
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Update(gameTime);

                if (buttons[i].WasClicked)
                {
                    HandleClick(i);
                }
            }
        }

        private void HandleClick(int index)
        {
            Game1.AudioManager.PlaySfx("click");
            switch (labels[index])
            {
                case "New Game":
                    // Sleep for 0.5 seconds to allow the click sound to play
                    System.Threading.Thread.Sleep(300);
                    GameManager.Instance.StartNewGame();
                    break;
                case "Load Game":
                    GameManager.Instance.LoadGame();
                    break;
                case "Save Game":
                    SaveLoadManager.SaveGame(GameManager.Instance.Player);
                    break;
                case "Quit":
                    Game1.ExitGame();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(logoTexture, new Rectangle(434, 25, 1463/2, 612/2), Color.White);
            // spriteBatch.DrawString(font, "SPACE SMUGGLER", new Vector2(480, 150), Color.White);

            foreach (var btn in buttons)
                btn.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
