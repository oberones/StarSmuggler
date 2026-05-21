namespace StarSmuggler.Editor.Services;

public sealed class ImageImportService
{
    public ImageImportResult OpenImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return ImageImportResult.Fail("Image path is required.");
        }

        string fullPath = Path.GetFullPath(imagePath);
        if (!File.Exists(fullPath))
        {
            return ImageImportResult.Fail($"Image file was not found: {fullPath}");
        }

        return ImageImportResult.Success(fullPath, NormalizeContentAssetKey(fullPath));
    }

    /// <summary>
    /// Converts repo-local Content paths to game asset keys so JSON never stores local absolute paths.
    /// For example, Content/UI/MainMenu.png becomes UI/MainMenu.
    /// </summary>
    public string NormalizeContentAssetKey(string imagePath)
    {
        string fullPath = Path.GetFullPath(imagePath);
        string repoRoot = FindRepositoryRoot(fullPath);
        string contentRoot = Path.Combine(repoRoot, "Content") + Path.DirectorySeparatorChar;

        if (fullPath.StartsWith(contentRoot, StringComparison.OrdinalIgnoreCase))
        {
            string relativeToContent = Path.GetRelativePath(Path.Combine(repoRoot, "Content"), fullPath);
            string withoutExtension = Path.Combine(
                Path.GetDirectoryName(relativeToContent) ?? string.Empty,
                Path.GetFileNameWithoutExtension(relativeToContent));
            return withoutExtension.Replace(Path.DirectorySeparatorChar, '/');
        }

        return Path.GetFileNameWithoutExtension(fullPath);
    }

    public string FindDefaultMenuImage()
    {
        string repoRoot = FindRepositoryRoot(Environment.CurrentDirectory);
        return Path.Combine(repoRoot, "Content", "UI", "MainMenu.png");
    }

    private static string FindRepositoryRoot(string startPath)
    {
        var directory = File.Exists(startPath)
            ? Directory.GetParent(Path.GetFullPath(startPath))
            : new DirectoryInfo(Path.GetFullPath(startPath));

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "StarSmuggler.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Environment.CurrentDirectory;
    }
}

public sealed record ImageImportResult(bool Succeeded, string? ImagePath, string? BackgroundAsset, string? ErrorMessage)
{
    public static ImageImportResult Success(string imagePath, string backgroundAsset)
    {
        return new ImageImportResult(true, imagePath, backgroundAsset, null);
    }

    public static ImageImportResult Fail(string errorMessage)
    {
        return new ImageImportResult(false, null, null, errorMessage);
    }
}
