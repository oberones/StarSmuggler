namespace StarSmuggler.MenuLayouts;

public static class CoordinateScaler
{
    /// <summary>
    /// Converts source-canvas coordinates to the active runtime viewport.
    /// Scaling is intentionally independent on X and Y to match the game's full-window background draw.
    /// </summary>
    public static MenuLayoutRect ScaleRect(
        MenuLayoutRect source,
        int sourceCanvasWidth,
        int sourceCanvasHeight,
        int viewportWidth,
        int viewportHeight)
    {
        if (sourceCanvasWidth <= 0 || sourceCanvasHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceCanvasWidth), "Source canvas dimensions must be positive.");
        }

        if (viewportWidth <= 0 || viewportHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewportWidth), "Viewport dimensions must be positive.");
        }

        double scaleX = (double)viewportWidth / sourceCanvasWidth;
        double scaleY = (double)viewportHeight / sourceCanvasHeight;

        return new MenuLayoutRect(
            ScaleValue(source.X, scaleX),
            ScaleValue(source.Y, scaleY),
            Math.Max(1, ScaleValue(source.Width, scaleX)),
            Math.Max(1, ScaleValue(source.Height, scaleY)));
    }

    public static double ScaleFontScale(double sourceFontScale, int sourceCanvasHeight, int viewportHeight)
    {
        if (sourceCanvasHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceCanvasHeight), "Source canvas height must be positive.");
        }

        if (viewportHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewportHeight), "Viewport height must be positive.");
        }

        return sourceFontScale * viewportHeight / sourceCanvasHeight;
    }

    private static int ScaleValue(int value, double scale)
    {
        return (int)Math.Round(value * scale, MidpointRounding.AwayFromZero);
    }
}
