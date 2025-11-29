
// ============================================================================
// ARCHIVO 4: Conversion/ThicknessConverter.cs
// ============================================================================
using System;
using System.Globalization;
using Avalonia;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para Thickness (márgenes, padding)
    /// </summary>
    public class ThicknessConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(Thickness);

        public object Convert(string value, Type targetType)
        {
            var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length switch
            {
                1 => new Thickness(double.Parse(parts[0], CultureInfo.InvariantCulture)),
                
                2 => new Thickness(
                    double.Parse(parts[0], CultureInfo.InvariantCulture),
                    double.Parse(parts[1], CultureInfo.InvariantCulture)),
                
                4 => new Thickness(
                    double.Parse(parts[0], CultureInfo.InvariantCulture),
                    double.Parse(parts[1], CultureInfo.InvariantCulture),
                    double.Parse(parts[2], CultureInfo.InvariantCulture),
                    double.Parse(parts[3], CultureInfo.InvariantCulture)),
                
                _ => throw new FormatException($"Formato de Thickness inválido: {value}. Use: 'uniforme' o 'horizontal,vertical' o 'izq,arr,der,aba'")
            };
        }
    }
}
