
// ============================================================================
// EJEMPLO DE USO
// ============================================================================
/*
using System;
using Avalonia.Controls;
using DynamicUI.Validation;

public class ValidationExample
{
    public static void Main()
    {
        // 1. Crear motor de validación
        var engine = new PropertyValidationEngine();
        
        // 2. Registrar validador personalizado (opcional)
        engine.RegisterValidator(new CustomValidator());
        
        // 3. Crear control para validar
        var textBlock = new TextBlock();
        
        // 4. Validar diferentes propiedades
        
        // Validar tamaño
        var result1 = engine.Validate(textBlock, "Width", "500");
        Console.WriteLine($"Width válido: {result1.IsValid}");
        
        // Validar tamaño inválido
        var result2 = engine.Validate(textBlock, "Width", "15000");
        Console.WriteLine($"Width inválido: {result2.IsValid}");
        foreach (var error in result2.Errors)
        {
            Console.WriteLine($"  Error: {error}");
        }
        
        // Validar texto con XSS
        var result3 = engine.Validate(textBlock, "Text", "<script>alert('xss')</script>");
        Console.WriteLine($"Texto con XSS: {result3.IsValid}");
        foreach (var error in result3.Errors)
        {
            Console.WriteLine($"  Error: {error}");
        }
        
        // Validar archivo
        var image = new Image();
        var result4 = engine.Validate(image, "Source", "image.png");
        Console.WriteLine($"Ruta de imagen: {result4.IsValid}");
        foreach (var warning in result4.Warnings)
        {
            Console.WriteLine($"  Advertencia: {warning}");
        }
        
        // 5. Ver todos los validadores registrados
        Console.WriteLine($"\nValidadores registrados: {engine.GetValidators().Count}");
    }
}

// Ejemplo de validador personalizado
public class CustomValidator : IPropertyValidator
{
    public bool CanValidate(Type controlType, string propertyName)
    {
        return propertyName.Equals("CustomProperty", StringComparison.OrdinalIgnoreCase);
    }

    public ValidationResult Validate(Control control, string propertyName, string value)
    {
        var result = ValidationResult.Success();
        
        if (value.Length < 5)
        {
            result.AddError("El valor debe tener al menos 5 caracteres");
        }
        
        return result;
    }
}
*/