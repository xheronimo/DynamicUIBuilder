
// ============================================================================
// ARCHIVO 9: Conversion/FontWeightConverter.cs
// ============================================================================
using System;
using Avalonia.Media;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para FontWeight
    /// </summary>
    public class FontWeightConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(FontWeight);

        public object Convert(string value, Type targetType)
        {
            return value.ToLowerInvariant() switch
            {
                "thin" => FontWeight.Thin,
                "extralight" => FontWeight.ExtraLight,
                "ultralight" => FontWeight.ExtraLight,
                "light" => FontWeight.Light,
                "normal" => FontWeight.Normal,
                "regular" => FontWeight.Normal,
                "medium" => FontWeight.Medium,
                "semibold" => FontWeight.SemiBold,
                "demibold" => FontWeight.SemiBold,
                "bold" => FontWeight.Bold,
                "extrabold" => FontWeight.ExtraBold,
                "ultrabold" => FontWeight.ExtraBold,
                "black" => FontWeight.Black,
                "heavy" => FontWeight.Black,
                _ => int.TryParse(value, out var weight) 
                    ? FontWeight.FromOpenTypeWeight(weight) 
                    : FontWeight.Normal
            };
        }
    }
}