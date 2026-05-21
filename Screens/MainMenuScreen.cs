using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StarSmuggler.MenuLayouts;
using StarSmuggler.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StarSmuggler.Screens
{
    public class MainMenuScreen : IScreen
    {
        private SpriteFont font;
        private Texture2D buttonTexture;
        private Texture2D backgroundTexture;
        private Texture2D logoTexture;
        private List<Button> fallbackButtons;
        private GraphicsDevice graphicsDevice;

        private const double NewGameActionDelaySeconds = 0.3;

        private readonly FallbackMenuItem[] fallbackMenuItems =
        [
            new FallbackMenuItem("New Game", MenuButtonAction.NewGame),
            new FallbackMenuItem("Load Game", MenuButtonAction.LoadGame),
            new FallbackMenuItem("Save Game", MenuButtonAction.SaveGame),
            new FallbackMenuItem("Quit", MenuButtonAction.Quit)
        ];

        private readonly List<RuntimeTextElement> layoutTextElements = new();
        private readonly List<RuntimeButtonMask> layoutButtonMasks = new();
        private readonly Dictionary<string, SpriteFont> fontCache = new(StringComparer.OrdinalIgnoreCase);
        private MenuLayoutDocument layoutDocument;
        private Texture2D layoutBackgroundTexture;
        private bool isUsingLayout;
        private int cachedViewportWidth;
        private int cachedViewportHeight;
        private MouseState previousMouse;
        private Song currentSong;
        private MenuButtonAction? pendingAction;
        private double pendingActionSecondsRemaining;

        public void Refresh(ContentManager content)
        {
            if (graphicsDevice != null)
            {
                TryLoadRuntimeLayout(content);
            }
        }

        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            currentSong = content.Load<Song>("Music/singularity");
            Game1.AudioManager.PlaySong("singularity");
            Game1.AudioManager.LoadSfx("click");
            font = content.Load<SpriteFont>("Fonts/TerminalBold");
            fontCache["Fonts/TerminalBold"] = font;
            buttonTexture = content.Load<Texture2D>("UI/button");
            backgroundTexture = content.Load<Texture2D>("UI/MainMenu");
            logoTexture = content.Load<Texture2D>("UI/logo1");
            graphicsDevice = graphics;
            CreateFallbackButtons();
            TryLoadRuntimeLayout(content);
        }

        public void Update(GameTime gameTime)
        {
            if (UpdatePendingAction(gameTime))
            {
                return;
            }

            if (isUsingLayout)
            {
                UpdateLayoutButtons();
                return;
            }

            for (int i = 0; i < fallbackButtons.Count; i++)
            {
                fallbackButtons[i].Update(gameTime);

                if (fallbackButtons[i].WasClicked)
                {
                    HandleFallbackClick(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (isUsingLayout)
            {
                EnsureLayoutCache();
                spriteBatch.Draw(layoutBackgroundTexture, GetViewportRectangle(), Color.White);
                foreach (var textElement in layoutTextElements)
                {
                    DrawLayoutText(spriteBatch, textElement);
                }
            }
            else
            {
                DrawFallback(spriteBatch);
            }

            spriteBatch.End();
        }

        private void TryLoadRuntimeLayout(ContentManager content)
        {
            // Layout loading is lifecycle work, not frame work. Any failure leaves the
            // original hardcoded menu active so a bad JSON file cannot block the title screen.
            isUsingLayout = false;
            layoutDocument = null;
            layoutTextElements.Clear();
            layoutButtonMasks.Clear();
            cachedViewportWidth = 0;
            cachedViewportHeight = 0;

            string layoutPath = Path.Combine(AppContext.BaseDirectory, "Content", "UI", "MenuLayouts", "main-menu.json");
            var loadResult = MenuLayoutLoader.TryLoad(layoutPath);
            if (!loadResult.Loaded || loadResult.Document == null)
            {
                Console.WriteLine($"Main menu layout fallback: {loadResult.WarningMessage}");
                return;
            }

            try
            {
                layoutBackgroundTexture = content.Load<Texture2D>(loadResult.Document.BackgroundAsset);
            }
            catch (Exception ex) when (ex is ContentLoadException or FileNotFoundException)
            {
                Console.WriteLine($"Main menu layout fallback: background asset '{loadResult.Document.BackgroundAsset}' could not be loaded: {ex.Message}");
                return;
            }

            layoutDocument = loadResult.Document;
            isUsingLayout = true;
            PreloadLayoutFonts(content);
            EnsureLayoutCache();
        }

        private void PreloadLayoutFonts(ContentManager content)
        {
            foreach (var textElement in layoutDocument.Elements.OfType<TextElement>())
            {
                if (fontCache.ContainsKey(textElement.FontKey))
                {
                    continue;
                }

                try
                {
                    fontCache[textElement.FontKey] = content.Load<SpriteFont>(textElement.FontKey);
                }
                catch (Exception ex) when (ex is ContentLoadException or FileNotFoundException)
                {
                    Console.WriteLine($"Main menu layout warning: font '{textElement.FontKey}' could not be loaded; using Fonts/TerminalBold. {ex.Message}");
                    fontCache[textElement.FontKey] = font;
                }
            }
        }

        private void EnsureLayoutCache()
        {
            // Source rectangles are authored on the layout canvas; cache their scaled
            // viewport rectangles until the window size changes to avoid per-frame churn.
            int viewportWidth = graphicsDevice.Viewport.Width;
            int viewportHeight = graphicsDevice.Viewport.Height;
            if (cachedViewportWidth == viewportWidth && cachedViewportHeight == viewportHeight)
            {
                return;
            }

            layoutTextElements.Clear();
            layoutButtonMasks.Clear();

            foreach (var element in layoutDocument.Elements)
            {
                var scaled = CoordinateScaler.ScaleRect(
                    element.Bounds,
                    layoutDocument.CanvasWidth,
                    layoutDocument.CanvasHeight,
                    viewportWidth,
                    viewportHeight);

                var rectangle = new Rectangle(scaled.X, scaled.Y, scaled.Width, scaled.Height);
                if (element is TextElement textElement)
                {
                    var spriteFont = fontCache.TryGetValue(textElement.FontKey, out var cachedFont)
                        ? cachedFont
                        : font;
                    var fontScale = CoordinateScaler.ScaleFontScale(
                        textElement.FontScale,
                        layoutDocument.CanvasHeight,
                        viewportHeight);
                    layoutTextElements.Add(new RuntimeTextElement(textElement, spriteFont, rectangle, (float)fontScale));
                }
                else if (element is ButtonMaskElement buttonMaskElement &&
                    buttonMaskElement.Enabled &&
                    Enum.TryParse<MenuButtonAction>(buttonMaskElement.Action, out var action))
                {
                    layoutButtonMasks.Add(new RuntimeButtonMask(action, rectangle));
                }
            }

            cachedViewportWidth = viewportWidth;
            cachedViewportHeight = viewportHeight;
        }

        private void UpdateLayoutButtons()
        {
            EnsureLayoutCache();
            var currentMouse = Mouse.GetState();
            var mousePosition = currentMouse.Position;

            if (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released)
            {
                foreach (var mask in layoutButtonMasks)
                {
                    if (mask.Bounds.Contains(mousePosition))
                    {
                        HandleAction(mask.Action);
                        break;
                    }
                }
            }

            previousMouse = currentMouse;
        }

        private void DrawLayoutText(SpriteBatch spriteBatch, RuntimeTextElement runtimeText)
        {
            var element = runtimeText.Element;
            float scale = runtimeText.FontScale;
            var textSize = runtimeText.Font.MeasureString(element.Text) * scale;
            float x = element.HorizontalAlignment switch
            {
                nameof(HorizontalTextAlignment.Center) => runtimeText.Bounds.X + (runtimeText.Bounds.Width - textSize.X) / 2,
                nameof(HorizontalTextAlignment.Right) => runtimeText.Bounds.Right - textSize.X,
                _ => runtimeText.Bounds.X
            };
            float y = runtimeText.Bounds.Y + (runtimeText.Bounds.Height - textSize.Y) / 2;

            spriteBatch.DrawString(
                runtimeText.Font,
                element.Text,
                new Vector2(x, y),
                ParseColor(element.Color),
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0);
        }

        private void DrawFallback(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, GetViewportRectangle(), Color.White);
            spriteBatch.Draw(logoTexture, new Rectangle(434, 25, 1463 / 2, 612 / 2), Color.White);

            foreach (var btn in fallbackButtons)
                btn.Draw(spriteBatch);
        }

        private void CreateFallbackButtons()
        {
            fallbackButtons = new List<Button>();
            int startY = 450;
            int spacing = 70;

            for (int i = 0; i < fallbackMenuItems.Length; i++)
            {
                var rect = new Rectangle(700, startY + i * spacing, 200, 50);
                fallbackButtons.Add(new Button(rect, fallbackMenuItems[i].Label, font, buttonTexture));
            }
        }

        private void HandleFallbackClick(int index)
        {
            if (index >= 0 && index < fallbackMenuItems.Length)
            {
                HandleAction(fallbackMenuItems[index].Action);
            }
        }

        private bool UpdatePendingAction(GameTime gameTime)
        {
            if (pendingAction is null)
            {
                return false;
            }

            pendingActionSecondsRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (pendingActionSecondsRemaining > 0)
            {
                return true;
            }

            var action = pendingAction.Value;
            pendingAction = null;
            pendingActionSecondsRemaining = 0;
            ExecuteAction(action);
            return true;
        }

        private void HandleAction(MenuButtonAction action)
        {
            // JSON actions intentionally dispatch through the same behavior as the
            // hardcoded fallback buttons, including the existing stranded-save guard.
            if (pendingAction is not null)
            {
                return;
            }

            Game1.AudioManager.PlaySfx("click");

            if (action == MenuButtonAction.NewGame)
            {
                // Preserve the click-audio beat without freezing update/draw on the main thread.
                pendingAction = action;
                pendingActionSecondsRemaining = NewGameActionDelaySeconds;
                return;
            }

            ExecuteAction(action);
        }

        private static void ExecuteAction(MenuButtonAction action)
        {
            var gameManager = GameManager.Instance;
            switch (action)
            {
                case MenuButtonAction.NewGame:
                    gameManager.StartNewGame();
                    break;
                case MenuButtonAction.LoadGame:
                    gameManager.LoadGame();
                    break;
                case MenuButtonAction.SaveGame:
                    if (gameManager.HasActiveGame && !gameManager.CheckForGameOver())
                        SaveLoadManager.SaveGame(gameManager.Player, GameState.PortOverview);
                    else if (gameManager.HasActiveGame)
                        Console.WriteLine("Cannot save a stranded game from the main menu.");
                    else
                        Console.WriteLine("No active game to save.");
                    break;
                case MenuButtonAction.Quit:
                    Game1.ExitGame();
                    break;
            }
        }

        private Rectangle GetViewportRectangle()
        {
            return new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }

        private static Color ParseColor(string hex)
        {
            string value = hex.TrimStart('#');
            if (value.Length == 6)
            {
                byte r = Convert.ToByte(value[..2], 16);
                byte g = Convert.ToByte(value.Substring(2, 2), 16);
                byte b = Convert.ToByte(value.Substring(4, 2), 16);
                return new Color(r, g, b);
            }

            if (value.Length == 8)
            {
                byte a = Convert.ToByte(value[..2], 16);
                byte r = Convert.ToByte(value.Substring(2, 2), 16);
                byte g = Convert.ToByte(value.Substring(4, 2), 16);
                byte b = Convert.ToByte(value.Substring(6, 2), 16);
                return new Color(r, g, b, a);
            }

            return Color.White;
        }

        private sealed record FallbackMenuItem(string Label, MenuButtonAction Action);

        private sealed record RuntimeTextElement(TextElement Element, SpriteFont Font, Rectangle Bounds, float FontScale);

        private sealed record RuntimeButtonMask(MenuButtonAction Action, Rectangle Bounds);
    }
}
