namespace StarSmuggler.MenuLayouts;

public enum MenuLayoutFallbackReason
{
    None,
    MissingFile,
    IoError,
    InvalidJson,
    UnsupportedVersion,
    UnsupportedAction,
    UnknownElementType,
    InvalidLayout
}

public sealed class MenuLayoutLoadResult
{
    private MenuLayoutLoadResult(
        bool loaded,
        MenuLayoutDocument? document,
        MenuLayoutFallbackReason fallbackReason,
        string? warningMessage,
        MenuLayoutValidationResult? validationResult)
    {
        Loaded = loaded;
        Document = document;
        FallbackReason = fallbackReason;
        WarningMessage = warningMessage;
        ValidationResult = validationResult;
    }

    public bool Loaded { get; }

    public MenuLayoutDocument? Document { get; }

    public MenuLayoutFallbackReason FallbackReason { get; }

    public string? WarningMessage { get; }

    public MenuLayoutValidationResult? ValidationResult { get; }

    public static MenuLayoutLoadResult Success(MenuLayoutDocument document)
    {
        return new(true, document, MenuLayoutFallbackReason.None, null, MenuLayoutValidationResult.Valid);
    }

    public static MenuLayoutLoadResult Fallback(
        MenuLayoutFallbackReason reason,
        string warningMessage,
        MenuLayoutValidationResult? validationResult = null)
    {
        return new(false, null, reason, warningMessage, validationResult);
    }
}
