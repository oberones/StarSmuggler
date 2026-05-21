using StarSmuggler.MenuLayouts;
using Xunit;

namespace StarSmuggler.Tests.MenuLayouts;

public sealed class CoordinateScalerTests
{
    [Fact]
    public void ScaleRectScalesIndependentlyFromSourceCanvasToViewport()
    {
        var source = new MenuLayoutRect(700, 450, 200, 50);

        var scaled = CoordinateScaler.ScaleRect(source, 1536, 1024, 768, 512);

        Assert.Equal(new MenuLayoutRect(350, 225, 100, 25), scaled);
    }

    [Fact]
    public void ScaleRectRejectsInvalidCanvasDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CoordinateScaler.ScaleRect(new MenuLayoutRect(0, 0, 10, 10), 0, 1024, 768, 512));
    }
}
