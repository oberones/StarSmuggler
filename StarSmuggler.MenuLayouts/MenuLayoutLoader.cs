namespace StarSmuggler.MenuLayouts;

public static class MenuLayoutLoader
{
    /// <summary>
    /// Loads and validates a layout file without throwing for expected fallback cases.
    /// The game uses the fallback reason to preserve the hardcoded menu and log a clear warning.
    /// </summary>
    public static MenuLayoutLoadResult TryLoad(string path)
    {
        if (!File.Exists(path))
        {
            return MenuLayoutLoadResult.Fallback(
                MenuLayoutFallbackReason.MissingFile,
                $"Main menu layout file was not found: {path}");
        }

        string json;
        try
        {
            json = File.ReadAllText(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return MenuLayoutLoadResult.Fallback(
                MenuLayoutFallbackReason.IoError,
                $"Main menu layout could not be read: {ex.Message}");
        }

        MenuLayoutDocument? document;
        try
        {
            document = MenuLayoutJson.Deserialize(json);
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException or NotSupportedException)
        {
            return MenuLayoutLoadResult.Fallback(
                MenuLayoutFallbackReason.InvalidJson,
                $"Main menu layout JSON is invalid: {ex.Message}");
        }

        var validation = MenuLayoutValidator.Validate(document);
        if (!validation.IsValid)
        {
            var reason = Classify(validation);
            string issueList = string.Join("; ", validation.Issues.Select(issue => issue.Message));
            return MenuLayoutLoadResult.Fallback(
                reason,
                $"Main menu layout failed validation: {issueList}",
                validation);
        }

        return MenuLayoutLoadResult.Success(document!);
    }

    public static void Save(string path, MenuLayoutDocument document)
    {
        var validation = MenuLayoutValidator.Validate(document);
        if (!validation.IsValid)
        {
            string issueList = string.Join("; ", validation.Issues.Select(issue => issue.Message));
            throw new InvalidOperationException($"Cannot save invalid menu layout: {issueList}");
        }

        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, MenuLayoutJson.Serialize(document));
    }

    private static MenuLayoutFallbackReason Classify(MenuLayoutValidationResult validation)
    {
        if (validation.Issues.Any(issue => issue.Code == "version.unsupported"))
        {
            return MenuLayoutFallbackReason.UnsupportedVersion;
        }

        if (validation.Issues.Any(issue => issue.Code == "button.action.unsupported"))
        {
            return MenuLayoutFallbackReason.UnsupportedAction;
        }

        if (validation.Issues.Any(issue => issue.Code == "element.type.unsupported"))
        {
            return MenuLayoutFallbackReason.UnknownElementType;
        }

        return MenuLayoutFallbackReason.InvalidLayout;
    }
}
