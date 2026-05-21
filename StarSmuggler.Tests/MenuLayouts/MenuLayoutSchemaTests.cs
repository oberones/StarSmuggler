using System.Text.Json;
using StarSmuggler.MenuLayouts;
using Xunit;

namespace StarSmuggler.Tests.MenuLayouts;

public sealed class MenuLayoutSchemaTests
{
    [Fact]
    public void SchemaContainsSupportedElementTypesAndActions()
    {
        string schemaPath = Path.Combine(
            FindRepositoryRoot(),
            "specs",
            "001-menu-layout-editor",
            "contracts",
            "main-menu-layout.schema.json");

        using var document = JsonDocument.Parse(File.ReadAllText(schemaPath));
        string schemaText = document.RootElement.GetRawText();

        Assert.Contains(MenuLayoutElementTypes.Text, schemaText);
        Assert.Contains(MenuLayoutElementTypes.ButtonMask, schemaText);
        Assert.Contains(nameof(MenuButtonAction.NewGame), schemaText);
        Assert.Contains(nameof(MenuButtonAction.LoadGame), schemaText);
        Assert.Contains(nameof(MenuButtonAction.SaveGame), schemaText);
        Assert.Contains(nameof(MenuButtonAction.Quit), schemaText);
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
