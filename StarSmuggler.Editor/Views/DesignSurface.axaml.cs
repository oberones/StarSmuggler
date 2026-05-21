using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using StarSmuggler.Editor.Services;
using StarSmuggler.Editor.ViewModels;
using StarSmuggler.MenuLayouts;

namespace StarSmuggler.Editor.Views;

public sealed partial class DesignSurface : UserControl
{
    private const int CanvasWidth = 1536;
    private const int CanvasHeight = 1024;
    private const double ResizeHandleSize = 12;
    private const double DefaultFontSize = 18;
    private static readonly FontFamily EditorFontFamily =
        FontFamily.Parse("avares://StarSmuggler.Editor/Assets/Fonts#Share Tech Mono");

    private Bitmap? backgroundBitmap;
    private string? loadedBitmapPath;
    private LayoutElementViewModel? activeElement;
    private MenuLayoutRect activeStartBounds;
    private Point dragStartCanvasPoint;
    private DragMode dragMode = DragMode.None;

    public DesignSurface()
    {
        AvaloniaXamlLoader.Load(this);
        ClipToBounds = true;
        DataContextChanged += (_, _) => SubscribeToViewModel();
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var canvasRect = GetCanvasRect(Bounds.Size);
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(9, 13, 18)), Bounds);
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(18, 24, 33)), canvasRect);

        if (DataContext is MainWindowViewModel viewModel)
        {
            DrawBackground(context, viewModel, canvasRect);
            DrawElements(context, viewModel, canvasRect);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
        {
            InvalidateVisual();
        }
    }

    private void SubscribeToViewModel()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.PropertyChanged += (_, _) => InvalidateVisual();
        viewModel.Elements.CollectionChanged += OnElementsChanged;
        foreach (var element in viewModel.Elements)
        {
            element.PropertyChanged += OnElementChanged;
        }
    }

    private void OnElementsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (LayoutElementViewModel element in e.NewItems)
            {
                element.PropertyChanged += OnElementChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (LayoutElementViewModel element in e.OldItems)
            {
                element.PropertyChanged -= OnElementChanged;
            }
        }

        InvalidateVisual();
    }

    private void OnElementChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvalidateVisual();
    }

    private void DrawBackground(DrawingContext context, MainWindowViewModel viewModel, Rect canvasRect)
    {
        if (!string.Equals(loadedBitmapPath, viewModel.BackgroundImagePath, StringComparison.Ordinal))
        {
            backgroundBitmap?.Dispose();
            backgroundBitmap = null;
            loadedBitmapPath = viewModel.BackgroundImagePath;

            if (!string.IsNullOrWhiteSpace(loadedBitmapPath) && File.Exists(loadedBitmapPath))
            {
                backgroundBitmap = new Bitmap(loadedBitmapPath);
            }
        }

        if (backgroundBitmap is not null)
        {
            context.DrawImage(backgroundBitmap, canvasRect);
        }
    }

    private void DrawElements(DrawingContext context, MainWindowViewModel viewModel, Rect canvasRect)
    {
        foreach (var element in viewModel.Elements)
        {
            var rect = ToScreenRect(element.Bounds, canvasRect);
            var color = element.IsText
                ? Color.FromArgb(220, 91, 164, 255)
                : Color.FromArgb(230, 255, 185, 80);

            var pen = new Pen(new SolidColorBrush(color), element == viewModel.SelectedElement ? 3 : 2);
            if (element.IsText)
            {
                DrawTextPreview(context, element, rect, canvasRect);
            }

            context.DrawRectangle(null, pen, rect);

            if (element == viewModel.SelectedElement)
            {
                context.FillRectangle(new SolidColorBrush(Color.FromArgb(210, 255, 255, 255)), GetResizeHandleRect(rect));
            }
        }
    }

    private static void DrawTextPreview(
        DrawingContext context,
        LayoutElementViewModel element,
        Rect rect,
        Rect canvasRect)
    {
        if (string.IsNullOrWhiteSpace(element.Text))
        {
            return;
        }

        // The editor cannot load MonoGame SpriteFont glyph metrics, so it uses the
        // bundled source font and layout scale as a close preview while runtime stays authoritative.
        double fontSize = GetSourceFontSize(element.FontKey) *
            Math.Max(0.1, element.FontScale) *
            (canvasRect.Height / CanvasHeight);
        var brush = new SolidColorBrush(ParsePreviewColor(element.Color));
        var formattedText = new FormattedText(
            element.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(
                EditorFontFamily,
                FontStyle.Normal,
                element.FontKey.Contains("Bold", StringComparison.OrdinalIgnoreCase) ? FontWeight.Bold : FontWeight.Normal),
            fontSize,
            brush)
        {
            MaxTextWidth = Math.Max(1, rect.Width),
            MaxTextHeight = Math.Max(1, rect.Height),
            TextAlignment = ParseTextAlignment(element.HorizontalAlignment),
            Trimming = TextTrimming.CharacterEllipsis
        };

        double y = rect.Y + Math.Max(0, (rect.Height - formattedText.Height) / 2);
        context.DrawText(formattedText, new Point(rect.X, y));
    }

    private static double GetSourceFontSize(string fontKey)
    {
        if (fontKey.EndsWith("12", StringComparison.OrdinalIgnoreCase))
        {
            return 12;
        }

        if (fontKey.EndsWith("16", StringComparison.OrdinalIgnoreCase))
        {
            return 16;
        }

        return DefaultFontSize;
    }

    private static TextAlignment ParseTextAlignment(string alignment)
    {
        return alignment switch
        {
            nameof(HorizontalTextAlignment.Center) => TextAlignment.Center,
            nameof(HorizontalTextAlignment.Right) => TextAlignment.Right,
            _ => TextAlignment.Left
        };
    }

    private static Color ParsePreviewColor(string color)
    {
        string value = color.TrimStart('#');
        try
        {
            if (value.Length == 6)
            {
                byte r = Convert.ToByte(value[..2], 16);
                byte g = Convert.ToByte(value.Substring(2, 2), 16);
                byte b = Convert.ToByte(value.Substring(4, 2), 16);
                return Color.FromRgb(r, g, b);
            }

            if (value.Length == 8)
            {
                byte a = Convert.ToByte(value[..2], 16);
                byte r = Convert.ToByte(value.Substring(2, 2), 16);
                byte g = Convert.ToByte(value.Substring(4, 2), 16);
                byte b = Convert.ToByte(value.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
        }
        catch (FormatException)
        {
            return Colors.White;
        }

        return Colors.White;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var canvasRect = GetCanvasRect(Bounds.Size);
        if (!canvasRect.Contains(e.GetPosition(this)))
        {
            return;
        }

        var canvasPoint = ToCanvasPoint(e.GetPosition(this), canvasRect);
        // Selection works in source-canvas coordinates so the editor stores the same
        // pixel rectangles the runtime later scales from 1536x1024.
        activeElement = HitTestElement(viewModel, canvasPoint);
        viewModel.Select(activeElement);

        if (activeElement is null)
        {
            return;
        }

        activeStartBounds = activeElement.Bounds;
        dragStartCanvasPoint = canvasPoint;
        dragMode = IsInResizeHandle(activeElement, e.GetPosition(this), canvasRect)
            ? DragMode.Resize
            : DragMode.Move;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (activeElement is null || dragMode == DragMode.None)
        {
            return;
        }

        var canvasRect = GetCanvasRect(Bounds.Size);
        var canvasPoint = ToCanvasPoint(e.GetPosition(this), canvasRect);
        int deltaX = (int)Math.Round(canvasPoint.X - dragStartCanvasPoint.X);
        int deltaY = (int)Math.Round(canvasPoint.Y - dragStartCanvasPoint.Y);

        var proposed = dragMode == DragMode.Move
            ? activeStartBounds with { X = activeStartBounds.X + deltaX, Y = activeStartBounds.Y + deltaY }
            : activeStartBounds with
            {
                Width = Math.Max(1, activeStartBounds.Width + deltaX),
                Height = Math.Max(1, activeStartBounds.Height + deltaY)
            };

        // Drag and resize edits are clamped before they touch the document, keeping
        // invalid off-canvas rectangles out of exported JSON.
        ApplyRect(activeElement, EditorCoordinateService.ClampToCanvas(proposed, CanvasWidth, CanvasHeight));
        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (dragMode != DragMode.None)
        {
            e.Pointer.Capture(null);
            dragMode = DragMode.None;
            activeElement = null;

            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.ValidateDraft();
            }
        }
    }

    private static void ApplyRect(LayoutElementViewModel element, MenuLayoutRect rect)
    {
        element.X = rect.X;
        element.Y = rect.Y;
        element.Width = rect.Width;
        element.Height = rect.Height;
        element.RefreshBounds();
    }

    private static LayoutElementViewModel? HitTestElement(MainWindowViewModel viewModel, Point canvasPoint)
    {
        for (int index = viewModel.Elements.Count - 1; index >= 0; index--)
        {
            var element = viewModel.Elements[index];
            if (new Rect(element.X, element.Y, element.Width, element.Height).Contains(canvasPoint))
            {
                return element;
            }
        }

        return null;
    }

    private static bool IsInResizeHandle(LayoutElementViewModel element, Point screenPoint, Rect canvasRect)
    {
        return GetResizeHandleRect(ToScreenRect(element.Bounds, canvasRect)).Contains(screenPoint);
    }

    private static Rect GetCanvasRect(Size availableSize)
    {
        double scale = Math.Min(availableSize.Width / CanvasWidth, availableSize.Height / CanvasHeight);
        if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
        {
            scale = 1;
        }

        double width = CanvasWidth * scale;
        double height = CanvasHeight * scale;
        return new Rect(
            (availableSize.Width - width) / 2,
            (availableSize.Height - height) / 2,
            width,
            height);
    }

    private static Rect ToScreenRect(MenuLayoutRect sourceRect, Rect canvasRect)
    {
        double scaleX = canvasRect.Width / CanvasWidth;
        double scaleY = canvasRect.Height / CanvasHeight;
        return new Rect(
            canvasRect.X + sourceRect.X * scaleX,
            canvasRect.Y + sourceRect.Y * scaleY,
            sourceRect.Width * scaleX,
            sourceRect.Height * scaleY);
    }

    private static Point ToCanvasPoint(Point screenPoint, Rect canvasRect)
    {
        double scaleX = CanvasWidth / canvasRect.Width;
        double scaleY = CanvasHeight / canvasRect.Height;
        return new Point(
            (screenPoint.X - canvasRect.X) * scaleX,
            (screenPoint.Y - canvasRect.Y) * scaleY);
    }

    private static Rect GetResizeHandleRect(Rect elementRect)
    {
        return new Rect(
            elementRect.Right - ResizeHandleSize,
            elementRect.Bottom - ResizeHandleSize,
            ResizeHandleSize,
            ResizeHandleSize);
    }

    private enum DragMode
    {
        None,
        Move,
        Resize
    }
}
