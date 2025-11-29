
// ============================================================================
// ARCHIVO 3: Animation/AnimationSetter.cs
// ============================================================================
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using DynamicUI.Setters;
using DynamicUI.Logging;

namespace DynamicUI.Animation
{
    /// <summary>
    /// Setter que parsea y ejecuta animaciones
    /// </summary>
    public class AnimationSetter : IPropertySetter, IAsyncPropertySetter
    {
        private readonly AnimationEngine _engine;
        private readonly IUILogger _logger;

        public AnimationSetter(AnimationEngine engine, IUILogger logger)
        {
            _engine = engine;
            _logger = logger;
        }

        public bool CanHandle(Control control, string propertyName)
        {
            return propertyName.StartsWith("Animate:", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.StartsWith("Anim:", StringComparison.OrdinalIgnoreCase);
        }

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;

            try
            {
                var descriptor = ParseAnimationDescriptor(propertyName, value);
                
                // Ejecutar animación de forma asíncrona (fire and forget)
                _ = _engine.AnimateAsync(control, descriptor);
                
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _logger.LogError($"Error parseando animación: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> ApplyAsync(Control control, string propertyName, string value, CancellationToken ct)
        {
            try
            {
                var descriptor = ParseAnimationDescriptor(propertyName, value);
                await _engine.AnimateAsync(control, descriptor);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error aplicando animación: {ex.Message}", ex);
                return false;
            }
        }

        private AnimationDescriptor ParseAnimationDescriptor(string propertyName, string value)
        {
            // Formato: Animate:PropertyName=toValue,duration[,delay][,easing][,repeat][,autoReverse]
            // Ejemplo: Animate:Opacity=0,1000,0,EaseOut,3,true
            // Formato alternativo: Animate:PropertyName=fromValue->toValue,duration,...
            
            var parts = value.Split(',');
            if (parts.Length < 2)
                throw new FormatException("Formato de animación inválido. Mínimo: toValue,duration");

            var descriptor = new AnimationDescriptor
            {
                PropertyName = propertyName.Substring(propertyName.IndexOf(':') + 1)
            };

            // Parsear valor (puede ser "valor" o "from->to")
            var valuePart = parts[0].Trim();
            if (valuePart.Contains("->"))
            {
                var valueRange = valuePart.Split(new[] { "->" }, StringSplitOptions.None);
                descriptor.FromValue = ParseValue(valueRange[0].Trim());
                descriptor.ToValue = ParseValue(valueRange[1].Trim());
            }
            else
            {
                descriptor.ToValue = ParseValue(valuePart);
            }

            // Parsear duración (en milisegundos)
            if (int.TryParse(parts[1].Trim(), out var durationMs))
            {
                descriptor.Duration = TimeSpan.FromMilliseconds(durationMs);
            }
            else
            {
                throw new FormatException("Duración debe ser un número en milisegundos");
            }

            // Parsear parámetros opcionales
            if (parts.Length > 2 && int.TryParse(parts[2].Trim(), out var delayMs))
            {
                descriptor.Delay = TimeSpan.FromMilliseconds(delayMs);
            }

            if (parts.Length > 3)
            {
                descriptor.EasingFunction = _engine.GetEasing(parts[3].Trim());
            }
            else
            {
                descriptor.EasingFunction = _engine.GetEasing("Linear");
            }

            if (parts.Length > 4 && int.TryParse(parts[4].Trim(), out var repeat))
            {
                descriptor.RepeatCount = repeat;
            }

            if (parts.Length > 5 && bool.TryParse(parts[5].Trim(), out var autoReverse))
            {
                descriptor.AutoReverse = autoReverse;
            }

            return descriptor;
        }

        private object ParseValue(string value)
        {
            // Intentar parsear como double
            if (double.TryParse(value, out var doubleValue))
                return doubleValue;

            // Intentar parsear como Point (formato: x;y)
            if (value.Contains(';'))
            {
                var coords = value.Split(';');
                if (coords.Length == 2 && 
                    double.TryParse(coords[0], out var x) && 
                    double.TryParse(coords[1], out var y))
                {
                    return new Point(x, y);
                }
            }

            // Intentar parsear como Thickness
            if (value.Contains(' '))
            {
                var parts = value.Split(' ');
                if (parts.Length == 4 &&
                    double.TryParse(parts[0], out var left) &&
                    double.TryParse(parts[1], out var top) &&
                    double.TryParse(parts[2], out var right) &&
                    double.TryParse(parts[3], out var bottom))
                {
                    return new Thickness(left, top, right, bottom);
                }
            }

            return value;
        }
    }
}
