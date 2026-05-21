using StarSmuggler.MenuLayouts;
using StarSmuggler.Tests.MenuLayouts;
using Xunit;

namespace StarSmuggler.Tests.Runtime;

public sealed class MainMenuLayoutLoaderTests
{
    [Fact]
    public void TryLoadReturnsDocumentForValidLayout()
    {
        string path = WriteTempLayout(MenuLayoutTestData.CreateValidDocument());

        var result = MenuLayoutLoader.TryLoad(path);

        Assert.True(result.Loaded);
        Assert.NotNull(result.Document);
        Assert.Equal(MenuLayoutFallbackReason.None, result.FallbackReason);
    }

    [Fact]
    public void TryLoadReturnsFallbackForMissingLayout()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "main-menu.json");

        var result = MenuLayoutLoader.TryLoad(path);

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.MissingFile, result.FallbackReason);
        Assert.Contains("not found", result.WarningMessage);
    }

    [Fact]
    public void TryLoadReturnsFallbackForMalformedJson()
    {
        string path = WriteTempJson("{ nope");

        var result = MenuLayoutLoader.TryLoad(path);

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.InvalidJson, result.FallbackReason);
    }

    [Fact]
    public void TryLoadClassifiesUnsupportedVersion()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        document.Version = 99;

        var result = MenuLayoutLoader.TryLoad(WriteTempLayout(document));

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.UnsupportedVersion, result.FallbackReason);
    }

    [Fact]
    public void TryLoadClassifiesUnsupportedAction()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var button = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        button.Action = "Warp";

        var result = MenuLayoutLoader.TryLoad(WriteTempLayout(document));

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.UnsupportedAction, result.FallbackReason);
    }

    [Fact]
    public void TryLoadClassifiesUnknownElementType()
    {
        string json = """
        {
          "Version": 1,
          "CanvasWidth": 1536,
          "CanvasHeight": 1024,
          "BackgroundAsset": "UI/MainMenu",
          "Elements": [
            {
              "Type": "PolygonMask",
              "Id": "poly",
              "X": 0,
              "Y": 0,
              "Width": 10,
              "Height": 10
            }
          ]
        }
        """;

        var result = MenuLayoutLoader.TryLoad(WriteTempJson(json));

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.UnknownElementType, result.FallbackReason);
    }

    [Fact]
    public void TryLoadRejectsOverlappingEnabledMasks()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var first = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        var second = Assert.IsType<ButtonMaskElement>(document.Elements[2]);
        second.X = first.X + 1;
        second.Y = first.Y + 1;

        var result = MenuLayoutLoader.TryLoad(WriteTempLayout(document));

        Assert.False(result.Loaded);
        Assert.Equal(MenuLayoutFallbackReason.InvalidLayout, result.FallbackReason);
    }

    [Fact]
    public void DisabledButtonMasksRemainValidWhenOverlapping()
    {
        var document = MenuLayoutTestData.CreateValidDocument();
        var first = Assert.IsType<ButtonMaskElement>(document.Elements[1]);
        var second = Assert.IsType<ButtonMaskElement>(document.Elements[2]);
        second.X = first.X + 1;
        second.Y = first.Y + 1;
        second.Enabled = false;

        var result = MenuLayoutLoader.TryLoad(WriteTempLayout(document));

        Assert.True(result.Loaded);
    }

    [Fact]
    public void TryLoadReturnsIoFallbackForUnreadableLayoutWhenSupported()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        string path = WriteTempLayout(MenuLayoutTestData.CreateValidDocument());
        UnixFileMode originalMode = File.GetUnixFileMode(path);

        try
        {
            File.SetUnixFileMode(path, UnixFileMode.None);

            var result = MenuLayoutLoader.TryLoad(path);

            Assert.False(result.Loaded);
            Assert.Equal(MenuLayoutFallbackReason.IoError, result.FallbackReason);
        }
        finally
        {
            File.SetUnixFileMode(path, originalMode);
        }
    }

    [Fact]
    public void SaveSupportsPathWithoutDirectoryComponent()
    {
        string originalDirectory = Environment.CurrentDirectory;
        string directory = Path.Combine(Path.GetTempPath(), "StarSmugglerMenuLayoutTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            Environment.CurrentDirectory = directory;

            MenuLayoutLoader.Save("main-menu.json", MenuLayoutTestData.CreateValidDocument());

            Assert.True(File.Exists(Path.Combine(directory, "main-menu.json")));
        }
        finally
        {
            Environment.CurrentDirectory = originalDirectory;
        }
    }

    private static string WriteTempLayout(MenuLayoutDocument document)
    {
        return WriteTempJson(MenuLayoutJson.Serialize(document));
    }

    private static string WriteTempJson(string json)
    {
        string directory = Path.Combine(Path.GetTempPath(), "StarSmugglerMenuLayoutTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "main-menu.json");
        File.WriteAllText(path, json);
        return path;
    }
}
