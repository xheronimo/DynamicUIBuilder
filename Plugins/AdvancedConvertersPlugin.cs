
// ============================================================================
// ARCHIVO 6: Plugins/AdvancedConvertersPlugin.cs (EJEMPLO)
// ============================================================================
using System;
using DynamicUI.Conversion;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Plugin para conversores de tipos avanzados
    /// </summary>
    public class AdvancedConvertersPlugin : IUIPlugin
    {
        public string Name => "Advanced Converters";
        public string Version => "1.0.0";
        public string Description => "Conversores de tipos avanzados";

        public void Initialize(PluginContext context)
        {
            context.RegisterConverter(new UriConverter());
            context.RegisterConverter(new DateTimeConverter());
            context.RegisterConverter(new TimeSpanConverter());
            context.Logger.LogInfo("AdvancedConvertersPlugin inicializado");
        }

        public void Shutdown() { }

        // Converter para Uri
        private class UriConverter : IValueConverter
        {
            public bool CanConvert(Type targetType) => targetType == typeof(Uri);

            public object Convert(string value, Type targetType)
            {
                return new Uri(value, UriKind.RelativeOrAbsolute);
            }
        }

        // Converter para DateTime
        private class DateTimeConverter : IValueConverter
        {
            public bool CanConvert(Type targetType) => targetType == typeof(DateTime);

            public object Convert(string value, Type targetType)
            {
                return DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        // Converter para TimeSpan
        private class TimeSpanConverter : IValueConverter
        {
            public bool CanConvert(Type targetType) => targetType == typeof(TimeSpan);

            public object Convert(string value, Type targetType)
            {
                // Formatos soportados: "hh:mm:ss", "mm:ss", o milisegundos
                if (value.Contains(":"))
                    return TimeSpan.Parse(value);
                else
                    return TimeSpan.FromMilliseconds(double.Parse(value));
            }
        }
    }
}