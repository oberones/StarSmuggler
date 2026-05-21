using StarSmuggler.MenuLayouts;
using Xunit;

namespace StarSmuggler.Tests.MenuLayouts;

public sealed class MenuLayoutValidationTests
{
    [Fact]
    public void ValidateAcceptsCompleteMainMenuLayout()
    {
        var result = MenuLayoutValidator.Validate(MenuLayoutTestData.CreateValidDocument());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRejectsMissingElementIds(string id)
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.Elements[0].Id = id;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "element.id.required");
    }

    [Fact]
    public void ValidateRejectsDuplicateElementIdsAcrossTypes()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.Elements[1].Id = document.Elements[0].Id;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "element.id.duplicate");
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(20, 0)]
    [InlineData(-1, 20)]
    public void ValidateRejectsNonPositiveBounds(int width, int height)
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.Elements[0].Width = width;
        document.Elements[0].Height = height;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "element.bounds.non_positive");
    }

    [Fact]
    public void ValidateRejectsOutOfCanvasBounds()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.Elements[0].X = document.CanvasWidth - 10;
        document.Elements[0].Width = 20;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "element.bounds.out_of_canvas");
    }

    [Fact]
    public void ValidateRejectsUnsupportedActions()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var button = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        button.Action = "LaunchMissiles";

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "button.action.unsupported");
    }

    [Fact]
    public void ValidateRejectsInvalidTextStyleFields()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var text = Assert.IsType<TextElement>(document.Elements[0]);
        text.FontScale = 0;
        text.Color = "white";
        text.HorizontalAlignment = "Middle";

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "text.font_scale.invalid");
        Assert.Contains(result.Issues, issue => issue.Code == "text.color.invalid");
        Assert.Contains(result.Issues, issue => issue.Code == "text.alignment.unsupported");
    }

    [Fact]
    public void ValidateRejectsOverlappingEnabledButtonMasks()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var first = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        var second = Assert.IsType<ButtonMaskElement>(document.Elements[2]);
        second.X = first.X + 10;
        second.Y = first.Y + 10;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "button.bounds.overlap");
    }

    [Fact]
    public void ValidateAllowsOverlappingDisabledButtonMasks()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var first = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        var second = Assert.IsType<ButtonMaskElement>(document.Elements[2]);
        second.X = first.X + 10;
        second.Y = first.Y + 10;
        second.Enabled = false;

        var result = MenuLayoutValidator.Validate(document);

        Assert.DoesNotContain(result.Issues, issue => issue.Code == "button.bounds.overlap");
    }
}
