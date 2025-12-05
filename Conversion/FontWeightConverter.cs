
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
            var lowerValue = value.ToLowerInvariant();

            // Primero intentar nombres conocidos
            var namedWeight = lowerValue switch
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
                _ => (FontWeight?)null
            };

            if (namedWeight.HasValue)
                return namedWeight.Value;

            // Si es un número, mapear al FontWeight más cercano
            if (int.TryParse(value, out var weight))
            {
                return weight switch
                {
                    <= 100 => FontWeight.Thin,           // 100
                    <= 200 => FontWeight.ExtraLight,     // 200
                    <= 300 => FontWeight.Light,          // 300
                    <= 400 => FontWeight.Normal,         // 400
                    <= 500 => FontWeight.Medium,         // 500
                    <= 600 => FontWeight.SemiBold,       // 600
                    <= 700 => FontWeight.Bold,           // 700
                    <= 800 => FontWeight.ExtraBold,      // 800
                    _ => FontWeight.Black                // 900+
                };
            }

            // Valor por defecto
            return FontWeight.Normal;
        }
    }
}