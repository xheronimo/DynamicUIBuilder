
// ============================================================================
// ARCHIVO 8: Conversion/GridLengthConverter.cs
// ============================================================================
using System;
using System.Globalization;
using Avalonia.Controls;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para GridLength (columnas y filas de Grid)
    /// </summary>
    public class GridLengthConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(GridLength);

        public object Convert(string value, Type targetType)
        {
            value = value.Trim();

            if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
                return GridLength.Auto;

            if (value.EndsWith("*"))
            {
                var starValue = value.TrimEnd('*');
                var multiplier = string.IsNullOrEmpty(starValue) 
                    ? 1.0 
                    : double.Parse(starValue, CultureInfo.InvariantCulture);
                return new GridLength(multiplier, GridUnitType.Star);
            }

            return new GridLength(double.Parse(value, CultureInfo.InvariantCulture));
        }
    }
}