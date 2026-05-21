using StarSmuggler.MenuLayouts;
using StarSmuggler.Editor.Services;
using Xunit;

namespace StarSmuggler.Tests.MenuLayouts;

public sealed class MenuLayoutPathTests
{
    [Theory]
    [InlineData("/Users/example/MainMenu.png")]
    [InlineData("C:\\Users\\example\\MainMenu.png")]
    public void ValidateRejectsAbsoluteBackgroundAssetPaths(string backgroundAsset)
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.BackgroundAsset = backgroundAsset;

        var result = MenuLayoutValidator.Validate(document);

        Assert.Contains(result.Issues, issue => issue.Code == "background.absolute_path");
    }

    [Fact]
    public void ValidateAcceptsContentAssetKeys()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.BackgroundAsset = "UI/MainMenu";

        var result = MenuLayoutValidator.Validate(document);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ImageImportNormalizesRepoContentImageToAssetKey()
    {
        string repoRoot = FindRepositoryRoot();
        string imagePath = Path.Combine(repoRoot, "Content", "UI", "MainMenu.png");
        var service = new ImageImportService();

        string assetKey = service.NormalizeContentAssetKey(imagePath);

        Assert.Equal("UI/MainMenu", assetKey);
    }

    private static string FindRepositoryRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current, "StarSmuggler.sln")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not find repository root.");
    }
}
