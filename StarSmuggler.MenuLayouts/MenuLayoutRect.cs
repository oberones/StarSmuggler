namespace StarSmuggler.MenuLayouts;

public readonly record struct MenuLayoutRect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;

    public int Bottom => Y + Height;

    public bool HasPositiveSize => Width > 0 && Height > 0;

    public bool IsInside(int canvasWidth, int canvasHeight)
    {
        return X >= 0 &&
            Y >= 0 &&
            Width > 0 &&
            Height > 0 &&
            Right <= canvasWidth &&
            Bottom <= canvasHeight;
    }

    public bool Intersects(MenuLayoutRect other)
    {
        return X < other.Right &&
            Right > other.X &&
            Y < other.Bottom &&
            Bottom > other.Y;
    }
}
