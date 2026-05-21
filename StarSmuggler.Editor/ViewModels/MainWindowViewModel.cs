using System.Collections.ObjectModel;
using StarSmuggler.Editor.Services;
using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ImageImportService imageImportService;
    private readonly LayoutFileService layoutFileService;
    private string? backgroundImagePath;
    private string backgroundAsset = "UI/MainMenu";
    private LayoutElementViewModel? selectedElement;

    public MainWindowViewModel()
        : this(new ImageImportService(), new LayoutFileService())
    {
    }

    public MainWindowViewModel(ImageImportService imageImportService, LayoutFileService layoutFileService)
    {
        this.imageImportService = imageImportService;
        this.layoutFileService = layoutFileService;

        Document = new MenuLayoutDocument();
        OpenImageCommand = new RelayCommand(OpenDefaultImage);
        OpenLayoutCommand = new RelayCommand(OpenDefaultLayout);
        SaveLayoutCommand = new RelayCommand(SaveDefaultLayout);
        AddTextCommand = new RelayCommand(AddText);
        AddButtonMaskCommand = new RelayCommand(AddButtonMask);
        DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => SelectedElement is not null);
    }

    public MenuLayoutDocument Document { get; private set; }

    public ObservableCollection<LayoutElementViewModel> Elements { get; } = new();

    public ObservableCollection<ValidationMessageViewModel> ValidationMessages { get; } = new();

    public string? BackgroundImagePath
    {
        get => backgroundImagePath;
        private set => SetProperty(ref backgroundImagePath, value);
    }

    public string BackgroundAsset
    {
        get => backgroundAsset;
        set
        {
            if (SetProperty(ref backgroundAsset, value))
            {
                Document.BackgroundAsset = value;
            }
        }
    }

    public LayoutElementViewModel? SelectedElement
    {
        get => selectedElement;
        set
        {
            if (SetProperty(ref selectedElement, value))
            {
                OnPropertyChanged(nameof(HasSelection));
                DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasSelection => SelectedElement is not null;

    public RelayCommand OpenImageCommand { get; }

    public RelayCommand OpenLayoutCommand { get; }

    public RelayCommand SaveLayoutCommand { get; }

    public RelayCommand AddTextCommand { get; }

    public RelayCommand AddButtonMaskCommand { get; }

    public RelayCommand DeleteSelectedCommand { get; }

    public void Select(LayoutElementViewModel? element)
    {
        SelectedElement = element;
    }

    public void ValidateDraft()
    {
        ValidationMessages.Clear();
        var result = MenuLayoutValidator.Validate(Document);
        foreach (var issue in result.Issues)
        {
            ValidationMessages.Add(new ValidationMessageViewModel(issue));
        }
    }

    private void OpenDefaultImage()
    {
        var result = imageImportService.OpenImage(imageImportService.FindDefaultMenuImage());
        if (!result.Succeeded)
        {
            ShowValidationMessage("image.open_failed", result.ErrorMessage ?? "Image could not be opened.");
            return;
        }

        BackgroundImagePath = result.ImagePath;
        BackgroundAsset = result.BackgroundAsset ?? "UI/MainMenu";
    }

    private void OpenDefaultLayout()
    {
        var result = layoutFileService.Load(layoutFileService.GetDefaultLayoutPath());
        if (!result.Loaded || result.Document is null)
        {
            ShowValidationMessage("layout.open_failed", result.WarningMessage ?? "Layout could not be opened.");
            return;
        }

        LoadDocument(result.Document);
    }

    private void SaveDefaultLayout()
    {
        ValidateDraft();
        if (ValidationMessages.Count > 0)
        {
            return;
        }

        layoutFileService.Save(layoutFileService.GetDefaultLayoutPath(), Document);
        ShowValidationMessage("layout.saved", "Layout saved.");
    }

    private void AddText()
    {
        var element = new TextElement
        {
            Id = CreateUniqueId("text"),
            X = 520,
            Y = 260,
            Width = 500,
            Height = 64,
            Text = "Menu Text",
            FontKey = "Fonts/TerminalBold",
            FontScale = 1.0,
            Color = "#FFFFFFFF",
            HorizontalAlignment = nameof(HorizontalTextAlignment.Center)
        };

        AddElement(element);
    }

    private void AddButtonMask()
    {
        var element = new ButtonMaskElement
        {
            Id = CreateUniqueId("button"),
            X = 700,
            Y = 450,
            Width = 200,
            Height = 50,
            Action = nameof(MenuButtonAction.NewGame),
            Label = "New Game",
            Enabled = true
        };

        AddElement(element);
    }

    private void DeleteSelected()
    {
        if (SelectedElement is null)
        {
            return;
        }

        Document.Elements.Remove(SelectedElement.Element);
        Elements.Remove(SelectedElement);
        SelectedElement = null;
    }

    private void AddElement(MenuLayoutElement element)
    {
        Document.Elements.Add(element);
        var viewModel = new LayoutElementViewModel(element);
        Elements.Add(viewModel);
        SelectedElement = viewModel;
    }

    private void LoadDocument(MenuLayoutDocument document)
    {
        Document = document;
        BackgroundAsset = document.BackgroundAsset;
        Elements.Clear();
        foreach (var element in document.Elements)
        {
            Elements.Add(new LayoutElementViewModel(element));
        }

        SelectedElement = null;
        ValidationMessages.Clear();
        OnPropertyChanged(nameof(Document));
    }

    private string CreateUniqueId(string prefix)
    {
        int index = 1;
        while (Document.Elements.Any(element => string.Equals(element.Id, $"{prefix}-{index}", StringComparison.OrdinalIgnoreCase)))
        {
            index++;
        }

        return $"{prefix}-{index}";
    }

    private void ShowValidationMessage(string code, string message)
    {
        ValidationMessages.Clear();
        ValidationMessages.Add(new ValidationMessageViewModel(code, message));
    }
}
