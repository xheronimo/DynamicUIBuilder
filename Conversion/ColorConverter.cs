
// ============================================================================
// ARCHIVO 7: Conversion/ColorConverter.cs
// ============================================================================
using System;
using Avalonia.Media;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para Color
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(Color);

        public object Convert(string value, Type targetType)
        {
            return Color.Parse(value);
        }
    }
}
