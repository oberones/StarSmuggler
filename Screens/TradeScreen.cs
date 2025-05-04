using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using StarSmuggler.UI;


namespace StarSmuggler.Screens
{
    public class TradeScreen : IScreen
    {
        private SpriteFont font;
        private Texture2D buttonTexture;
        private Texture2D terminalButtonTexture;

        private List<Good> goods;
        private List<Button> buyButtons;
        private List<Button> sellButtons;

        private Button doneButton;
        private Texture2D terminalTexture;
        private Terminal terminalWindow;
        private List<NumericInput> numericInputs;

        private int spacingY = 80;
        private int baseY = 100;


        public void Refresh(ContentManager content)
        {
            // Reload whatever content depends on game state

            // Optional: re-roll event text or background
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/Default");
            buttonTexture = content.Load<Texture2D>("UI/button");
            terminalButtonTexture = content.Load<Texture2D>("UI/terminalButton");
            terminalTexture = content.Load<Texture2D>("UI/terminalEmpty"); 
            Game1.AudioManager.LoadSfx("click");
            var port = GameManager.Instance.CurrentPort;
            goods = port.AvailableGoods;

            buyButtons = new List<Button>();
            sellButtons = new List<Button>();
            int doneButtonY = baseY + goods.Count * spacingY + 40;
            
            // Calculate the center position for the Terminal
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            int terminalWidth = 900; // Width of the Terminal
            int terminalHeight = 904; // Height of the Terminal

            int terminalX = (screenWidth - terminalWidth) / 2;
            int terminalY = (screenHeight - terminalHeight) / 2;

            terminalWindow = new Terminal(new Rectangle(terminalX, terminalY, terminalWidth, terminalHeight), texture: terminalTexture);

            numericInputs = new List<NumericInput>();

            for (int i = 0; i < goods.Count; i++)
            {
                int y = baseY + i * spacingY;

                buyButtons.Add(new Button(new Rectangle(600, y, 80, 40), "Buy", font, buttonTexture));
                sellButtons.Add(new Button(new Rectangle(700, y, 80, 40), "Sell", font, buttonTexture));

                var inputRect = new Rectangle(500, y, 80, 40);
                numericInputs.Add(new NumericInput(inputRect, font, buttonTexture));
            }
            doneButton = new Button(new Rectangle(1078, 670, 100, 51), "Done", font, terminalButtonTexture, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < goods.Count; i++)
            {
                buyButtons[i].Update(gameTime);
                sellButtons[i].Update(gameTime);
                doneButton.Update(gameTime);

                var good = goods[i];
                numericInputs[i].Update(gameTime);
                int quantity = numericInputs[i].Value;
                var player = GameManager.Instance.Player;

                int totalBuyCost = good.BasePrice * quantity;
                int ownedQty = player.CargoHold.ContainsKey(good) ? player.CargoHold[good] : 0;

                // Handle Buy
                if (buyButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    if (player.Credits >= totalBuyCost && player.CanAddCargo(quantity))
                    {
                        player.Credits -= totalBuyCost;
                        if (!player.CargoHold.ContainsKey(good))
                            player.CargoHold[good] = 0;
                        player.CargoHold[good] += quantity;
                    }
                }

                // Handle Sell
                if (sellButtons[i].WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
                    if (ownedQty >= quantity)
                    {
                        player.Credits += good.BasePrice * quantity;
                        player.CargoHold[good] -= quantity;
                        if (player.CargoHold[good] <= 0)
                            player.CargoHold.Remove(good);
                    }
                }

                // Handle done trading
                if (doneButton.WasClicked)
                {
                    Game1.AudioManager.PlaySfx("click");
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
            int terminalY = terminalWindow.Bounds.Y + 100; // Padding from the top edge

                // Draw the player's credits inside the terminal
            spriteBatch.DrawString(
                font, 
                $"Credits: {GameManager.Instance.Player.Credits}", 
                new Vector2(terminalX + 20, terminalY + 20), // Offset relative to the terminal
                Color.White
            );

            // Draw goods, buttons, and numeric inputs inside the terminal
            for (int i = 0; i < goods.Count; i++)
            {
                int y = terminalY + 60 + i * spacingY; // Offset Y relative to the terminal

                var good = goods[i];
                int ownedQty = GameManager.Instance.Player.CargoHold.ContainsKey(good) ? GameManager.Instance.Player.CargoHold[good] : 0;
                int qty = numericInputs[i].Value;

                // Draw the good's information
                string line = $"{good.Name} - ${good.BasePrice} | Owned: {ownedQty} | Qty: {qty}";
                spriteBatch.DrawString(font, line, new Vector2(terminalX + 20, y), Color.LightGray);

                // Draw numeric input, buy button, and sell button relative to the terminal
                numericInputs[i].Bounds = new Rectangle(terminalX + 400, y, 80, 40); // Selected value
                numericInputs[i].Draw(spriteBatch);

                buyButtons[i].Bounds = new Rectangle(terminalX + 500, y, 80, 40); // Adjust position
                buyButtons[i].Draw(spriteBatch);

                sellButtons[i].Bounds = new Rectangle(terminalX + 600, y, 80, 40); // Adjust position
                sellButtons[i].Draw(spriteBatch);
            }
            // doneButton.Bounds = new Rectangle(terminalX + 20, terminalY + terminalWindow.Bounds.Height - 60, 120, 40); // Adjust position
            doneButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
