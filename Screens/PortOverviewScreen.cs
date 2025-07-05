using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using StarSmuggler.UI;
using StarSmuggler.Events;
using System.Text;
using System;

namespace StarSmuggler.Screens
{
    public class PortOverviewScreen : IScreen
    {
        private SpriteFont font;
        private Texture2D backgroundTexture;
        private Song currentSong;
        private string portName;
        private string portDescription;
        private Texture2D buttonTexture;
        private Texture2D terminalTexture;
        private Terminal terminalWindow;
        private Button continueButton;
        private int terminalX;
        private int terminalY;

         public void Refresh(ContentManager content)
        {
            var currentPort = GameManager.Instance.CurrentPort;
            portDescription = currentPort.Description;
            portName = currentPort.Name;
            backgroundTexture = content.Load<Texture2D>(currentPort.BackgroundImagePath);
            currentSong = content.Load<Song>($"Music/{currentPort.MusicTrackName}");
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            var currentPort = GameManager.Instance.CurrentPort;
            Console.WriteLine($"Loading port: {currentPort.Name}");
            portName = currentPort.Name;
            portDescription = currentPort.Description;

            // Load the current port's music and background image
            currentSong = content.Load<Song>($"Music/{currentPort.MusicTrackName}");
            Game1.AudioManager.PlaySong(currentPort.MusicTrackName);
            backgroundTexture = content.Load<Texture2D>(currentPort.BackgroundImagePath);
            Game1.AudioManager.LoadSfx("click");
            font = content.Load<SpriteFont>("Fonts/Terminal");
            // Load the button and terminal textures
            buttonTexture = content.Load<Texture2D>("UI/button");  // Placeholder button image
            terminalTexture = content.Load<Texture2D>("UI/terminalEmptyNew"); 
            // Calculate the center position for the Terminal
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            
            // Set the size of the continue button
            int buttonWidth = 150; // Width of the button
            int buttonHeight = 50; // Height of the button

            int buttonX = screenWidth - buttonWidth - 20; // 20px padding from the right edge
            int buttonY = screenHeight - buttonHeight - 20; // 20px padding from the bottom edge
            continueButton = new Button(new Rectangle(buttonX, buttonY, buttonWidth, 50), "Continue", font, buttonTexture);
            

            int terminalWidth = 800; // Width of the Terminal
            int terminalHeight = 802; // Height of the Terminal

            terminalX = (screenWidth - terminalWidth) / 2;
            terminalY = (screenHeight - terminalHeight) / 2;

            terminalWindow = new Terminal(new Rectangle(terminalX, terminalY, terminalWidth, terminalHeight), texture: terminalTexture);
        }

        public void Update(GameTime gameTime)
        {
            continueButton.Update(gameTime);

            if (continueButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                GameManager.Instance.SetGameState(GameState.TradeScreen);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();
            int xOffset = terminalX + 100;
            int yOffset = terminalY + 100;;

            // Draw the background texture
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1600, 900), Color.White);
            
            // Draw the terminal window
            terminalWindow.Draw(spriteBatch);

            // Draw the port name
            spriteBatch.DrawString(font, $"Welcome to: {portName}", new Vector2(xOffset, yOffset), Color.Green);

            // Draw the port description and wrap it if necessary
            string wrappedPortDesc = WrapText(font, portDescription, terminalWindow.Bounds.Width - 175); // Add padding
            spriteBatch.DrawString(font, wrappedPortDesc, new Vector2(xOffset, yOffset + 60), Color.LightGreen);
            continueButton.Draw(spriteBatch);

            if (GameManager.Instance.GetLastEvent() is GameEvent evt)
            {
                spriteBatch.DrawString(font, $"EVENT: {evt.Name}", new Vector2(xOffset, yOffset + 140), Color.Yellow);
                // Wrap the evt.Description text
                string wrappedDescription = WrapText(font, evt.Description, terminalWindow.Bounds.Width - 175); // Add padding
                spriteBatch.DrawString(font, wrappedDescription, new Vector2(xOffset, yOffset + 170), Color.LightYellow);
            }           
            spriteBatch.End();
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
    }
}
