// ============================================================================
// ARCHIVO 1: Conversion/IValueConverter.cs
// ============================================================================
using System;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Interfaz para conversores de valores de string a tipos específicos
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Determina si este converter puede convertir al tipo especificado
        /// </summary>
        bool CanConvert(Type targetType);

        /// <summary>
        /// Convierte un string al tipo especificado
        /// </summary>
        object Convert(string value, Type targetType);
    }
}