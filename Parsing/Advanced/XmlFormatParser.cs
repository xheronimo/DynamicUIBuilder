
// ============================================================================
// ARCHIVO 5: Parsing/Advanced/XmlFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Parser para archivos XML
    /// </summary>
    public class XmlFormatParser : IFormatParser
    {
        public bool CanParse(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".xml", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<ControlDescriptor> Parse(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            if (root == null)
                throw new InvalidOperationException("El archivo XML no tiene elemento raíz");

            // Parsear defaults globales
            var globalDefaults = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var defaultsElement = root.Element("Defaults");
            
            if (defaultsElement != null)
            {
                foreach (var controlDefault in defaultsElement.Elements())
                {
                    var controlType = controlDefault.Name.LocalName;
                    globalDefaults[controlType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var attr in controlDefault.Attributes())
                    {
                        globalDefaults[controlType][attr.Name.LocalName] = attr.Value;
                    }
                }
            }

            // Parsear controles
            var controlsElement = root.Element("Controls");
            if (controlsElement != null)
            {
                foreach (var controlElement in controlsElement.Elements("Control"))
                {
                    yield return ParseControl(controlElement, globalDefaults);
                }
            }
        }

        private ControlDescriptor ParseControl(XElement element, Dictionary<string, Dictionary<string, string>> globalDefaults)
        {
            var type = element.Attribute("Type")?.Value ?? element.Attribute("type")?.Value;
            if (string.IsNullOrEmpty(type))
                throw new InvalidOperationException("Control sin atributo Type");

            var groupName = element.Attribute("Group")?.Value ?? element.Attribute("group")?.Value;
            var properties = new List<PropertyEntry>();

            // Aplicar defaults globales
            if (globalDefaults.TryGetValue(type, out var defaults))
            {
                foreach (var kv in defaults)
                {
                    properties.Add(new PropertyEntry(kv.Key, kv.Value));
                }
            }

            // Parsear atributos como propiedades
            foreach (var attr in element.Attributes())
            {
                if (attr.Name.LocalName.Equals("Type", StringComparison.OrdinalIgnoreCase) ||
                    attr.Name.LocalName.Equals("Group", StringComparison.OrdinalIgnoreCase))
                    continue;

                var existingIndex = properties.FindIndex(p => p.Name.Equals(attr.Name.LocalName, StringComparison.OrdinalIgnoreCase));
                if (existingIndex >= 0)
                    properties[existingIndex] = new PropertyEntry(attr.Name.LocalName, attr.Value);
                else
                    properties.Add(new PropertyEntry(attr.Name.LocalName, attr.Value));
            }

            // Parsear elementos hijos como propiedades
            foreach (var prop in element.Elements())
            {
                var propName = prop.Name.LocalName;
                var propValue = prop.Value;

                var existingIndex = properties.FindIndex(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (existingIndex >= 0)
                    properties[existingIndex] = new PropertyEntry(propName, propValue);
                else
                    properties.Add(new PropertyEntry(propName, propValue));
            }

            return new ControlDescriptor(type, properties, groupName);
        }
    }
}
