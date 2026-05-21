using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Tests.MenuLayouts;

public static class MenuLayoutTestData
{
    public static MenuLayoutDocument CreateValidDocument()
    {
        return new MenuLayoutDocument
        {
            Version = MenuLayoutDocument.CurrentVersion,
            CanvasWidth = 1536,
            CanvasHeight = 1024,
            BackgroundAsset = "UI/MainMenu",
            Elements =
            [
                new TextElement
                {
                    Id = "title",
                    X = 420,
                    Y = 120,
                    Width = 700,
                    Height = 80,
                    Text = "STAR SMUGGLER",
                    FontKey = "Fonts/TerminalBold",
                    FontScale = 1.5,
                    Color = "#FFFFFFFF",
                    HorizontalAlignment = nameof(HorizontalTextAlignment.Center)
                },
                Button("new-game", "New Game", nameof(MenuButtonAction.NewGame), 700, 450),
                Button("load-game", "Load Game", nameof(MenuButtonAction.LoadGame), 700, 520),
                Button("save-game", "Save Game", nameof(MenuButtonAction.SaveGame), 700, 590),
                Button("quit", "Quit", nameof(MenuButtonAction.Quit), 700, 660)
            ]
        };
    }

    public static ButtonMaskElement Button(string id, string label, string action, int x, int y)
    {
        return new ButtonMaskElement
        {
            Id = id,
            X = x,
            Y = y,
            Width = 200,
            Height = 50,
            Action = action,
            Label = label,
            Enabled = true
        };
    }
}
