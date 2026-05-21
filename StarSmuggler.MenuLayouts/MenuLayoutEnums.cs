namespace StarSmuggler.MenuLayouts;

public static class MenuLayoutElementTypes
{
    public const string Text = "Text";
    public const string ButtonMask = "ButtonMask";
}

public enum MenuLayoutElementType
{
    Text,
    ButtonMask
}

public enum MenuButtonAction
{
    NewGame,
    LoadGame,
    SaveGame,
    Quit
}

public enum HorizontalTextAlignment
{
    Left,
    Center,
    Right
}
