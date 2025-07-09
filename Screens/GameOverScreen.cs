using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
// using StarSmuggler.Screens;
using StarSmuggler.UI;
using System.Collections.Generic;

namespace StarSmuggler.Screens
{
    public class GameOverScreen : IScreen
    {
        private string summaryText = "";
        private Texture2D backgroundTexture;
        private GraphicsDevice graphicsDevice;
        private SpriteFont font;

        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/Default");
            backgroundTexture = content.Load<Texture2D>("Screens/gameover_broke");
            this.graphicsDevice = graphics;
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                GameManager.Instance.StartNewGame(); // Restart
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            var y = 420;
            foreach (var line in summaryText.Split('\n'))
            {
                spriteBatch.DrawString(font, line, new Vector2(430, y), Color.LightGray);
                y += 40;
            }

            spriteBatch.DrawString(font, "Game Over", new Vector2(550, 300), Color.Red);
            spriteBatch.DrawString(font, "Press Escape for the main menu", new Vector2(430, 360), Color.White);
            spriteBatch.End();
        }

        public void Refresh(ContentManager content) { 
            var player = GameManager.Instance.Player;
            var port = player.CurrentPort;

            int cargoValue = 0;
            foreach (var pair in player.CargoHold)
            {
                var matchingItem = port.AvailableItems.Find(g => g == pair.Key);
                if (matchingItem != null)
                {
                    cargoValue += pair.Value * matchingItem.BasePrice;
                }
            }

            int lowestTravelCost = int.MaxValue;
            foreach (var destination in PortsDatabase.AllPorts)
            {
                if (destination != port)
                {
                    int cost = GameManager.Instance.GetTravelCost(port, destination);
                    if (cost < lowestTravelCost)
                        lowestTravelCost = cost;
                }
            }

            summaryText = $"You are stranded on {port.Name}.\n" +
                        $"Credits: {player.Credits}\n" +
                        $"Sellable cargo value: {cargoValue}\n" +
                        $"Cheapest travel cost: {lowestTravelCost}\n" +
                        $"No way to earn enough to leave.";
        }
    }
}
