
// ============================================================================
// ARCHIVO 2: Validation/IPropertyValidator.cs
// ============================================================================
using System;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Interfaz para validadores de propiedades
    /// </summary>
    public interface IPropertyValidator
    {
        /// <summary>
        /// Determina si este validador puede validar la propiedad especificada
        /// </summary>
        bool CanValidate(Type controlType, string propertyName);

        /// <summary>
        /// Valida el valor de una propiedad
        /// </summary>
        ValidationResult Validate(Control control, string propertyName, string value);
    }
}