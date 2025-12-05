
// ============================================================================
// ARCHIVO 4: Validation/NumericRangeValidator.cs
// ============================================================================
using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador de rangos numéricos para propiedades comunes
    /// </summary>
    public class NumericRangeValidator : IPropertyValidator
    {
        private readonly Dictionary<string, (double min, double max)> _ranges = new()
        {
            { "Width", (0, 10000) },
            { "Height", (0, 10000) },
            { "MinWidth", (0, 10000) },
            { "MinHeight", (0, 10000) },
            { "MaxWidth", (0, 10000) },
            { "MaxHeight", (0, 10000) },
            { "Opacity", (0, 1) },
            { "FontSize", (1, 1000) },
            { "X", (-10000, 10000) },
            { "Y", (-10000, 10000) },
            { "Left", (-10000, 10000) },
            { "Top", (-10000, 10000) },
            { "Right", (-10000, 10000) },
            { "Bottom", (-10000, 10000) }
        };

        public bool CanValidate(Type controlType, string propertyName)
        {
            return _ranges.ContainsKey(propertyName);
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            if (!double.TryParse(value, out var numValue))
                return ValidationResult.Failure($"'{value}' no es un número válido para {propertyName}");

            if (_ranges.TryGetValue(propertyName, out var range))
            {
                if (numValue < range.min || numValue > range.max)
                {
                    return ValidationResult.Failure(
                        $"{propertyName} debe estar entre {range.min} y {range.max} (valor: {numValue})");
                }

                // Advertencias para valores extremos
                if (propertyName is "Width" or "Height")
                {
                    if (numValue > 5000)
                    {
                        var result = ValidationResult.Success();
                        result.AddWarning($"{propertyName} muy grande ({numValue}), puede afectar el rendimiento");
                        return result;
                    }
                }
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// Agrega o actualiza un rango de validación
        /// </summary>
        public void SetRange(string propertyName, double min, double max)
        {
            _ranges[propertyName] = (min, max);
        }
    }
}
