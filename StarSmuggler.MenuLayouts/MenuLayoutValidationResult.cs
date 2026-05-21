namespace StarSmuggler.MenuLayouts;

public sealed class MenuLayoutValidationIssue
{
    public MenuLayoutValidationIssue(string code, string message, string? elementId = null)
    {
        Code = code;
        Message = message;
        ElementId = elementId;
    }

    public string Code { get; }

    public string Message { get; }

    public string? ElementId { get; }
}

public sealed class MenuLayoutValidationResult
{
    public MenuLayoutValidationResult(IReadOnlyList<MenuLayoutValidationIssue> issues)
    {
        Issues = issues;
    }

    public bool IsValid => Issues.Count == 0;

    public IReadOnlyList<MenuLayoutValidationIssue> Issues { get; }

    public static MenuLayoutValidationResult Valid { get; } = new(Array.Empty<MenuLayoutValidationIssue>());
}
