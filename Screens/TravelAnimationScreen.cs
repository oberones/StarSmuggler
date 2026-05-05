using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarSmuggler.Screens
{
    /// <summary>
    /// Screen that displays an animated space travel sequence when traveling between ports.
    /// Duration scales with distance between ports to give a sense of travel time.
    /// </summary>
    public class TravelAnimationScreen : IScreen
    {
        private AnimatedTexture shipAnimation;
        private Texture2D backgroundTexture;
        private GraphicsDevice graphicsDevice;
        private SpriteFont font;
        
        // Travel state management
        private Port fromPort;
        private Port toPort;
        private int travelCost;
        private float travelDuration; // Duration in seconds
        private float elapsedTime;
        private bool travelComplete;
        
        // Ship movement
        private Vector2 shipPosition;
        private float shipSpeed; // pixels per second
        private int shipWidth;
        private int shipHeight;
        
        // UI elements
        private string statusText;
        private bool showSkipPrompt;
        private KeyboardState previousKeyboardState;

        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            this.graphicsDevice = graphics;
            
            // Load font for UI text
            font = content.Load<SpriteFont>("Fonts/Terminal");
            
            // Load the static background
            try
            {
                backgroundTexture = content.Load<Texture2D>("Screens/travelBackground");
            }
            catch (ContentLoadException)
            {
                // Fallback to cockpit background if travel background not found
                backgroundTexture = content.Load<Texture2D>("UI/cockpit");
            }
            
            // Initialize the animated ship sprite
            shipAnimation = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0.5f, true);
            try
            {
                // Try to load the ship animation
                shipAnimation.Load(content, "Screens/shipAnimation", 6, 8); // 8 FPS for smooth but not too fast animation
                
                // Calculate ship dimensions (assuming each frame is square-ish, adjust as needed)
                shipWidth = shipAnimation.Texture.Width / 6; // 6 frames horizontally
                shipHeight = shipAnimation.Texture.Height;
            }
            catch (ContentLoadException)
            {
                // Fallback: create a simple single-frame animation
                shipAnimation.Load(content, "UI/iconShip", 1, 1);
                shipWidth = 64; // Default ship size
                shipHeight = 64;
            }
            
            // Set up animation completion callback
            shipAnimation.OnAnimationComplete = OnTravelAnimationComplete;
        }

        public void Refresh(ContentManager content)
        {
            // Get travel information from GameManager
            var gameManager = GameManager.Instance;
            var player = gameManager.Player;
            
            // Store the travel details for this animation sequence
            fromPort = player.CurrentPort;
            
            // Calculate travel duration based on distance between ports
            // Base duration of 3 seconds, plus additional time based on zone distance
            float baseDuration = 3.0f;
            float distanceMultiplier = 1.0f;
            
            if (toPort != null && fromPort != null)
            {
                // Calculate zone distance for duration scaling
                int zoneDifference = System.Math.Abs((int)fromPort.Zone - (int)toPort.Zone);
                distanceMultiplier = 1.0f + (zoneDifference * 1.5f); // +1.5 seconds per zone difference
                
                statusText = $"Traveling from {fromPort.Name} to {toPort.Name}...";
            }
            else
            {
                // Fallback values
                distanceMultiplier = 2.0f;
                statusText = "Traveling through space...";
            }
            
            travelDuration = baseDuration * distanceMultiplier;
            elapsedTime = 0f;
            travelComplete = false;
            showSkipPrompt = false;
            
            // Set up ship movement
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;
            
            // Start ship off-screen to the left
            shipPosition = new Vector2(-shipWidth, screenHeight / 2 - shipHeight / 2);
            
            // Calculate speed to cross screen in the travel duration
            float totalDistance = screenWidth + shipWidth * 2; // Extra distance for off-screen start/end
            shipSpeed = totalDistance / travelDuration;
            
            // Reset and start the ship animation
            shipAnimation.Reset();
            shipAnimation.Play();
            
            System.Console.WriteLine($"Starting travel animation - Duration: {travelDuration:F1} seconds, Ship speed: {shipSpeed:F1} px/s");
        }

        public void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update the ship animation
            shipAnimation.UpdateFrame(deltaTime);
            
            // Update ship position
            shipPosition.X += shipSpeed * deltaTime;
            
            // Update travel timer
            elapsedTime += deltaTime;
            
            // Show skip prompt after 1 second
            if (elapsedTime > 1.0f && !showSkipPrompt)
            {
                showSkipPrompt = true;
            }
            
            // Check for travel completion (either time elapsed or ship moved off-screen)
            bool timeComplete = elapsedTime >= travelDuration;
            bool shipComplete = shipPosition.X > graphicsDevice.Viewport.Width + shipWidth;
            
            if ((timeComplete || shipComplete) && !travelComplete)
            {
                CompleteTravelSequence();
            }
            
            // Allow player to skip animation with Space or Enter
            if (showSkipPrompt && 
                ((currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space)) ||
                 (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))))
            {
                if (!travelComplete)
                {
                    CompleteTravelSequence();
                }
            }
            
            previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            
            // Draw the static background
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            
            // Draw the animated ship sprite
            shipAnimation.DrawFrame(spriteBatch, shipPosition);
            
            // Draw status text in the center bottom
            var screenCenter = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height * 0.85f);
            var textSize = font.MeasureString(statusText);
            var textPosition = screenCenter - textSize / 2;
            
            // Draw text with shadow for better visibility
            spriteBatch.DrawString(font, statusText, textPosition + Vector2.One, Color.Black);
            spriteBatch.DrawString(font, statusText, textPosition, Color.White);
            
            // Draw skip prompt if available
            if (showSkipPrompt && !travelComplete)
            {
                string skipText = "Press SPACE or ENTER to skip";
                var skipTextSize = font.MeasureString(skipText);
                var skipTextPosition = new Vector2(
                    (graphicsDevice.Viewport.Width - skipTextSize.X) / 2,
                    screenCenter.Y + 40
                );
                
                // Fade in the skip text
                float alpha = MathHelper.Clamp((elapsedTime - 1.0f) * 2.0f, 0f, 1f);
                Color skipColor = Color.LightGray * alpha;
                Color skipShadowColor = Color.Black * alpha;
                
                spriteBatch.DrawString(font, skipText, skipTextPosition + Vector2.One, skipShadowColor);
                spriteBatch.DrawString(font, skipText, skipTextPosition, skipColor);
            }
            
            spriteBatch.End();
        }

        /// <summary>
        /// Sets the destination port for this travel sequence.
        /// Should be called before transitioning to this screen.
        /// </summary>
        public void SetTravelDestination(Port destination, int cost)
        {
            toPort = destination;
            travelCost = cost;
        }

        private void OnTravelAnimationComplete()
        {
            // This callback is triggered when the animation completes (if it's non-looping)
            // For a looping animation, this won't be called
            if (!travelComplete)
            {
                CompleteTravelSequence();
            }
        }

        private void CompleteTravelSequence()
        {
            if (travelComplete) return;
            
            travelComplete = true;
            
            System.Console.WriteLine("Travel animation complete, processing travel logic...");
            
            // Execute the actual travel logic
            if (toPort != null)
            {
                var gameManager = GameManager.Instance;
                var player = gameManager.Player;
                
                // Deduct travel cost
                player.Credits -= travelCost;
                
                // Set new current port
                player.CurrentPort = toPort;
                
                // Load goods for the new port
                gameManager.LoadGoodsForCurrentPort();
                
                // Update jumps counter
                player.JumpsSinceLastUpdate++;
                
                // Update prices if needed
                gameManager.UpdatePrices(PortsDatabase.AllPorts, ItemsDatabase.AllItems);
                
                // Trigger random event
                gameManager.TriggerRandomEvent();
                
                // Save game
                SaveLoadManager.SaveGame(player);
                
                // Check for game over
                if (gameManager.CheckForGameOver())
                    return;
            }
            
            // Transition to port overview screen
            GameManager.Instance.SetGameState(GameState.PortOverview);
        }
    }
}
