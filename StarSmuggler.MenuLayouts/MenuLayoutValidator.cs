using System.Text.RegularExpressions;

namespace StarSmuggler.MenuLayouts;

public static partial class MenuLayoutValidator
{
    public static MenuLayoutValidationResult Validate(MenuLayoutDocument? document)
    {
        var issues = new List<MenuLayoutValidationIssue>();

        if (document is null)
        {
            issues.Add(new MenuLayoutValidationIssue("document.missing", "Layout document is missing."));
            return new MenuLayoutValidationResult(issues);
        }

        if (document.Version != MenuLayoutDocument.CurrentVersion)
        {
            issues.Add(new MenuLayoutValidationIssue(
                "version.unsupported",
                $"Layout version {document.Version} is not supported."));
        }

        if (document.CanvasWidth <= 0 || document.CanvasHeight <= 0)
        {
            issues.Add(new MenuLayoutValidationIssue(
                "canvas.invalid",
                "Canvas width and height must be positive."));
        }

        if (string.IsNullOrWhiteSpace(document.BackgroundAsset))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "background.required",
                "Background asset is required."));
        }
        else if (IsAbsolutePath(document.BackgroundAsset))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "background.absolute_path",
                "Background asset must be a content key or repo-relative path."));
        }

        ValidateElements(document, issues);
        return issues.Count == 0 ? MenuLayoutValidationResult.Valid : new MenuLayoutValidationResult(issues);
    }

    private static void ValidateElements(MenuLayoutDocument document, List<MenuLayoutValidationIssue> issues)
    {
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var enabledButtonMasks = new List<ButtonMaskElement>();

        foreach (var element in document.Elements ?? new List<MenuLayoutElement>())
        {
            string? elementId = string.IsNullOrWhiteSpace(element.Id) ? null : element.Id;

            if (string.IsNullOrWhiteSpace(element.Id))
            {
                issues.Add(new MenuLayoutValidationIssue(
                    "element.id.required",
                    "Element id is required.",
                    elementId));
            }
            else if (!ids.Add(element.Id.Trim()))
            {
                issues.Add(new MenuLayoutValidationIssue(
                    "element.id.duplicate",
                    $"Element id '{element.Id}' must be unique across the layout.",
                    element.Id));
            }

            if (!element.Bounds.HasPositiveSize)
            {
                issues.Add(new MenuLayoutValidationIssue(
                    "element.bounds.non_positive",
                    "Element width and height must be positive.",
                    elementId));
            }
            else if (!element.Bounds.IsInside(document.CanvasWidth, document.CanvasHeight))
            {
                issues.Add(new MenuLayoutValidationIssue(
                    "element.bounds.out_of_canvas",
                    "Element bounds must stay inside the source canvas.",
                    elementId));
            }

            switch (element)
            {
                case TextElement textElement:
                    ValidateTextElement(textElement, issues);
                    break;
                case ButtonMaskElement buttonMaskElement:
                    ValidateButtonMaskElement(buttonMaskElement, issues, enabledButtonMasks);
                    break;
                default:
                    issues.Add(new MenuLayoutValidationIssue(
                        "element.type.unsupported",
                        $"Element type '{element.Type}' is not supported.",
                        elementId));
                    break;
            }
        }

        ValidateButtonMaskOverlap(enabledButtonMasks, issues);
    }

    private static void ValidateTextElement(TextElement textElement, List<MenuLayoutValidationIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(textElement.Text))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "text.text.required",
                "Text element display text is required.",
                textElement.Id));
        }

        if (string.IsNullOrWhiteSpace(textElement.FontKey))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "text.font.required",
                "Text element font key is required.",
                textElement.Id));
        }

        if (textElement.FontScale <= 0)
        {
            issues.Add(new MenuLayoutValidationIssue(
                "text.font_scale.invalid",
                "Text element font scale must be greater than zero.",
                textElement.Id));
        }

        if (!HexColorRegex().IsMatch(textElement.Color ?? string.Empty))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "text.color.invalid",
                "Text element color must be #RRGGBB or #AARRGGBB.",
                textElement.Id));
        }

        if (!Enum.TryParse<HorizontalTextAlignment>(textElement.HorizontalAlignment, ignoreCase: false, out _))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "text.alignment.unsupported",
                $"Text element alignment '{textElement.HorizontalAlignment}' is not supported.",
                textElement.Id));
        }
    }

    private static void ValidateButtonMaskElement(
        ButtonMaskElement buttonMaskElement,
        List<MenuLayoutValidationIssue> issues,
        List<ButtonMaskElement> enabledButtonMasks)
    {
        if (!Enum.TryParse<MenuButtonAction>(buttonMaskElement.Action, ignoreCase: false, out _))
        {
            issues.Add(new MenuLayoutValidationIssue(
                "button.action.unsupported",
                $"Button action '{buttonMaskElement.Action}' is not supported.",
                buttonMaskElement.Id));
        }

        if (buttonMaskElement.Enabled && buttonMaskElement.Bounds.HasPositiveSize)
        {
            enabledButtonMasks.Add(buttonMaskElement);
        }
    }

    private static void ValidateButtonMaskOverlap(
        IReadOnlyList<ButtonMaskElement> enabledButtonMasks,
        List<MenuLayoutValidationIssue> issues)
    {
        for (int first = 0; first < enabledButtonMasks.Count; first++)
        {
            for (int second = first + 1; second < enabledButtonMasks.Count; second++)
            {
                var firstMask = enabledButtonMasks[first];
                var secondMask = enabledButtonMasks[second];

                if (firstMask.Bounds.Intersects(secondMask.Bounds))
                {
                    issues.Add(new MenuLayoutValidationIssue(
                        "button.bounds.overlap",
                        $"Enabled button masks '{firstMask.Id}' and '{secondMask.Id}' must not overlap.",
                        firstMask.Id));
                }
            }
        }
    }

    private static bool IsAbsolutePath(string value)
    {
        return Path.IsPathRooted(value) || WindowsAbsolutePathRegex().IsMatch(value);
    }

    [GeneratedRegex(@"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$")]
    private static partial Regex HexColorRegex();

    [GeneratedRegex(@"^[A-Za-z]:[\\/]|^\\\\")]
    private static partial Regex WindowsAbsolutePathRegex();
}
