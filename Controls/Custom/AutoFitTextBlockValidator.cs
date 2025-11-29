// ============================================================================
// ARCHIVO 3: Controls/Custom/AutoFitTextBlockValidator.cs
// ============================================================================
using System;
using Avalonia.Controls;
using DynamicUI.Validation;

namespace DynamicUI.Controls.Custom
{
    /// <summary>
    /// Validador especializado para AutoFitTextBlock
    /// </summary>
    public class AutoFitTextBlockValidator : IPropertyValidator
    {
        public bool CanValidate(Type controlType, string propertyName)
        {
            if (!typeof(AutoFitTextBlock).IsAssignableFrom(controlType))
                return false;

            return propertyName.Equals("MinFontSize", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("MaxFontSize", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Text", StringComparison.OrdinalIgnoreCase);
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();
            var autoFit = control as AutoFitTextBlock;

            // Validar tamaños de fuente
            if (propertyName.Equals("MinFontSize", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("MaxFontSize", StringComparison.OrdinalIgnoreCase))
            {
                if (!double.TryParse(value, out var size))
                {
                    result.AddError($"{propertyName} debe ser un número válido");
                    return result;
                }

                if (size <= 0)
                {
                    result.AddError($"{propertyName} debe ser mayor que 0");
                    return result;
                }

                if (size > 500)
                {
                    result.AddWarning($"{propertyName} es muy grande ({size}), puede causar problemas de rendimiento");
                }

                // Validación cruzada
                if (propertyName.Equals("MinFontSize", StringComparison.OrdinalIgnoreCase))
                {
                    if (autoFit != null && size >= autoFit.MaxFontSize)
                    {
                        result.AddError($"MinFontSize ({size}) debe ser menor que MaxFontSize ({autoFit.MaxFontSize})");
                    }
                }
                else if (propertyName.Equals("MaxFontSize", StringComparison.OrdinalIgnoreCase))
                {
                    if (autoFit != null && size <= autoFit.MinFontSize)
                    {
                        result.AddError($"MaxFontSize ({size}) debe ser mayor que MinFontSize ({autoFit.MinFontSize})");
                    }
                }
            }

            // Validar texto
            if (propertyName.Equals("Text", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(value))
                {
                    result.AddWarning("AutoFitTextBlock sin texto no mostrará nada");
                }

                if (value.Length > 500)
                {
                    result.AddWarning($"Texto muy largo ({value.Length} caracteres) puede no ajustarse correctamente");
                }
            }

            return result;
        }
    }
}