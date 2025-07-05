using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StarSmuggler.Screens
{
    public class ScreenManager
    {
        private Dictionary<GameState, IScreen> screens = new();
        private IScreen activeScreen;
        private Dictionary<GameState, bool> isLoaded = new();
        private GraphicsDevice graphics;
        private ContentManager content;

        // Constructor to initialize the ScreenManager with GraphicsDevice and ContentManager
        public ScreenManager(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            graphics = graphicsDevice;
            content = contentManager;
        }

        // Method to register a screen for a specific game state
        public void Register(GameState state, IScreen screen)
        {
            screens[state] = screen;
            isLoaded[state] = false; // mark as not yet loaded
        }

        // Method to set the active screen based on the game state
        public void SetActive(GameState state)
        {
            // If the state is not registered, throw an exception
            if (screens.TryGetValue(state, out var screen))
            {
                // If the screen is not loaded, load its content
                if (!isLoaded[state])
                {
                    screen.LoadContent(graphics, content);
                    isLoaded[state] = true;
                }
                // If the screen is already active, do nothing
                activeScreen = screen;
                activeScreen.Refresh(content);
            }
        }

        // Method to check if a screen is loaded for a specific game state
        public void Update(GameTime gameTime)
        {
            activeScreen?.Update(gameTime);
        }

        // Method to draw the active screen
        public void Draw(SpriteBatch spriteBatch)
        {
            activeScreen?.Draw(spriteBatch);
        }
    }
}
