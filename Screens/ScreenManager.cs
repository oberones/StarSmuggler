using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        public ScreenManager(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            graphics = graphicsDevice;
            content = contentManager;
        }

        public void Register(GameState state, IScreen screen)
        {
            screens[state] = screen;
            isLoaded[state] = false; // mark as not yet loaded
        }

        public void SetActive(GameState state)
        {
            if (screens.TryGetValue(state, out var screen))
            {
                if (!isLoaded[state])
                {
                    screen.LoadContent(graphics, content);
                    isLoaded[state] = true;
                }

                activeScreen = screen;
                activeScreen.Refresh(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            activeScreen?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            activeScreen?.Draw(spriteBatch);
        }
    }
}
