
// ============================================================================
// ARCHIVO 3: Conversion/PrimitiveConverter.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para tipos primitivos
    /// </summary>
    public class PrimitiveConverter : IValueConverter
    {
        private static readonly HashSet<Type> _supportedTypes = new()
        {
            typeof(string), 
            typeof(int), 
            typeof(double), 
            typeof(float),
            typeof(bool), 
            typeof(long), 
            typeof(decimal), 
            typeof(byte),
            typeof(short),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(sbyte),
            typeof(char)
        };

        public bool CanConvert(Type targetType) => _supportedTypes.Contains(targetType);

        public object Convert(string value, Type targetType)
        {
            if (targetType == typeof(string)) 
                return value;
            
            if (targetType == typeof(bool)) 
                return ParseBool(value);
            
            if (targetType == typeof(int)) 
                return int.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(double)) 
                return double.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(float)) 
                return float.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(long)) 
                return long.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(decimal)) 
                return decimal.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(byte)) 
                return byte.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(short)) 
                return short.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(uint)) 
                return uint.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(ulong)) 
                return ulong.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(ushort)) 
                return ushort.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(sbyte)) 
                return sbyte.Parse(value, CultureInfo.InvariantCulture);
            
            if (targetType == typeof(char)) 
                return value.Length > 0 ? value[0] : '\0';

            throw new NotSupportedException($"Tipo primitivo {targetType.Name} no soportado");
        }

        private bool ParseBool(string value)
        {
            var lower = value.ToLowerInvariant().Trim();
            return lower == "true" || lower == "1" || lower == "yes" || lower == "si" || lower == "sí";
        }
    }
}