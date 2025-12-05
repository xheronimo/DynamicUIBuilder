
// ============================================================================
// ARCHIVO 6: Conversion/BrushConverter.cs
// ============================================================================
using System;
using Avalonia;
using Avalonia.Media;

namespace DynamicUI.Conversion
{
    /// <summary>
    /// Converter para Brushes (colores sólidos y gradientes)
    /// </summary>
    public class BrushConverter : IValueConverter
    {
        public bool CanConvert(Type targetType) => typeof(IBrush).IsAssignableFrom(targetType);

        public object Convert(string value, Type targetType)
        {
            // Colores sólidos con #
            if (value.StartsWith("#"))
                return new SolidColorBrush(Color.Parse(value));

            // Nombres de colores
            if (TryParseColorName(value, out var color))
                return new SolidColorBrush(color);

            // Gradientes lineales: "LinearGradient:Red,Blue" o "LinearGradient:Red,Blue,90"
            if (value.StartsWith("LinearGradient:", StringComparison.OrdinalIgnoreCase))
            {
                return ParseLinearGradient(value.Substring(15));
            }

            // Gradientes radiales: "RadialGradient:Red,Blue"
            if (value.StartsWith("RadialGradient:", StringComparison.OrdinalIgnoreCase))
            {
                return ParseRadialGradient(value.Substring(15));
            }

            throw new FormatException($"Formato de Brush inválido: {value}");
        }

        private bool TryParseColorName(string name, out Color color)
        {
            try
            {
                color = Color.Parse(name);
                return true;
            }
            catch
            {
                color = default;
                return false;
            }
        }

        private LinearGradientBrush ParseLinearGradient(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 2)
                throw new FormatException("LinearGradient requiere al menos 2 colores");

            double angle = 0;
            int colorCount = parts.Length;

            // Verificar si el último valor es un ángulo
            if (parts.Length > 2 && double.TryParse(parts[parts.Length - 1], out var parsedAngle))
            {
                angle = parsedAngle;
                colorCount = parts.Length - 1;
            }

            // Convertir ángulo a puntos
            var (startPoint, endPoint) = AngleToPoints(angle);

            var brush = new LinearGradientBrush
            {
                StartPoint = startPoint,
                EndPoint = endPoint
            };

            for (int i = 0; i < colorCount; i++)
            {
                var offset = i / (double)(colorCount - 1);
                brush.GradientStops.Add(new GradientStop(Color.Parse(parts[i].Trim()), offset));
            }

            return brush;
        }

        private RadialGradientBrush ParseRadialGradient(string value)
        {
            var colors = value.Split(',');
            if (colors.Length < 2)
                throw new FormatException("RadialGradient requiere al menos 2 colores");

            var brush = new RadialGradientBrush();

            for (int i = 0; i < colors.Length; i++)
            {
                var offset = i / (double)(colors.Length - 1);
                brush.GradientStops.Add(new GradientStop(Color.Parse(colors[i].Trim()), offset));
            }

            return brush;
        }

        private (RelativePoint start, RelativePoint end) AngleToPoints(double angle)
        {
            // Convertir ángulo a radianes
            var rad = angle * Math.PI / 180;
            
            // Calcular puntos basados en el ángulo
            var x = Math.Cos(rad);
            var y = Math.Sin(rad);

            return (
                new RelativePoint(0.5 - x * 0.5, 0.5 - y * 0.5, RelativeUnit.Relative),
                new RelativePoint(0.5 + x * 0.5, 0.5 + y * 0.5, RelativeUnit.Relative)
            );
        }
    }
}