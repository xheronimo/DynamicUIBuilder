
// ============================================================================
// ARCHIVO 5: Plugins/SecurityValidationPlugin.cs (EJEMPLO)
// ============================================================================
using System;
using Avalonia.Controls;
using DynamicUI.Validation;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Ejemplo de plugin para validaciones adicionales de seguridad
    /// </summary>
    public class SecurityValidationPlugin : IUIPlugin
    {
        public string Name => "Security Validation";
        public string Version => "1.0.0";
        public string Description => "Agrega validaciones de seguridad adicionales";

        public void Initialize(PluginContext context)
        {
            context.RegisterValidator(new XSSValidator());
            context.RegisterValidator(new SQLInjectionValidator());
            context.Logger.LogInfo("SecurityValidationPlugin inicializado");
        }

        public void Shutdown() 
        { 
            // Cleanup
        }

        // Validador XSS
        private class XSSValidator : IPropertyValidator
        {
            public bool CanValidate(Type controlType, string propertyName)
            {
                return propertyName.Equals("Text", StringComparison.OrdinalIgnoreCase) ||
                       propertyName.Equals("Content", StringComparison.OrdinalIgnoreCase);
            }

            public ValidationResult Validate(Control control, string propertyName, string value)
            {
                var result = ValidationResult.Success();
                
                if (value.Contains("<script", StringComparison.OrdinalIgnoreCase))
                    result.AddError("Posible ataque XSS detectado");
                
                if (value.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
                    result.AddError("Código JavaScript no permitido");

                return result;
            }
        }

        // Validador SQL Injection
        private class SQLInjectionValidator : IPropertyValidator
        {
            private readonly string[] _sqlKeywords = { "DROP", "DELETE", "INSERT", "UPDATE", "SELECT", "EXEC", "UNION" };

            public bool CanValidate(Type controlType, string propertyName)
            {
                return propertyName.Contains("Text") || propertyName.Contains("Content");
            }

            public ValidationResult Validate(Control control, string propertyName, string value)
            {
                var result = ValidationResult.Success();
                var upper = value.ToUpperInvariant();

                foreach (var keyword in _sqlKeywords)
                {
                    if (upper.Contains(keyword))
                    {
                        result.AddWarning($"Palabra clave SQL detectada: {keyword}");
                        break;
                    }
                }

                return result;
            }
        }
    }
}