
// ============================================================================
// ARCHIVO 10: Conversion/EnumConverter.cs
// ============================================================================
using System;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter genérico para enums
    /// </summary>
    public class EnumConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => targetType.IsEnum;

        public object Convert(string value, Type targetType)
        {
            return Enum.Parse(targetType, value, ignoreCase: true);
        }
    }
}
