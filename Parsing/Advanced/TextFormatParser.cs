// ============================================================================
// ARCHIVO 3: Parsing/Advanced/TextFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Parser para archivos de texto (.txt, .ui, .dui)
    /// Usa TextParser que soporta carga de archivos externos
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
            // Usar TextParser que tiene soporte completo de:
            // - Defaults (Control=)
            // - Grupos (Grupo=)
            // - Carga de propiedades externas (PropertiesFile=)
            // - Carga de controles hijos (Children=)
            return TextParser.ParseFile(filePath);
        }
    }
}