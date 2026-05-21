using StarSmuggler.MenuLayouts;
using Xunit;

namespace StarSmuggler.Tests.MenuLayouts;

public sealed class MenuLayoutJsonTests
{
    [Fact]
    public void RoundTripPreservesTextAndButtonMaskFields()
    {
        var original = MenuLayoutTestData.CreateValidDocument();

        var json = MenuLayoutJson.Serialize(original);
        var roundTripped = MenuLayoutJson.Deserialize(json);

        Assert.NotNull(roundTripped);
        Assert.Equal(original.Version, roundTripped.Version);
        Assert.Equal(original.CanvasWidth, roundTripped.CanvasWidth);
        Assert.Equal(original.CanvasHeight, roundTripped.CanvasHeight);
        Assert.Equal(original.BackgroundAsset, roundTripped.BackgroundAsset);
        Assert.IsType<TextElement>(roundTripped.Elements[0]);
        Assert.IsType<ButtonMaskElement>(roundTripped.Elements[1]);
        Assert.Equal(original.Elements.Count, roundTripped.Elements.Count);
    }

    [Fact]
    public void SerializeUsesIndentedPascalCaseJson()
    {
        var json = MenuLayoutJson.Serialize(MenuLayoutTestData.CreateValidDocument());

        Assert.Contains(Environment.NewLine, json);
        Assert.Contains("\"Version\"", json);
        Assert.Contains("\"CanvasWidth\"", json);
        Assert.Contains("\"BackgroundAsset\"", json);
        Assert.Contains("\"Elements\"", json);
        Assert.DoesNotContain("\"canvasWidth\"", json);
    }
}
