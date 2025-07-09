using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using StarSmuggler.UI;
using System;


namespace StarSmuggler.Screens
{
    public class TradeScreen : IScreen
    {
        private BackButton backButton;
        private SpriteFont buttonFont;
        private SpriteFont textFont;
        private Texture2D buttonTexture;
        private Texture2D terminalButtonTexture;

        private List<Item> items;
        private List<Button> buyButtons;
        private List<Button> sellButtons;

        private Button doneButton;
        private Texture2D terminalTexture;
        private Terminal terminalWindow;
        private List<NumericInput> numericInputs;
        private Port currentPort;
        private Texture2D backgroundTexture;
        private GraphicsDevice graphicsDevice;


        private int spacingY = 80;
        private int baseY = 150;


        public void Refresh(ContentManager content)
        {
            currentPort = GameManager.Instance.CurrentPort;
            items = GameManager.Instance.CurrentPort.AvailableItems;
            // Load the trade-specific background image for the current port
            backgroundTexture = content.Load<Texture2D>($"Trade/{currentPort.Id}Trade");
        }

        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            Game1.AudioManager.LoadSfx("click");
            buttonFont = content.Load<SpriteFont>("Fonts/Terminal");
            textFont = content.Load<SpriteFont>("Fonts/Terminal18");
            buttonTexture = content.Load<Texture2D>("UI/button");
            terminalButtonTexture = content.Load<Texture2D>("UI/terminalButtonNew");
            terminalTexture = content.Load<Texture2D>("Trade/tradeTerminal");
            this.graphicsDevice = graphics;

            currentPort = GameManager.Instance.CurrentPort;
            items = currentPort.AvailableItems;
            
            // Load the trade-specific background image for the current port
            backgroundTexture = content.Load<Texture2D>($"Trade/{currentPort.Id}Trade");
            
            buyButtons = new List<Button>();
            sellButtons = new List<Button>();
            
            // Calculate the center position for the Terminal
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            int terminalWidth = 1200; // Width of the new trade terminal
            int terminalHeight = 800; // Height of the new trade terminal

            int terminalX = (screenWidth - terminalWidth) / 2;
            int terminalY = (screenHeight - terminalHeight) / 2;

            terminalWindow = new Terminal(new Rectangle(terminalX, terminalY, terminalWidth, terminalHeight), texture: terminalTexture);

            numericInputs = new List<NumericInput>();

