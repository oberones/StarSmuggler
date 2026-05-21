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
        private ContentManager contentManager;

        private readonly string[] labels = new[] { "New Game", "Load Game", "Save Game", "Quit" };
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
            contentManager = content;
            CreateFallbackButtons();
            TryLoadRuntimeLayout(content);
        }

        public void Update(GameTime gameTime)
        {
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
                    layoutTextElements.Add(new RuntimeTextElement(textElement, spriteFont, rectangle));
                }
                else if (element is ButtonMaskElement buttonMaskElement && buttonMaskElement.Enabled)
                {
                    layoutButtonMasks.Add(new RuntimeButtonMask(buttonMaskElement.Action, rectangle));
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
            float scale = (float)element.FontScale;
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

            for (int i = 0; i < labels.Length; i++)
            {
                var rect = new Rectangle(700, startY + i * spacing, 200, 50);
                fallbackButtons.Add(new Button(rect, labels[i], font, buttonTexture));
            }
        }

        private void HandleFallbackClick(int index)
        {
            HandleAction(labels[index].Replace(" ", string.Empty));
        }

        private void HandleAction(string action)
        {
            // JSON actions intentionally dispatch through the same behavior as the
            // hardcoded fallback buttons, including the existing stranded-save guard.
            Game1.AudioManager.PlaySfx("click");
            switch (action)
            {
                case nameof(MenuButtonAction.NewGame):
                    System.Threading.Thread.Sleep(300);
                    GameManager.Instance.StartNewGame();
                    break;
                case nameof(MenuButtonAction.LoadGame):
                    GameManager.Instance.LoadGame();
                    break;
                case nameof(MenuButtonAction.SaveGame):
                    var gameManager = GameManager.Instance;
                    if (gameManager.HasActiveGame && !gameManager.CheckForGameOver())
                        SaveLoadManager.SaveGame(gameManager.Player, GameState.PortOverview);
                    else if (gameManager.HasActiveGame)
                        Console.WriteLine("Cannot save a stranded game from the main menu.");
                    else
                        Console.WriteLine("No active game to save.");
                    break;
                case nameof(MenuButtonAction.Quit):
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

        private sealed record RuntimeTextElement(TextElement Element, SpriteFont Font, Rectangle Bounds);

        private sealed record RuntimeButtonMask(string Action, Rectangle Bounds);
    }
}
