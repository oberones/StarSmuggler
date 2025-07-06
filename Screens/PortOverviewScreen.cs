using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using StarSmuggler.UI;
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
        private Button visitTraderButton;
        private Button returnToShipButton;
        private Button refuelShipButton;
        private InfoPanel infoPanel;
        private Texture2D infoPanelTexture;
        private Texture2D iconTradeTexture;
        private Texture2D iconShipTexture;
        private Texture2D iconFuelTexture;
        private GraphicsDevice graphicsDevice;

         public void Refresh(ContentManager content)
        {
            var currentPort = GameManager.Instance.CurrentPort;
            portDescription = currentPort.Description;
            portName = currentPort.Name;
            backgroundTexture = content.Load<Texture2D>(currentPort.BackgroundImagePath);
            currentSong = content.Load<Song>($"Music/{currentPort.MusicTrackName}");

            // Update the info panel with the current port information
            if (infoPanel != null)
            {
                string panelTitle = $"Welcome to {portName}";
                infoPanel.UpdateText(panelTitle, portDescription);
            }
        }

        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            // Cache the graphics device for later use
            this.graphicsDevice = graphics;
            // Load the name and description of the current port
            var currentPort = GameManager.Instance.CurrentPort;
            Console.WriteLine($"Loading port: {currentPort.Name}");
            portName = currentPort.Name;
            portDescription = currentPort.Description;

            // Load the music and sound effects
            currentSong = content.Load<Song>($"Music/{currentPort.MusicTrackName}");
            Game1.AudioManager.LoadSfx("click");

            // Load the fonts and textures
            backgroundTexture = content.Load<Texture2D>(currentPort.BackgroundImagePath);
            font = content.Load<SpriteFont>("Fonts/Terminal");
            buttonTexture = content.Load<Texture2D>("UI/button");
            infoPanelTexture = content.Load<Texture2D>("UI/infoPanel");
            
            // Load the icon textures
            iconTradeTexture = content.Load<Texture2D>("UI/iconTrade");
            iconShipTexture = content.Load<Texture2D>("UI/iconShip");
            iconFuelTexture = content.Load<Texture2D>("UI/iconFuel");

            // Play the current port's music
            Game1.AudioManager.PlaySong(currentPort.MusicTrackName);
            
            // Calculate the screen dimensions
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            
            // Set the size of the info panel
            int panelWidth = 700;
            int panelHeight = 558;
            int panelX = (screenWidth - panelWidth) / 2; // Center horizontally
            int panelY = (screenHeight - panelHeight) / 4;

            // Initialize the info panel
            string panelTitle = $"Welcome to {portName}";
            infoPanel = new InfoPanel(new Rectangle(panelX, panelY, panelWidth, panelHeight), titleText: panelTitle, descriptionText: portDescription, font: font, texture: infoPanelTexture);

            // Set the size of the buttons
            int buttonWidth = 200; // Width of the buttons
            int buttonHeight = 60; // Height of the buttons
            int buttonSpacing = 30; // Space between buttons
            int iconPadding = 10; // Space between icons and buttons

            // Calculate total width needed for all buttons
            int totalButtonWidth = (3 * buttonWidth) + (2 * buttonSpacing);
            
            // Calculate icon heights based on button width to maintain aspect ratio
            int iconTradeHeight = (int)(iconTradeTexture.Height * ((float)buttonWidth / iconTradeTexture.Width));
            int iconShipHeight = (int)(iconShipTexture.Height * ((float)buttonWidth / iconShipTexture.Width));
            int iconFuelHeight = (int)(iconFuelTexture.Height * ((float)buttonWidth / iconFuelTexture.Width));
            
            // Find the maximum icon height to ensure consistent spacing
            int maxIconHeight = Math.Max(iconTradeHeight, Math.Max(iconShipHeight, iconFuelHeight));
            
            // Position buttons horizontally at the bottom of the screen, accounting for icon space
            int startX = (screenWidth - totalButtonWidth) / 2; // Center horizontally
            int buttonY = screenHeight - buttonHeight - 50; // Position at bottom with padding
            int iconY = buttonY - maxIconHeight - iconPadding; // Position icons above buttons

            // Initialize the three buttons horizontally
            visitTraderButton = new Button(new Rectangle(startX, buttonY, buttonWidth, buttonHeight), "Visit Trader", font, buttonTexture);
            returnToShipButton = new Button(new Rectangle(startX + buttonWidth + buttonSpacing, buttonY, buttonWidth, buttonHeight), "Return to Ship", font, buttonTexture);
            refuelShipButton = new Button(new Rectangle(startX + 2 * (buttonWidth + buttonSpacing), buttonY, buttonWidth, buttonHeight), "Refuel Ship", font, buttonTexture);
        }

        public void Update(GameTime gameTime)
        {
            visitTraderButton.Update(gameTime);
            returnToShipButton.Update(gameTime);
            refuelShipButton.Update(gameTime);

            if (visitTraderButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                GameManager.Instance.SetGameState(GameState.TradeScreen);
            }
            else if (returnToShipButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                GameManager.Instance.SetGameState(GameState.TravelScreen);
            }
            else if (refuelShipButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                // TODO: Implement refuel screen in the future
                // For now, do nothing
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the background texture
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);

            // Draw the info panel
            infoPanel.Draw(spriteBatch);
            
            // Calculate icon positions and dimensions (same as in LoadContent)
            int buttonWidth = 200;
            int buttonSpacing = 30;
            int iconPadding = 10;
            int totalButtonWidth = (3 * buttonWidth) + (2 * buttonSpacing);
            int startX = (1536 - totalButtonWidth) / 2; // Use actual screen width
            int buttonY = 1024 - 60 - 50; // Use actual screen height and button height
            
            // Calculate icon heights based on button width to maintain aspect ratio
            int iconTradeHeight = (int)(iconTradeTexture.Height * ((float)buttonWidth / iconTradeTexture.Width));
            int iconShipHeight = (int)(iconShipTexture.Height * ((float)buttonWidth / iconShipTexture.Width));
            int iconFuelHeight = (int)(iconFuelTexture.Height * ((float)buttonWidth / iconFuelTexture.Width));
            
            // Find the maximum icon height for consistent positioning
            int maxIconHeight = Math.Max(iconTradeHeight, Math.Max(iconShipHeight, iconFuelHeight));
            int iconY = buttonY - maxIconHeight - iconPadding;
            
            // Draw the icons above each button
            spriteBatch.Draw(iconTradeTexture, new Rectangle(startX, iconY, buttonWidth, iconTradeHeight), Color.White);
            spriteBatch.Draw(iconShipTexture, new Rectangle(startX + buttonWidth + buttonSpacing, iconY, buttonWidth, iconShipHeight), Color.White);
            spriteBatch.Draw(iconFuelTexture, new Rectangle(startX + 2 * (buttonWidth + buttonSpacing), iconY, buttonWidth, iconFuelHeight), Color.White);
            
            // Draw the three buttons
            visitTraderButton.Draw(spriteBatch);
            returnToShipButton.Draw(spriteBatch);
            refuelShipButton.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
