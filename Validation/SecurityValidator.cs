
// ============================================================================
// ARCHIVO 7: Validation/SecurityValidator.cs
// ============================================================================
using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador de seguridad para detectar patrones peligrosos
    /// </summary>
    public class SecurityValidator : IPropertyValidator
    {
        private readonly HashSet<string> _dangerousPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            "<script",
            "</script>",
            "javascript:",
            "onclick=",
            "onerror=",
            "onload=",
            "eval(",
            "expression(",
            "vbscript:",
            "data:text/html"
        };

        private readonly string[] _sqlKeywords = 
        { 
            "DROP TABLE", "DROP DATABASE", "DELETE FROM", "TRUNCATE", 
            "EXEC ", "EXECUTE ", "xp_", "sp_executesql" 
        };

        public bool CanValidate(Type controlType, string propertyName)
        {
            // Validar todas las propiedades de texto
            return true;
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrEmpty(value))
                return result;

            // Detectar XSS y scripts maliciosos
            foreach (var pattern in _dangerousPatterns)
            {
                if (value.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    result.AddError($"Patrón peligroso detectado: {pattern}");
                }
            }

            // Detectar SQL Injection
            var upperValue = value.ToUpperInvariant();
            foreach (var keyword in _sqlKeywords)
            {
                if (upperValue.Contains(keyword))
                {
                    result.AddWarning($"Patrón SQL sospechoso detectado: {keyword}");
                }
            }

            // Detectar exceso de caracteres especiales (posible ataque)
            int specialCharCount = 0;
            foreach (char c in value)
            {
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                {
                    specialCharCount++;
                }
            }

            if (value.Length > 0 && specialCharCount > value.Length * 0.5)
            {
                result.AddWarning("Proporción inusualmente alta de caracteres especiales");
            }

            return result;
        }
    }
}