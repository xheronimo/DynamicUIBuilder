
// ============================================================================
// EJEMPLO DE USO
// ============================================================================
/*
using System;
using Avalonia;
using Avalonia.Media;
using DynamicUI.Conversion;

public class TypeConverterExample
{
    public static void Main()
    {
        // 1. Crear motor de conversión
        var engine = new TypeConversionEngine();
        
        // 2. Convertir tipos primitivos
        var width = engine.ConvertValue("500", typeof(double));
        Console.WriteLine($"Width: {width} ({width.GetType().Name})");
        
        var isEnabled = engine.ConvertValue("true", typeof(bool));
        Console.WriteLine($"IsEnabled: {isEnabled}");
        
        // 3. Convertir Thickness
        var margin = engine.ConvertValue("10,20,10,20", typeof(Thickness));
        Console.WriteLine($"Margin: {margin}");
        
        // 4. Convertir CornerRadius
        var corners = engine.ConvertValue("5", typeof(CornerRadius));
        Console.WriteLine($"CornerRadius: {corners}");
        
        // 5. Convertir Brush
        var solidBrush = engine.ConvertValue("#FF0000", typeof(IBrush));
        Console.WriteLine($"Brush: {solidBrush}");
        
        var gradientBrush = engine.ConvertValue("LinearGradient:Red,Blue,90", typeof(IBrush));
        Console.WriteLine($"Gradient: {gradientBrush}");
        
        // 6. Convertir GridLength
        var colWidth = engine.ConvertValue("2*", typeof(GridLength));
        Console.WriteLine($"GridLength: {colWidth}");
        
        var autoWidth = engine.ConvertValue("Auto", typeof(GridLength));
        Console.WriteLine($"GridLength Auto: {autoWidth}");
        
        // 7. Convertir FontWeight
        var fontWeight = engine.ConvertValue("Bold", typeof(FontWeight));
        Console.WriteLine($"FontWeight: {fontWeight}");
        
        // 8. Registrar converter personalizado
        engine.RegisterConverter(new CustomConverter());
        
        // 9. Ver todos los converters
        Console.WriteLine($"\nConverters registrados: {engine.GetConverters().Count}");
    }
}

// Ejemplo de converter personalizado
public class CustomConverter : IValueConverter
{
    public bool CanConvert(Type targetType) => targetType == typeof(Point);

    public object Convert(string value, Type targetType)
    {
        var parts = value.Split(',');
        return new Point(
            double.Parse(parts[0]),
            double.Parse(parts[1])
        );
    }
}
*/