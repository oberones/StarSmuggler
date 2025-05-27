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
        // private CurrentPrices currentPrices;


        private int spacingY = 80;
        private int baseY = 150;


        public void Refresh(ContentManager content)
        {
            currentPort = GameManager.Instance.CurrentPort;
            items = GameManager.Instance.CurrentPort.AvailableItems;
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            Game1.AudioManager.LoadSfx("click");
            buttonFont = content.Load<SpriteFont>("Fonts/Terminal");
            textFont = content.Load<SpriteFont>("Fonts/Terminal16");
            buttonTexture = content.Load<Texture2D>("UI/button");
            terminalButtonTexture = content.Load<Texture2D>("UI/terminalButton");
            terminalTexture = content.Load<Texture2D>("UI/terminalEmptyNew"); 

            currentPort = GameManager.Instance.CurrentPort;
            items = currentPort.AvailableItems;
            buyButtons = new List<Button>();
            sellButtons = new List<Button>();
            
            // Calculate the center position for the Terminal
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            int terminalWidth = 900; // Width of the Terminal
            int terminalHeight = 904; // Height of the Terminal

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
            backButton = new BackButton(buttonFont, terminalButtonTexture, 963, 671, 100, 51);
            doneButton = new Button(new Rectangle(1078, 670, 100, 51), "Done", buttonFont, terminalButtonTexture, Color.White);
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

                int totalBuyCost = item.BasePrice * quantity;
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
                        player.Credits += item.BasePrice * quantity;
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

            // Draw the terminal window
            terminalWindow.Draw(spriteBatch);

            // Offset all items to be relative to the terminal window's bounds + padding
            int terminalX = terminalWindow.Bounds.X + 100; // Padding from the left edge
            int terminalY = terminalWindow.Bounds.Y + 80; // Padding from the top edge

            // Draw the player's credits inside the terminal
            spriteBatch.DrawString(
                buttonFont, 
                $"Credits: {GameManager.Instance.Player.Credits}", 
                new Vector2(terminalX + 150, terminalY + 645), // Offset relative to the terminal
                Color.White
            );

            Console.WriteLine($"TS Drawing inventory prices for : {currentPort.Name}");
            // Draw items, buttons, and numeric inputs inside the terminal
            for (int i = 0; i < items.Count; i++)
            {
                int y = terminalY + 25 + i * spacingY; // Offset Y relative to the terminal

                var item = items[i];
                int ownedQty = GameManager.Instance.Player.CargoHold.ContainsKey(item) ? GameManager.Instance.Player.CargoHold[item] : 0;
                int qty = numericInputs[i].Value;

                // Draw the item's information
                // var price = GameManager.Instance.CurrentPort.CurrentPrices[item];
                var price = currentPort.Prices[item.Id];
                string line = $"{item.Name} - ${price} | Owned: {ownedQty}";
                spriteBatch.DrawString(textFont, line, new Vector2(terminalX + 20, y), Color.LightGray);

                // Draw numeric input, buy button, and sell button relative to the terminal
                numericInputs[i].Bounds = new Rectangle(terminalX + 26, y+35, 80, 40); // Selected value
                numericInputs[i].Draw(spriteBatch);

                buyButtons[i].Bounds = new Rectangle(terminalX + 160, y+35, 80, 40); // Adjust position
                buyButtons[i].Draw(spriteBatch);

                sellButtons[i].Bounds = new Rectangle(terminalX + 260, y+35, 80, 40); // Adjust position
                sellButtons[i].Draw(spriteBatch);
            }
            // Draw the back button
            backButton.Draw(spriteBatch);

            // Draw the done button
            doneButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
