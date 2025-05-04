using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StarSmuggler.UI;
using System;
using System.Linq;

namespace StarSmuggler.Screens
{
    public class TravelScreen : IScreen
    {
        // Class variables are accessible to all methods in this class
        // They are not accessible to other classes unless they are public or protected
        private List<Port> availableDestinations;
        private Texture2D backgroundTexture;
        private BackButton backButton;
        private Texture2D buttonTexture;
        private SpriteFont font;
        private GraphicsDevice graphicsDevice;
        private List<Button> travelButtons;
        private Terminal terminalWindow;
        private Texture2D terminalTexture;



        // GenerateTravelButtons is a helper function to create the travel buttons with as little code as possible
        private void GenerateTravelButtons(GraphicsDevice graphics, ContentManager content)
        {
            // Get the current port and available destinations
            Port currentPort = GameManager.Instance.CurrentPort;
            availableDestinations = PortsDatabase.AllPorts
                .Where(p => p != currentPort)
                .ToList();

            travelButtons = new List<Button>();

            int screenWidth = graphics.Viewport.Width;
            int buttonWidth = 500;
            int startY = 100;
            int spacingY = 70;

            // Create a button for each available destination
            // The button will display the destination name and the travel cost
            for (int i = 0; i < availableDestinations.Count; i++)
            {
                Port dest = availableDestinations[i];
                int travelCost = GameManager.Instance.GetTravelCost(currentPort, dest);
                string label = $"{dest.Name} - ${travelCost}";

                int buttonX = (screenWidth - buttonWidth) / 2;
                Rectangle rect = new Rectangle(buttonX, startY + i * spacingY, buttonWidth, 50);

                var button = new Button(rect, label, font, buttonTexture);
                travelButtons.Add(button);
            }
        }

        // Refresh is called when the game state changes, such as when the player travels to a new port
        // It reloads the travel buttons and their prices based on the current game state
        public void Refresh(ContentManager content)
        {
            GenerateTravelButtons(graphicsDevice, content);
        }

        // LoadContent is called when the screen is first loaded
        // It loads the necessary content such as textures and fonts
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            // Load the click sound effect for use later
            Game1.AudioManager.LoadSfx("click");

            // Cache for refresh use
            this.graphicsDevice = graphicsDevice;

            // Load the background texture, font, and button texture 
            font = content.Load<SpriteFont>("Fonts/Terminal");
            buttonTexture = content.Load<Texture2D>("UI/button");
            backgroundTexture = content.Load<Texture2D>("UI/cockpit");
            terminalTexture = content.Load<Texture2D>("UI/terminalEmpty"); 
            // Calculate the center position for the Terminal
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            int terminalWidth = 900; // Width of the Terminal
            int terminalHeight = 904; // Height of the Terminal

            int terminalX = (screenWidth - terminalWidth) / 2;
            int terminalY = (screenHeight - terminalHeight) / 2;

            terminalWindow = new Terminal(new Rectangle(terminalX, terminalY, terminalWidth, terminalHeight), texture: terminalTexture);
            // Define the back button
            backButton = new BackButton(font, buttonTexture, 700, 650, 200, 50);
            
            // Generate the travel buttons based on the current game state
            GenerateTravelButtons(graphicsDevice, content);
        }

        // Update is called every frame to update the state of the screen
        // It checks for mouse/button clicks and updates the game state accordingly
        public void Update(GameTime gameTime)
        {
            backButton.Update(gameTime);
            for (int i = 0; i < travelButtons.Count; i++)
            {
                travelButtons[i].Update(gameTime);

                if (travelButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    Port destination = availableDestinations[i];
                    int cost = GameManager.Instance.GetTravelCost(GameManager.Instance.CurrentPort, destination);
                    Console.WriteLine($"Travelling to destination port: {destination.Name}, Travel Cost: {cost}");
                    GameManager.Instance.TravelToPort(destination, cost);
                }
            }
        }

        // Draw is called every frame to render the screen
        // It draws the background, buttons, and any other UI elements
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1600, 900), Color.White);
            terminalWindow.Draw(spriteBatch);
            spriteBatch.DrawString(font, $"Select Destination (Credits: {GameManager.Instance.Player.Credits})", new Vector2(50, 20), Color.White);

            foreach (var button in travelButtons)
            {
                button.Draw(spriteBatch);
            }
            backButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
