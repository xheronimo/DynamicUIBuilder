
// ============================================================================
// ARCHIVO 9: Validation/EnumValidator.cs
// ============================================================================
using System;
using System.Linq;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador para valores de enumeración
    /// </summary>
    public class EnumValidator : IPropertyValidator
    {
        public bool CanValidate(Type controlType, string propertyName)
        {
            // Validar propiedades que típicamente son enums
            return propertyName.EndsWith("Alignment", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Orientation", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Visibility", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Dock", StringComparison.OrdinalIgnoreCase);
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            // Obtener el tipo de la propiedad
            var propInfo = control.GetType().GetProperty(propertyName, 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.IgnoreCase);

            if (propInfo != null && propInfo.PropertyType.IsEnum)
            {
                var enumValues = Enum.GetNames(propInfo.PropertyType);
                
                if (!enumValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                {
                    result.AddError($"Valor '{value}' no válido para {propertyName}. Valores permitidos: {string.Join(", ", enumValues)}");
                }
            }

            return result;
        }
    }
}