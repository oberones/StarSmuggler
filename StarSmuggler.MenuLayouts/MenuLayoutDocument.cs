namespace StarSmuggler.MenuLayouts;

/// <summary>
/// Versioned JSON document exported by the menu layout editor and consumed by the game runtime.
/// Version 1 stores source-canvas pixel coordinates that are scaled to the active viewport at runtime.
/// </summary>
public sealed class MenuLayoutDocument
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;

    public int CanvasWidth { get; set; } = 1536;

    public int CanvasHeight { get; set; } = 1024;

    public string BackgroundAsset { get; set; } = "UI/MainMenu";

    public List<MenuLayoutElement> Elements { get; set; } = new();
}

public class MenuLayoutElement
{
    public string Type { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public MenuLayoutRect Bounds => new(X, Y, Width, Height);
}

public sealed class TextElement : MenuLayoutElement
{
    public TextElement()
    {
        Type = MenuLayoutElementTypes.Text;
    }

    public string Text { get; set; } = string.Empty;

    public string FontKey { get; set; } = "Fonts/TerminalBold";

    public double FontScale { get; set; } = 1.0;

    public string Color { get; set; } = "#FFFFFFFF";

    public string HorizontalAlignment { get; set; } = nameof(HorizontalTextAlignment.Left);
}

public sealed class ButtonMaskElement : MenuLayoutElement
{
    public ButtonMaskElement()
    {
        Type = MenuLayoutElementTypes.ButtonMask;
    }

    public string Action { get; set; } = nameof(MenuButtonAction.NewGame);

    public string? Label { get; set; }

    public bool Enabled { get; set; } = true;
}
