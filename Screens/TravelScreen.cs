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
        private Texture2D portPreviewTexture;
        private Terminal terminalWindow;
        private Texture2D terminalTexture;
        private Port selectedPort;
        private Port currentPort;
        private Button previousButton;
        private Button nextButton;
        private SpriteFont buttonFont;
        private Texture2D terminalButtonTexture;
        private Button travelButton;
        private ContentManager content;

        // Refresh is called when the game state changes, such as when the player travels to a new port
        // It reloads the travel buttons and their prices based on the current game state
        public void Refresh(ContentManager content)
        {
            // Select a random inner port for the preview to start with
            selectedPort = PortsDatabase.GetRandomInnerPort();
            currentPort = GameManager.Instance.CurrentPort;
            // Update the port texture based on the selected port
            portPreviewTexture = content.Load<Texture2D>(selectedPort.PreviewImagePath);
        }

        // LoadContent is called when the screen is first loaded
        // It loads the necessary content such as textures and fonts
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            // Load the click sound effect for use later
            Game1.AudioManager.LoadSfx("click");

            // Cache for refresh use
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            buttonFont = content.Load<SpriteFont>("Fonts/Terminal");
            // Load the background texture, font, and button texture 
            font = content.Load<SpriteFont>("Fonts/Terminal");
            buttonTexture = content.Load<Texture2D>("UI/button");
            backgroundTexture = content.Load<Texture2D>("UI/cockpit");
            terminalTexture = content.Load<Texture2D>("UI/terminalTransparent");
            terminalButtonTexture = content.Load<Texture2D>("UI/terminalButtonNew");
            portPreviewTexture = content.Load<Texture2D>("Ports/mercuryPreview"); // TODO: figure out how to load the correct port preview dynamically
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

            // // Generate the travel buttons based on the current game state
            // GenerateTravelButtons(graphicsDevice, content);
            int navBtnX = terminalWindow.Bounds.X + 620;
            int navBtnY = terminalWindow.Bounds.Y + 670;
            previousButton = new Button(new Rectangle(navBtnX, navBtnY, 100, 51), "Prev.", buttonFont, terminalButtonTexture, Color.White);
            nextButton = new Button(new Rectangle(navBtnX + 100, navBtnY, 100, 51), "Next", buttonFont, terminalButtonTexture, Color.White);
            // Add Travel button inside the terminal window, below the port name/cost
            int travelBtnWidth = 140;
            int travelBtnHeight = 51;
            int travelBtnX = terminalWindow.Bounds.X + 650;
            int travelBtnY = terminalWindow.Bounds.Y + 725;
            travelButton = new Button(new Rectangle(travelBtnX, travelBtnY, travelBtnWidth, travelBtnHeight), "Travel", buttonFont, terminalButtonTexture, Color.White);
        }

        // Update is called every frame to update the state of the screen
        // It checks for mouse/button clicks and updates the game state accordingly
        public void Update(GameTime gameTime)
        {
            backButton.Update(gameTime);
            previousButton.Update(gameTime);
            nextButton.Update(gameTime);
            travelButton.Update(gameTime);
            // Check if the previous or next button was clicked
            if (previousButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                // Select the previous port in the list
                int currentIndex = PortsDatabase.AllPorts.IndexOf(selectedPort);
                if (currentIndex > 0)
                {
                    selectedPort = PortsDatabase.AllPorts[currentIndex - 1];
                    portPreviewTexture = content.Load<Texture2D>(selectedPort.PreviewImagePath);
                }
            }
            else if (nextButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                // Select the next port in the list
                int currentIndex = PortsDatabase.AllPorts.IndexOf(selectedPort);
                if (currentIndex < PortsDatabase.AllPorts.Count - 1)
                {
                    selectedPort = PortsDatabase.AllPorts[currentIndex + 1];
                    portPreviewTexture = content.Load<Texture2D>(selectedPort.PreviewImagePath);
                }
            }
            else if (travelButton.WasClicked)
            {
                Game1.AudioManager.PlaySfx("click");
                int cost = GameManager.Instance.GetTravelCost(currentPort, selectedPort);
                GameManager.Instance.TravelToPort(selectedPort, cost);
                // Optionally, refresh the screen or update UI as needed
            }
        }

        // Draw is called every frame to render the screen
        // It draws the background, buttons, and any other UI elements
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Draw the background texture
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            var previewHeight = terminalWindow.Bounds.Height - 350;
            var ratio = (float)4 / 3; // Maintain a 4:3 aspect ratio for the preview
            var previewWidth = Convert.ToInt32(previewHeight * ratio);

            // Draw the port preview texture in the terminal window
            spriteBatch.Draw(portPreviewTexture, new Rectangle(terminalWindow.Bounds.X + 65, terminalWindow.Bounds.Y + 75, previewWidth, previewHeight), Color.White);

            // Draw the terminal window (defined above)
            terminalWindow.Draw(spriteBatch);

            // Draw the name and cost of the selected port
            spriteBatch.DrawString(font, $"{selectedPort.Name}: {GameManager.Instance.GetTravelCost(currentPort, selectedPort)} Credits", new Vector2(terminalWindow.Bounds.X + 140, terminalWindow.Bounds.Y + 717), Color.White);

            // Draw the next and previous buttons
            previousButton.Draw(spriteBatch);
            nextButton.Draw(spriteBatch);
            // Draw the Travel button
            travelButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
