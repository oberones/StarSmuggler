using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StarSmuggler.Editor.Views;

public sealed partial class PropertyPanel : UserControl
{
    public PropertyPanel()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
