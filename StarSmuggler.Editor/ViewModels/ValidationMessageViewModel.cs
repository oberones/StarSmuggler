using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.ViewModels;

public sealed class ValidationMessageViewModel
{
    public ValidationMessageViewModel(string code, string message, string? elementId = null)
    {
        Code = code;
        Message = message;
        ElementId = elementId;
    }

    public ValidationMessageViewModel(MenuLayoutValidationIssue issue)
        : this(issue.Code, issue.Message, issue.ElementId)
    {
    }

    public string Code { get; }

    public string Message { get; }

    public string? ElementId { get; }
}
