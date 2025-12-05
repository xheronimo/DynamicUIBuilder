// ============================================================================
// ARCHIVO 1: Parsing/Advanced/IFormatParser.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Interfaz para parsers de diferentes formatos
    /// </summary>
    public interface IFormatParser
    {
        /// <summary>
        /// Determina si este parser puede procesar el archivo
        /// </summary>
        bool CanParse(string filePath);
        
        /// <summary>
        /// Parsea el archivo y devuelve descriptores de controles
        /// </summary>
        IEnumerable<ControlDescriptor> Parse(string filePath);
    }
}