
// ============================================================================
// ARCHIVO 2: Parsing/Advanced/MultiFormatParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using DynamicUI.Parsing;

namespace DynamicUI.Parsing.Advanced
{
    /// <summary>
    /// Factory que selecciona el parser apropiado según la extensión
    /// </summary>
    public class MultiFormatParser
    {
        private readonly List<IFormatParser> _parsers = new();

        public MultiFormatParser()
        {
            RegisterDefaultParsers();
        }

        private void RegisterDefaultParsers()
        {
            _parsers.Add(new TextFormatParser());
            _parsers.Add(new JsonFormatParser());
            _parsers.Add(new XmlFormatParser());
            _parsers.Add(new YamlFormatParser());
        }

        /// <summary>
        /// Registra un parser personalizado
        /// </summary>
        public void RegisterParser(IFormatParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            
            _parsers.Insert(0, parser); // Insertar al inicio para dar prioridad
        }

        /// <summary>
        /// Elimina un parser
        /// </summary>
        public void RemoveParser(IFormatParser parser)
        {
            _parsers.Remove(parser);
        }

        /// <summary>
        /// Parsea un archivo usando el parser apropiado
        /// </summary>
        public IEnumerable<ControlDescriptor> ParseFile(string filePath)
        {
            foreach (var parser in _parsers)
            {
                if (parser.CanParse(filePath))
                {
                    var descriptors = parser.Parse(filePath);

                    foreach (var d in descriptors)
                    {
                        d.SourceFile = filePath;
                    }

                    return descriptors;
                }
            }

            throw new NotSupportedException($"No hay parser disponible para el archivo: {filePath}");
        }


        /// <summary>
        /// Obtiene todos los parsers registrados
        /// </summary>
        public IReadOnlyList<IFormatParser> GetParsers() => _parsers.AsReadOnly();
    }
}