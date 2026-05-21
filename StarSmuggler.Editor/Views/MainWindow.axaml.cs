using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using StarSmuggler.Editor.ViewModels;

namespace StarSmuggler.Editor.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new MainWindowViewModel();
    }
}
