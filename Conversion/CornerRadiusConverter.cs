
// ============================================================================
// ARCHIVO 5: Conversion/CornerRadiusConverter.cs
// ============================================================================
using System;
using System.Globalization;
using Avalonia;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para CornerRadius (bordes redondeados)
    /// </summary>
    public class CornerRadiusConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(CornerRadius);

        public object Convert(string value, Type targetType)
        {
            var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length switch
            {
                1 => new CornerRadius(double.Parse(parts[0], CultureInfo.InvariantCulture)),
                
                4 => new CornerRadius(
                    double.Parse(parts[0], CultureInfo.InvariantCulture),
                    double.Parse(parts[1], CultureInfo.InvariantCulture),
                    double.Parse(parts[2], CultureInfo.InvariantCulture),
                    double.Parse(parts[3], CultureInfo.InvariantCulture)),
                
                _ => throw new FormatException($"Formato de CornerRadius inválido: {value}. Use: 'uniforme' o 'arr-izq,arr-der,aba-der,aba-izq'")
            };
        }
    }
}