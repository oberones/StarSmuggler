using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.Services;

public static class EditorCoordinateService
{
    /// <summary>
    /// Keeps editor drag/resize operations inside the source canvas so exported layouts never
    /// need off-canvas clipping rules in the game runtime.
    /// </summary>
    public static MenuLayoutRect ClampToCanvas(MenuLayoutRect proposed, int canvasWidth, int canvasHeight)
    {
        int width = Math.Clamp(proposed.Width, 1, Math.Max(1, canvasWidth));
        int height = Math.Clamp(proposed.Height, 1, Math.Max(1, canvasHeight));
        int x = Math.Clamp(proposed.X, 0, Math.Max(0, canvasWidth - width));
        int y = Math.Clamp(proposed.Y, 0, Math.Max(0, canvasHeight - height));
        return new MenuLayoutRect(x, y, width, height);
    }
}
