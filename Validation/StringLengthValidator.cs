
// ============================================================================
// ARCHIVO 5: Validation/StringLengthValidator.cs
// ============================================================================
using System;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador de longitud de strings
    /// </summary>
    public class StringLengthValidator : IPropertyValidator
    {
        private readonly int _maxLength;
        private readonly int _warningThreshold;

        public StringLengthValidator(int maxLength = 10000, int warningThreshold = 1000)
        {
            _maxLength = maxLength;
            _warningThreshold = warningThreshold;
        }

        public bool CanValidate(Type controlType, string propertyName)
        {
            return propertyName.Equals("Text", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Content", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Header", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Title", StringComparison.OrdinalIgnoreCase);
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrEmpty(value))
                return result;

            if (value.Length > _maxLength)
            {
                result.AddError($"El texto excede la longitud máxima de {_maxLength} caracteres (actual: {value.Length})");
            }

            if (value.Length > _warningThreshold)
            {
                result.AddWarning($"Texto muy largo ({value.Length} caracteres), puede afectar el rendimiento");
            }

            // Validar caracteres problemáticos
            if (value.Contains("\0"))
            {
                result.AddError("El texto contiene caracteres nulos no permitidos");
            }

            return result;
        }
    }
}
