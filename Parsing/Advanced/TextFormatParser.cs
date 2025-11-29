
// ============================================================================
// ARCHIVO 3: Parsing/Advanced/TextFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Parser original para archivos de texto
    /// </summary>
    public class TextFormatParser : IFormatParser
    {
        public bool CanParse(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext == ".txt" || ext == ".ui" || ext == ".dui";
        }

        public IEnumerable<ControlDescriptor> Parse(string filePath)
        {
            return FileParser.ParseFile(filePath);
        }
    }
}