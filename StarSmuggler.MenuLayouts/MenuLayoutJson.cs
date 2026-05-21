using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarSmuggler.MenuLayouts;

public static class MenuLayoutJson
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    /// <summary>
    /// Central serializer options keep editor and runtime JSON property names deterministic.
    /// Version 1 intentionally uses PascalCase names to match the shared DTO contract.
    /// </summary>
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = null,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        options.Converters.Add(new MenuLayoutElementJsonConverter());
        return options;
    }

    public static string Serialize(MenuLayoutDocument document)
    {
        return JsonSerializer.Serialize(document, Options);
    }

    public static MenuLayoutDocument? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<MenuLayoutDocument>(json, Options);
    }

    private sealed class MenuLayoutElementJsonConverter : JsonConverter<MenuLayoutElement>
    {
        public override MenuLayoutElement? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            var root = jsonDocument.RootElement;

            string type = root.TryGetProperty(nameof(MenuLayoutElement.Type), out var typeProperty)
                ? typeProperty.GetString() ?? string.Empty
                : string.Empty;

            return type switch
            {
                MenuLayoutElementTypes.Text => ReadTextElement(root),
                MenuLayoutElementTypes.ButtonMask => ReadButtonMaskElement(root),
                _ => ReadUnknownElement(root, type)
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            MenuLayoutElement value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(MenuLayoutElement.Type), value.Type);
            writer.WriteString(nameof(MenuLayoutElement.Id), value.Id);
            writer.WriteNumber(nameof(MenuLayoutElement.X), value.X);
            writer.WriteNumber(nameof(MenuLayoutElement.Y), value.Y);
            writer.WriteNumber(nameof(MenuLayoutElement.Width), value.Width);
            writer.WriteNumber(nameof(MenuLayoutElement.Height), value.Height);

            if (value is TextElement textElement)
            {
                writer.WriteString(nameof(TextElement.Text), textElement.Text);
                writer.WriteString(nameof(TextElement.FontKey), textElement.FontKey);
                writer.WriteNumber(nameof(TextElement.FontScale), textElement.FontScale);
                writer.WriteString(nameof(TextElement.Color), textElement.Color);
                writer.WriteString(nameof(TextElement.HorizontalAlignment), textElement.HorizontalAlignment);
            }
            else if (value is ButtonMaskElement buttonMaskElement)
            {
                writer.WriteString(nameof(ButtonMaskElement.Action), buttonMaskElement.Action);
                if (buttonMaskElement.Label is not null)
                {
                    writer.WriteString(nameof(ButtonMaskElement.Label), buttonMaskElement.Label);
                }

                writer.WriteBoolean(nameof(ButtonMaskElement.Enabled), buttonMaskElement.Enabled);
            }

            writer.WriteEndObject();
        }

        private static TextElement ReadTextElement(JsonElement root)
        {
            var element = new TextElement();
            ReadBase(root, element);
            element.Text = ReadString(root, nameof(TextElement.Text));
            element.FontKey = ReadString(root, nameof(TextElement.FontKey));
            element.FontScale = ReadDouble(root, nameof(TextElement.FontScale), 0);
            element.Color = ReadString(root, nameof(TextElement.Color));
            element.HorizontalAlignment = ReadString(root, nameof(TextElement.HorizontalAlignment));
            return element;
        }

        private static ButtonMaskElement ReadButtonMaskElement(JsonElement root)
        {
            var element = new ButtonMaskElement();
            ReadBase(root, element);
            element.Action = ReadString(root, nameof(ButtonMaskElement.Action));
            element.Label = root.TryGetProperty(nameof(ButtonMaskElement.Label), out var label)
                ? label.GetString()
                : null;
            element.Enabled = !root.TryGetProperty(nameof(ButtonMaskElement.Enabled), out var enabled) ||
                enabled.ValueKind != JsonValueKind.False;
            return element;
        }

        private static MenuLayoutElement ReadUnknownElement(JsonElement root, string type)
        {
            var element = new MenuLayoutElement { Type = type };
            ReadBase(root, element);
            return element;
        }

        private static void ReadBase(JsonElement root, MenuLayoutElement element)
        {
            element.Type = ReadString(root, nameof(MenuLayoutElement.Type));
            element.Id = ReadString(root, nameof(MenuLayoutElement.Id));
            element.X = ReadInt(root, nameof(MenuLayoutElement.X), 0);
            element.Y = ReadInt(root, nameof(MenuLayoutElement.Y), 0);
            element.Width = ReadInt(root, nameof(MenuLayoutElement.Width), 0);
            element.Height = ReadInt(root, nameof(MenuLayoutElement.Height), 0);
        }

        private static string ReadString(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static int ReadInt(JsonElement root, string propertyName, int fallback)
        {
            return root.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
                ? value
                : fallback;
        }

        private static double ReadDouble(JsonElement root, string propertyName, double fallback)
        {
            return root.TryGetProperty(propertyName, out var property) && property.TryGetDouble(out var value)
                ? value
                : fallback;
        }
    }
}
