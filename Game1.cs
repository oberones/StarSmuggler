using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarSmuggler.Screens;
using StarSmuggler.UI;
using StarSmuggler.Audio;

namespace StarSmuggler;


public class Game1 : Game
{
    private KeyboardState previousKeyboardState;
    public static Game1 Instance { get; private set; }
    public static ScreenManager ScreenManagerRef { get; private set; }
    public static AudioManager AudioManager { get; private set; }
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // PortOverviewScreen portOverviewScreen;
    // TradeScreen tradeScreen;
    // TravelScreen travelScreen;
    ScreenManager screenManager;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Instance = this;
    }

    public static void ExitGame()
    {
        Instance.Exit();
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();
        GameManager.Instance.SetGameState(GameState.MainMenu);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Setup screen manager FIRST
        screenManager = new ScreenManager(GraphicsDevice, Content);
        ScreenManagerRef = screenManager;

        screenManager.Register(GameState.MainMenu, new MainMenuScreen());
        screenManager.Register(GameState.TradeScreen, new TradeScreen());
        screenManager.Register(GameState.TravelScreen, new TravelScreen());
        screenManager.Register(GameState.PortOverview, new PortOverviewScreen());

        AudioManager = new AudioManager();
        AudioManager.Initialize(Content);

        GameManager.Instance.SetGameState(GameState.MainMenu);
    }

    protected override void Update(GameTime gameTime)
    {
        AudioManager.Update(gameTime);
        var currentKeyboardState = Keyboard.GetState();

        // If ESC was just pressed
        if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
        {
            if (GameManager.Instance.CurrentState == GameState.MainMenu &&
                GameManager.Instance.HasPreviousState())
            {
                GameManager.Instance.ReturnToPreviousState();
            }
            else
            {
                GameManager.Instance.SetGameState(GameState.MainMenu);
            }
        }

        previousKeyboardState = currentKeyboardState;
        ScreenManagerRef.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        ScreenManagerRef.Draw(_spriteBatch);
        base.Draw(gameTime);
    }
}
