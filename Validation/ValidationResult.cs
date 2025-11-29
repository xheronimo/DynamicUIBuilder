// ============================================================================
// ARCHIVO 1: Validation/ValidationResult.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Resultado de una validación de propiedad
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Crea un resultado de validación exitoso
        /// </summary>
        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        
        /// <summary>
        /// Crea un resultado de validación fallido con un error
        /// </summary>
        public static ValidationResult Failure(string error) => new ValidationResult 
        { 
            IsValid = false, 
            Errors = new List<string> { error } 
        };

        /// <summary>
        /// Agrega un error al resultado
        /// </summary>
        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }

        /// <summary>
        /// Agrega una advertencia al resultado
        /// </summary>
        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        /// <summary>
        /// Combina este resultado con otro
        /// </summary>
        public void Merge(ValidationResult other)
        {
            if (!other.IsValid)
            {
                IsValid = false;
            }
            Errors.AddRange(other.Errors);
            Warnings.AddRange(other.Warnings);
        }
    }
}