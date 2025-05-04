using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StarSmuggler.UI;
using System;

namespace StarSmuggler.Screens
{
    public class TravelScreen : IScreen
    {
        private SpriteFont font;
        private Texture2D buttonTexture;

        private List<Port> availableDestinations;
        private List<Button> travelButtons;
        private Texture2D backgroundTexture;

        public void Refresh(ContentManager content)
        {
            // Reload whatever content depends on game state
            // Optional: re-roll event text or background
            // backgroundTexture = content.Load<Texture2D>($"Ports/{currentPort.Name.ToLower()}");
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/Default");
            buttonTexture = content.Load<Texture2D>("UI/button");
            Port currentPort = GameManager.Instance.CurrentPort;
            backgroundTexture = content.Load<Texture2D>("UI/cockpit");
            availableDestinations = PortsDatabase.AllPorts.FindAll(p => p != currentPort);
            travelButtons = new List<Button>();
            Game1.AudioManager.LoadSfx("click");
            int screenWidth = graphicsDevice.Viewport.Width; // Get the screen width
            int buttonWidth = 300; // Width of the button
            int startY = 100;
            int spacingY = 70;

            for (int i = 0; i < availableDestinations.Count; i++)
            {
                Port dest = availableDestinations[i];
                int travelCost = GetTravelCost(currentPort, dest);

                string label = $"{dest.Name} - ${travelCost}";
                // Calculate the X position to center the button
                int buttonX = (screenWidth - buttonWidth) / 2;
                Rectangle rect = new Rectangle(buttonX, startY + i * spacingY, buttonWidth, 50);

                var button = new Button(rect, label, font, buttonTexture);
                travelButtons.Add(button);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < travelButtons.Count; i++)
            {
                travelButtons[i].Update(gameTime);

                if (travelButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    Port destination = availableDestinations[i];
                    int cost = GetTravelCost(GameManager.Instance.CurrentPort, destination);
                    Console.WriteLine($"Travelling to destination port: {destination.Name}, Travel Cost: {cost}");
                    GameManager.Instance.TravelToPort(destination, cost);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1600, 900), Color.White);
            spriteBatch.DrawString(font, $"Select Destination (Credits: {GameManager.Instance.Player.Credits})", new Vector2(50, 20), Color.White);

            foreach (var button in travelButtons)
            {
                button.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private int GetTravelCost(Port from, Port to)
        {
            // Basic cost system based on zone distance
            int zoneDifference = System.Math.Abs((int)from.Zone - (int)to.Zone);
            return 50 + (zoneDifference * 100); // e.g., Inner → Outer = +100, Inner → Fringe = +200
        }
    }
}
