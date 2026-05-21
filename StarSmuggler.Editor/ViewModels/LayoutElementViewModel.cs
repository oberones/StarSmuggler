using Avalonia.Media;
using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.ViewModels;

public sealed class LayoutElementViewModel : ViewModelBase
{
    private static readonly IReadOnlyList<FontColorOption> AvailableFontColorOptions =
    [
        new FontColorOption("White", "#FFFFFFFF"),
        new FontColorOption("Black", "#FF000000"),
        new FontColorOption("Terminal Green", "#FF38F58B"),
        new FontColorOption("Amber", "#FFFFB84D"),
        new FontColorOption("Cyan", "#FF5BA4FF"),
        new FontColorOption("Warning Red", "#FFFF5A5A"),
        new FontColorOption("Muted Gray", "#FF8995A8")
    ];

    public LayoutElementViewModel(MenuLayoutElement element)
    {
        Element = element;
    }

    public MenuLayoutElement Element { get; }

    public string Type => Element.Type;

    public bool IsText => Element is TextElement;

    public bool IsButtonMask => Element is ButtonMaskElement;

    public string Id
    {
        get => Element.Id;
        set
        {
            if (Element.Id != value)
            {
                Element.Id = value;
                OnPropertyChanged();
            }
        }
    }

    public int X
    {
        get => Element.X;
        set
        {
            if (Element.X != value)
            {
                Element.X = value;
                OnPropertyChanged();
            }
        }
    }

    public int Y
    {
        get => Element.Y;
        set
        {
            if (Element.Y != value)
            {
                Element.Y = value;
                OnPropertyChanged();
            }
        }
    }

    public int Width
    {
        get => Element.Width;
        set
        {
            if (Element.Width != value)
            {
                Element.Width = value;
                OnPropertyChanged();
            }
        }
    }

    public int Height
    {
        get => Element.Height;
        set
        {
            if (Element.Height != value)
            {
                Element.Height = value;
                OnPropertyChanged();
            }
        }
    }

    public string Text
    {
        get => TextElement?.Text ?? string.Empty;
        set
        {
            if (TextElement is not null && TextElement.Text != value)
            {
                TextElement.Text = value;
                OnPropertyChanged();
            }
        }
    }

    public string FontKey
    {
        get => TextElement?.FontKey ?? string.Empty;
        set
        {
            if (TextElement is not null && TextElement.FontKey != value)
            {
                TextElement.FontKey = value;
                OnPropertyChanged();
            }
        }
    }

    public double FontScale
    {
        get => TextElement?.FontScale ?? 1;
        set
        {
            if (TextElement is not null && Math.Abs(TextElement.FontScale - value) > double.Epsilon)
            {
                TextElement.FontScale = value;
                OnPropertyChanged();
            }
        }
    }

    public string Color
    {
        get => TextElement?.Color ?? string.Empty;
        set
        {
            if (TextElement is not null && TextElement.Color != value)
            {
                TextElement.Color = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedFontColorOption));
                OnPropertyChanged(nameof(ColorBrush));
            }
        }
    }

    public IReadOnlyList<FontColorOption> FontColorOptions => AvailableFontColorOptions;

    public IBrush ColorBrush => new SolidColorBrush(ParseColor(Color));

    public FontColorOption? SelectedFontColorOption
    {
        get => FontColorOptions.FirstOrDefault(option =>
            string.Equals(option.Hex, Color, StringComparison.OrdinalIgnoreCase));
        set
        {
            if (value is not null)
            {
                Color = value.Hex;
            }
        }
    }

    public string HorizontalAlignment
    {
        get => TextElement?.HorizontalAlignment ?? string.Empty;
        set
        {
            if (TextElement is not null && TextElement.HorizontalAlignment != value)
            {
                TextElement.HorizontalAlignment = value;
                OnPropertyChanged();
            }
        }
    }

    public string Action
    {
        get => ButtonMaskElement?.Action ?? string.Empty;
        set
        {
            if (ButtonMaskElement is not null && ButtonMaskElement.Action != value)
            {
                ButtonMaskElement.Action = value;
                OnPropertyChanged();
            }
        }
    }

    public string Label
    {
        get => ButtonMaskElement?.Label ?? string.Empty;
        set
        {
            if (ButtonMaskElement is not null && ButtonMaskElement.Label != value)
            {
                ButtonMaskElement.Label = value;
                OnPropertyChanged();
            }
        }
    }

    public bool Enabled
    {
        get => ButtonMaskElement?.Enabled ?? false;
        set
        {
            if (ButtonMaskElement is not null && ButtonMaskElement.Enabled != value)
            {
                ButtonMaskElement.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public MenuLayoutRect Bounds => Element.Bounds;

    private TextElement? TextElement => Element as TextElement;

    private ButtonMaskElement? ButtonMaskElement => Element as ButtonMaskElement;

    public void RefreshBounds()
    {
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(Y));
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(Bounds));
    }

    private static Color ParseColor(string color)
    {
        string value = color.TrimStart('#');
        try
        {
            if (value.Length == 6)
            {
                byte r = Convert.ToByte(value[..2], 16);
                byte g = Convert.ToByte(value.Substring(2, 2), 16);
                byte b = Convert.ToByte(value.Substring(4, 2), 16);
                return Avalonia.Media.Color.FromRgb(r, g, b);
            }

            if (value.Length == 8)
            {
                byte a = Convert.ToByte(value[..2], 16);
                byte r = Convert.ToByte(value.Substring(2, 2), 16);
                byte g = Convert.ToByte(value.Substring(4, 2), 16);
                byte b = Convert.ToByte(value.Substring(6, 2), 16);
                return Avalonia.Media.Color.FromArgb(a, r, g, b);
            }
        }
        catch (FormatException)
        {
            return Colors.Transparent;
        }

        return Colors.Transparent;
    }
}

public sealed record FontColorOption(string Name, string Hex)
{
    public string DisplayName => $"{Name} ({Hex})";
}
