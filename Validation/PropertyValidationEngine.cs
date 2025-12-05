
// ============================================================================
// ARCHIVO 3: Validation/PropertyValidationEngine.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Motor de validación que coordina múltiples validadores
    /// </summary>
    public class PropertyValidationEngine
    {
        private readonly List<IPropertyValidator> _validators = new();

        public PropertyValidationEngine()
        {
            // Registrar validadores por defecto
            RegisterDefaultValidators();
        }

        private void RegisterDefaultValidators()
        {
            _validators.Add(new NumericRangeValidator());
            _validators.Add(new StringLengthValidator());
            _validators.Add(new FilePathValidator());
            _validators.Add(new SecurityValidator());
            _validators.Add(new DataContextValidator());
        }

        /// <summary>
        /// Registra un validador personalizado
        /// </summary>
        public void RegisterValidator(IPropertyValidator validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            
            _validators.Insert(0, validator); // Insertar al inicio para dar prioridad
        }

        /// <summary>
        /// Elimina un validador
        /// </summary>
        public void RemoveValidator(IPropertyValidator validator)
        {
            _validators.Remove(validator);
        }

        /// <summary>
        /// Valida una propiedad con todos los validadores aplicables
        /// </summary>
        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            // Ejecutar todos los validadores aplicables
            foreach (var validator in _validators.Where(v => v.CanValidate(control.GetType(), propertyName)))
            {
                try
                {
                    var validationResult = validator.Validate(control, propertyName, value);
                    result.Merge(validationResult);
                }
                catch (Exception ex)
                {
                    result.AddError($"Error en validador {validator.GetType().Name}: {ex.Message}");
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene todos los validadores registrados
        /// </summary>
        public IReadOnlyList<IPropertyValidator> GetValidators() => _validators.AsReadOnly();
    }
}