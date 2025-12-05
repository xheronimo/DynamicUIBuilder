
// ============================================================================
// ARCHIVO 6: Validation/FilePathValidator.cs
// ============================================================================
using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;

namespace DynamicUI.Validation
{
    /// <summary>
    /// Validador de rutas de archivo
    /// </summary>
    public class FilePathValidator : IPropertyValidator
    {
        private readonly string[] _imageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico", ".svg" };
        private readonly string[] _dangerousPatterns = { "..", "//", "\\\\", "<", ">", "|", ":" };

        public bool CanValidate(Type controlType, string propertyName)
        {
            return (propertyName.Equals("Source", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Imagen", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("FilePath", StringComparison.OrdinalIgnoreCase)) 
                   && (controlType == typeof(Image) || controlType.Name.Contains("Image"));
        }

        public ValidationResult Validate(Control control, string propertyName, string value)
        {
            var result = ValidationResult.Success();

            if (string.IsNullOrWhiteSpace(value))
                return result;

            // Validar caracteres peligrosos
            foreach (var pattern in _dangerousPatterns)
            {
                if (value.Contains(pattern))
                {
                    result.AddError($"La ruta contiene el patrón peligroso '{pattern}'");
                }
            }

            // Validar que sea una ruta válida
            try
            {
                var fileName = Path.GetFileName(value);
                if (string.IsNullOrEmpty(fileName))
                {
                    result.AddError("Ruta de archivo inválida");
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ruta de archivo inválida: {ex.Message}");
                return result;
            }

            // Validar extensión para imágenes
            if (control is Image)
            {
                var extension = Path.GetExtension(value).ToLowerInvariant();
                
                if (!string.IsNullOrEmpty(extension) && !_imageExtensions.Contains(extension))
                {
                    result.AddWarning($"Extensión '{extension}' puede no ser compatible con controles de imagen");
                }
            }

            // Validar que el archivo existe (advertencia, no error)
            if (!File.Exists(value))
            {
                // Solo advertir si parece una ruta local
                if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                    !value.StartsWith("avares://", StringComparison.OrdinalIgnoreCase))
                {
                    result.AddWarning($"El archivo '{value}' no existe");
                }
            }

            return result;
        }
    }
}