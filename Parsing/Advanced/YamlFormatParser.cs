
// ============================================================================
// ARCHIVO 6: Parsing/Advanced/YamlFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Parser simple para YAML (sin dependencias externas)
    /// Para producción, considerar usar YamlDotNet
    /// </summary>
    public class YamlFormatParser : IFormatParser
    {
        public bool CanParse(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext == ".yaml" || ext == ".yml";
        }

        public IEnumerable<ControlDescriptor> Parse(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var globalDefaults = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var inDefaults = false;
            var inControls = false;
            var currentControlType = "";
            var currentControl = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var currentGroup = "";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Ignorar líneas vacías y comentarios
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var indent = line.TakeWhile(char.IsWhiteSpace).Count();

                // Sección defaults
                if (trimmed.StartsWith("defaults:", StringComparison.OrdinalIgnoreCase))
                {
                    inDefaults = true;
                    inControls = false;
                    continue;
                }

                // Sección controls
                if (trimmed.StartsWith("controls:", StringComparison.OrdinalIgnoreCase))
                {
                    inControls = true;
                    inDefaults = false;
                    continue;
                }

                // Parsear defaults
                if (inDefaults && indent == 2 && trimmed.Contains(":") && !trimmed.Contains("- "))
                {
                    currentControlType = trimmed.TrimEnd(':');
                    globalDefaults[currentControlType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else if (inDefaults && indent == 4 && trimmed.Contains(":"))
                {
                    var parts = trimmed.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim().Trim('"');
                        globalDefaults[currentControlType][key] = value;
                    }
                }
                // Parsear controles
                else if (inControls && indent == 2 && trimmed.StartsWith("- type:", StringComparison.OrdinalIgnoreCase))
                {
                    // Guardar control anterior
                    if (!string.IsNullOrEmpty(currentControlType))
                    {
                        yield return CreateDescriptor(currentControlType, currentControl, currentGroup, globalDefaults);
                    }

                    currentControlType = trimmed.Substring(7).Trim().Trim('"');
                    currentControl = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    currentGroup = "";
                }
                else if (inControls && indent == 4 && trimmed.Contains(":"))
                {
                    var parts = trimmed.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim().Trim('"');

                        if (key.Equals("group", StringComparison.OrdinalIgnoreCase))
                            currentGroup = value;
                        else
                            currentControl[key] = value;
                    }
                }
            }

            // Guardar último control
            if (!string.IsNullOrEmpty(currentControlType))
            {
                yield return CreateDescriptor(currentControlType, currentControl, currentGroup, globalDefaults);
            }
        }

        private ControlDescriptor CreateDescriptor(
            string type, 
            Dictionary<string, string> properties, 
            string group, 
            Dictionary<string, Dictionary<string, string>> globalDefaults)
        {
            var props = new List<PropertyEntry>();

            // Aplicar defaults
            if (globalDefaults.TryGetValue(type, out var defaults))
            {
                foreach (var kv in defaults)
                {
                    props.Add(new PropertyEntry(kv.Key, kv.Value));
                }
            }

            // Aplicar propiedades específicas (sobreescribiendo defaults)
            foreach (var kv in properties)
            {
                var existingIndex = props.FindIndex(p => p.Name.Equals(kv.Key, StringComparison.OrdinalIgnoreCase));
                if (existingIndex >= 0)
                    props[existingIndex] = new PropertyEntry(kv.Key, kv.Value);
                else
                    props.Add(new PropertyEntry(kv.Key, kv.Value));
            }

            return new ControlDescriptor(type, props, group);
        }
    }
}