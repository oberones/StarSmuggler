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
        private SpriteFont font;
        private Texture2D buttonTexture;

        private List<Port> availableDestinations;
        private List<Button> travelButtons;
        private Texture2D backgroundTexture;

        private GraphicsDevice graphicsDevice; // Store this at class level

        private void GenerateTravelButtons(GraphicsDevice graphics, ContentManager content)
        {
            Port currentPort = GameManager.Instance.CurrentPort;
            availableDestinations = PortsDatabase.AllPorts
                .Where(p => p != currentPort)
                .ToList();

            travelButtons = new List<Button>();

            int screenWidth = graphics.Viewport.Width;
            int buttonWidth = 300;
            int startY = 100;
            int spacingY = 70;

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


        public void Refresh(ContentManager content)
        {
            // Reload whatever content depends on game state
            GenerateTravelButtons(graphicsDevice, content);
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice; // Cache for refresh use
            font = content.Load<SpriteFont>("Fonts/Default");
            buttonTexture = content.Load<Texture2D>("UI/button");
            backgroundTexture = content.Load<Texture2D>("UI/cockpit");

            Game1.AudioManager.LoadSfx("click");

            GenerateTravelButtons(graphicsDevice, content);
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
                    int cost = GameManager.Instance.GetTravelCost(GameManager.Instance.CurrentPort, destination);
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

        // private int GetTravelCost(Port from, Port to)
        // {
        //     // Basic cost system based on zone distance
        //     int zoneDifference = System.Math.Abs((int)from.Zone - (int)to.Zone);
        //     return 50 + (zoneDifference * 100); // e.g., Inner → Outer = +100, Inner → Fringe = +200
        // }
    }
}
