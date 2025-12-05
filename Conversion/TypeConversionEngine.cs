
// ============================================================================
// ARCHIVO 2: Conversion/TypeConversionEngine.cs
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Motor de conversión de tipos con cache y múltiples conversores
    /// </summary>
    public class TypeConversionEngine
    {
        private readonly List<IValueConverter> _converters = new();
        private readonly ConcurrentDictionary<Type, IValueConverter> _converterCache = new();

        public TypeConversionEngine()
        {
            RegisterDefaultConverters();
        }

        private void RegisterDefaultConverters()
        {
            RegisterConverter(new PrimitiveConverter());
            RegisterConverter(new ThicknessConverter());
            RegisterConverter(new CornerRadiusConverter());
            RegisterConverter(new BrushConverter());
            RegisterConverter(new ColorConverter());
            RegisterConverter(new GridLengthConverter());
            RegisterConverter(new FontWeightConverter());
            RegisterConverter(new EnumConverter());
        }

        /// <summary>
        /// Registra un converter personalizado
        /// </summary>
        public void RegisterConverter(IValueConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            
            _converters.Insert(0, converter); // Insertar al inicio para dar prioridad
            _converterCache.Clear(); // Invalidar cache
        }

        /// <summary>
        /// Elimina un converter
        /// </summary>
        public void RemoveConverter(IValueConverter converter)
        {
            _converters.Remove(converter);
            _converterCache.Clear();
        }

        /// <summary>
        /// Convierte un valor string al tipo especificado
        /// </summary>
        public object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return GetDefaultValue(targetType);

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Buscar en cache
            if (_converterCache.TryGetValue(underlyingType, out var cachedConverter))
            {
                return cachedConverter.Convert(value, underlyingType);
            }

            // Buscar converter apropiado
            foreach (var converter in _converters)
            {
                if (converter.CanConvert(underlyingType))
                {
                    _converterCache.TryAdd(underlyingType, converter);
                    return converter.Convert(value, underlyingType);
                }
            }

            // Fallback a TypeDescriptor
            try
            {
                var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(underlyingType);
                if (typeConverter.CanConvertFrom(typeof(string)))
                    return typeConverter.ConvertFromInvariantString(value);
            }
            catch { }

            throw new InvalidOperationException($"No se pudo convertir '{value}' a tipo {targetType.Name}");
        }

        private object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Obtiene todos los converters registrados
        /// </summary>
        public IReadOnlyList<IValueConverter> GetConverters() => _converters.AsReadOnly();
    }
}