// ============================================================================
// ARCHIVO 1: Setters/IPropertySetter.cs
// ============================================================================
using Avalonia.Controls;

namespace DynamicUI.Setters
{
    /// <summary>
    /// Interfaz para setters de propiedades
    /// </summary>
    public interface IPropertySetter
    {
        /// <summary>
        /// Determina si este setter puede manejar la propiedad especificada
        /// </summary>
        bool CanHandle(Control control, string propertyName);

        /// <summary>
        /// Aplica el valor a la propiedad del control
        /// </summary>
        /// <param name="control">Control al que aplicar la propiedad</param>
        /// <param name="propertyName">Nombre de la propiedad</param>
        /// <param name="value">Valor como string</param>
        /// <param name="error">Mensaje de error si falla</param>
        /// <returns>True si se aplicó correctamente</returns>
        bool Apply(Control control, string propertyName, string value, out string error);
    }
}