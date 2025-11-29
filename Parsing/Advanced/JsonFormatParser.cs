
// ============================================================================
// ARCHIVO 4: Parsing/Advanced/JsonFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Parser para archivos JSON
    /// </summary>
    public class JsonFormatParser : IFormatParser
    {
        public bool CanParse(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<ControlDescriptor> Parse(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Parsear defaults globales
            var globalDefaults = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            
            if (root.TryGetProperty("defaults", out var defaultsElement))
            {
                foreach (var defaultProp in defaultsElement.EnumerateObject())
                {
                    var controlType = defaultProp.Name;
                    globalDefaults[controlType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var prop in defaultProp.Value.EnumerateObject())
                    {
                        var value = prop.Value.ValueKind == JsonValueKind.String
                            ? prop.Value.GetString()
                            : prop.Value.ToString();
                        globalDefaults[controlType][prop.Name] = value;
                    }
                }
            }

            // Parsear controles
            if (root.TryGetProperty("controls", out var controlsElement))
            {
                foreach (var controlElement in controlsElement.EnumerateArray())
                {
                    yield return ParseControl(controlElement, globalDefaults);
                }
            }
        }

        private ControlDescriptor ParseControl(JsonElement element, Dictionary<string, Dictionary<string, string>> globalDefaults)
        {
            var type = element.GetProperty("type").GetString();
            var properties = new List<PropertyEntry>();
            string groupName = null;

            if (element.TryGetProperty("group", out var groupElement))
            {
                groupName = groupElement.GetString();
            }

            // Aplicar defaults globales
            if (globalDefaults.TryGetValue(type, out var defaults))
            {
                foreach (var kv in defaults)
                {
                    properties.Add(new PropertyEntry(kv.Key, kv.Value));
                }
            }

            // Parsear propiedades del control
            if (element.TryGetProperty("properties", out var propsElement))
            {
                foreach (var prop in propsElement.EnumerateObject())
                {
                    var value = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();

                    // Sobreescribir defaults si existe
                    var existingIndex = properties.FindIndex(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                    if (existingIndex >= 0)
                        properties[existingIndex] = new PropertyEntry(prop.Name, value);
                    else
                        properties.Add(new PropertyEntry(prop.Name, value));
                }
            }

            return new ControlDescriptor(type, properties, groupName);
        }
    }
}