            for (int i = 0; i < items.Count; i++)
            {
                int y = baseY + i * spacingY;

                buyButtons.Add(new Button(new Rectangle(0, y, 80, 40), "Buy", buttonFont, buttonTexture));
                sellButtons.Add(new Button(new Rectangle(0, y, 80, 40), "Sell", buttonFont, buttonTexture));

                var inputRect = new Rectangle(500, y-60, 80, 40);
                numericInputs.Add(new NumericInput(inputRect, buttonFont, buttonTexture));
            }
            // Position navigation buttons at the far right of the terminal
            int backBtnX = terminalX + terminalWidth - 220; // Move closer to right edge
            int backBtnY = terminalY + terminalHeight - 100; // Move up slightly
            backButton = new BackButton(buttonFont, terminalButtonTexture, backBtnX, backBtnY, 90, 45);
            doneButton = new Button(new Rectangle(backBtnX + 100, backBtnY, 90, 45), "Done", buttonFont, terminalButtonTexture, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            backButton.Update(gameTime);
            for (int i = 0; i < items.Count; i++)
            {
                buyButtons[i].Update(gameTime);
                sellButtons[i].Update(gameTime);
                doneButton.Update(gameTime);

                var item = items[i];
                numericInputs[i].Update(gameTime);
                int quantity = numericInputs[i].Value;
                var player = GameManager.Instance.Player;

                int totalBuyCost = currentPort.Prices[item.Id] * quantity;
                int ownedQty = player.CargoHold.ContainsKey(item) ? player.CargoHold[item] : 0;

                // Handle Buy
                if (buyButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    if (player.Credits >= totalBuyCost && player.CanAddCargo(quantity))
                    {
                        player.Credits -= totalBuyCost;
                        if (!player.CargoHold.ContainsKey(item))
                            player.CargoHold[item] = 0;
                        player.CargoHold[item] += quantity;
                    }
                }

                // Handle Sell
                if (sellButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    if (ownedQty >= quantity)
                    {
                        player.Credits += currentPort.Prices[item.Id] * quantity;
                        player.CargoHold[item] -= quantity;
                        if (player.CargoHold[item] <= 0)
                            player.CargoHold.Remove(item);
                    }
                }

                // Handle done trading
                if (doneButton.WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    GameManager.Instance.CheckForGameOver();
                    SaveLoadManager.SaveGame(GameManager.Instance.Player);
                    GameManager.Instance.SetGameState(GameState.TravelScreen);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the background texture first
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);

            // Draw the terminal window
            terminalWindow.Draw(spriteBatch);

            // Offset all items to be relative to the terminal window's bounds + padding
            int terminalX = terminalWindow.Bounds.X + 80; // Padding from the left edge
            int terminalY = terminalWindow.Bounds.Y + 80; // Padding from the top edge

            // Draw the player's credits inside the terminal header area
            spriteBatch.DrawString(
                buttonFont, 
                $"Credits: {GameManager.Instance.Player.Credits}", 
                new Vector2(terminalX + 20, terminalY + 10),
                Color.Cyan
            );

            // Draw port name in the terminal
            spriteBatch.DrawString(
                buttonFont, 
                $"Trading at: {currentPort.Name}", 
                new Vector2(terminalX + 300, terminalY + 10),
                Color.Orange
            );

            Console.WriteLine($"TS Drawing inventory prices for : {currentPort.Name}");
            // Draw items, buttons, and numeric inputs inside the terminal
            for (int i = 0; i < items.Count; i++)
            {
                int y = terminalY + 80 + i * spacingY; // Start items lower in terminal

                var item = items[i];
                int ownedQty = GameManager.Instance.Player.CargoHold.ContainsKey(item) ? GameManager.Instance.Player.CargoHold[item] : 0;
                int qty = numericInputs[i].Value;

                // Draw the item's information in a more organized layout
                var price = currentPort.Prices[item.Id];
                string itemName = $"{item.Name}";
                string priceText = $"${price}";
                string ownedText = $"Owned: {ownedQty}";
                
                var itemX = terminalX + 30; // Item name position
                
                // Draw item name
                spriteBatch.DrawString(textFont, itemName, new Vector2(itemX, y), Color.White);
                
                // Draw item description below the name with a smaller font and muted color
                spriteBatch.DrawString(buttonFont, item.Description, new Vector2(itemX, y + 25), Color.LightGray);
                
                // Draw price
                spriteBatch.DrawString(textFont, priceText, new Vector2(itemX + 550, y), GetPriceColor(item, price));
                
                // Add visual indicator for event-affected prices
                int expectedPrice = GetExpectedPrice(item, currentPort);
                Console.WriteLine($"TS Expected price for {item.Name} at {currentPort.Name}: {expectedPrice}");
                float priceRatio = (float)price / expectedPrice;
                if (Math.Abs(priceRatio - 2.0f) < 0.3f) // Doubled by Merchant Strike
                {
                    spriteBatch.DrawString(buttonFont, "++", new Vector2(itemX + 522, y + 3), Color.Red);
                }
                else if (Math.Abs(priceRatio - 0.5f) < 0.2f) // Halved by Market Glut
                {
                    spriteBatch.DrawString(buttonFont, "--", new Vector2(itemX + 522, y + 3), Color.Cyan);
                }
                
                // Draw owned quantity
                spriteBatch.DrawString(textFont, ownedText, new Vector2(itemX + 650, y), Color.LightBlue);

                // Position buttons far to the right side of the terminal
                buyButtons[i].Bounds = new Rectangle(itemX + 800, y - 5, 80, 35); // Move Buy button to far right
                buyButtons[i].Draw(spriteBatch);

                sellButtons[i].Bounds = new Rectangle(itemX + 890, y - 5, 80, 35); // Move Sell button to far right
                sellButtons[i].Draw(spriteBatch);
            }
            // Draw the back button
            backButton.Draw(spriteBatch);

            // Draw the done button
            doneButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        // Helper method to calculate the expected "normal" price for an item at the current port
        // This is used to detect if an event has modified the price
        private int GetExpectedPrice(Item item, Port port)
        {
            // Use the same logic as UpdatePrices in GameManager but without variance
            float markup = GameManager.Instance.GetItemMarkup(item, port);
            float multiplier = 1f + markup; // No variance, just the base markup
            return Math.Max(1, (int)(item.BasePrice * multiplier));
        }

        // Helper method to determine the price color based on event modifications
        private Color GetPriceColor(Item item, int currentPrice)
        {
            int expectedPrice = GetExpectedPrice(item, currentPort);
            
            // Calculate the ratio of current price to expected price
            float priceRatio = (float)currentPrice / expectedPrice;
            
            // Check for specific event-based price modifications
            if (Math.Abs(priceRatio - 2.0f) < 0.3f) // Price roughly doubled (Merchant Strike)
                return Color.Red; // Very expensive due to strike
            else if (Math.Abs(priceRatio - 0.5f) < 0.2f) // Price roughly halved (Market Glut)
                return Color.Cyan; // Great bargain due to market glut
            else if (priceRatio < 0.8f) // Price reduced by 20% or more
                return Color.White; // Good deal
            else if (priceRatio > 1.3f) // Price increased by 30% or more  
                return Color.Orange; // More expensive than usual
            else
                return Color.LightGreen; // Normal price color
        }
    }
}
