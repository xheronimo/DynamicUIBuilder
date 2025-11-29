
// ============================================================================
// ARCHIVO 8: Validation/DataContextValidator.cs
// ============================================================================
using System;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador para DataContext
    /// </summary>
    public class DataContextValidator : IPropertyValidator
    {
        public bool CanValidate(Type controlType, string propertyName)
        {
            return propertyName.Equals("DataContext", StringComparison.OrdinalIgnoreCase);
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrWhiteSpace(value))
            {
                result.AddWarning("DataContext vacío puede causar errores de binding");
            }

            return result;
        }
    }
}