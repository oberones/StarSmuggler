using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.Services;

public sealed class LayoutFileService
{
    public string GetDefaultLayoutPath()
    {
        return Path.Combine(FindRepositoryRoot(), "Content", "UI", "MenuLayouts", "main-menu.json");
    }

    public MenuLayoutLoadResult Load(string path)
    {
        return MenuLayoutLoader.TryLoad(path);
    }

    public void Save(string path, MenuLayoutDocument document)
    {
        MenuLayoutLoader.Save(path, document);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(Environment.CurrentDirectory);
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
