// ============================================================================
// ARCHIVO 5: Plugins/Advanced/DynamicUIBuilderExtensions.cs
// ============================================================================
using System.Collections.Generic;
using DynamicUI.Setters;
using DynamicUI.Validation;
using DynamicUI.Conversion;

namespace DynamicUI.Plugins.Advanced
{
    /// <summary>
    /// Extensiones para DynamicUIBuilder para soportar unregister
    /// </summary>
    public static class DynamicUIBuilderExtensions
    {
        private static readonly Dictionary<DynamicUIBuilder, List<IPropertySetter>> _setters = new();
        private static readonly Dictionary<DynamicUIBuilder, List<IPropertyValidator>> _validators = new();
        private static readonly Dictionary<DynamicUIBuilder, List<IValueConverter>> _converters = new();

        public static void TrackSetter(this DynamicUIBuilder builder, IPropertySetter setter)
        {
            if (!_setters.ContainsKey(builder))
                _setters[builder] = new List<IPropertySetter>();
            _setters[builder].Add(setter);
        }

        public static void UnregisterSetter(this DynamicUIBuilder builder, IPropertySetter setter)
        {
            if (_setters.TryGetValue(builder, out var list))
            {
                list.Remove(setter);
            }
            // Nota: Requiere acceso a la lista interna de setters en DynamicUIBuilder
        }

        public static void UnregisterValidator(this DynamicUIBuilder builder, IPropertyValidator validator)
        {
            if (_validators.TryGetValue(builder, out var list))
            {
                list.Remove(validator);
            }
        }

        public static void UnregisterConverter(this DynamicUIBuilder builder, IValueConverter converter)
        {
            if (_converters.TryGetValue(builder, out var list))
            {
                list.Remove(converter);
            }
        }
    }
}