
// ============================================================================
// ARCHIVO 7: Plugins/AnimationPlugin.cs (EJEMPLO)
// ============================================================================
using System;
using Avalonia.Controls;
using DynamicUI.Setters;
using DynamicUI.Logging;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Plugin básico para soporte de animaciones
    /// (La versión completa está en DynamicUI.Animation)
    /// </summary>
    public class AnimationPlugin : IUIPlugin
    {
        public string Name => "Animation Support";
        public string Version => "1.0.0";
        public string Description => "Agrega soporte básico para animaciones declarativas";

        public void Initialize(PluginContext context)
        {
            context.RegisterPropertySetter(new SimpleAnimationSetter(context.Logger));
            context.Logger.LogInfo("AnimationPlugin inicializado");
        }

        public void Shutdown() { }

        // Setter simple de animaciones
        private class SimpleAnimationSetter : IPropertySetter
        {
            private readonly IUILogger _logger;

            public SimpleAnimationSetter(IUILogger logger)
            {
                _logger = logger;
            }

            public bool CanHandle(Control control, string propertyName)
            {
                return propertyName.StartsWith("Animate:", StringComparison.OrdinalIgnoreCase);
            }

            public bool Apply(Control control, string propertyName, string value, out string error)
            {
                error = null;
                
                // Formato básico: Animate:Property=value,duration
                // Ejemplo: Animate:Opacity=0,1000
                
                try
                {
                    var parts = value.Split(',');
                    if (parts.Length < 2)
                    {
                        error = "Formato de animación inválido. Use: valor,duración";
                        return false;
                    }

                    var targetProperty = propertyName.Substring(8); // Remover "Animate:"
                    var targetValue = parts[0].Trim();
                    var duration = int.Parse(parts[1].Trim());

                    _logger.LogDebug($"Animación registrada: {targetProperty} -> {targetValue} en {duration}ms");
                    
                    // Aquí se implementaría la lógica real de animación
                    // Por ahora solo lo registramos
                    
                    return true;
                }
                catch (Exception ex)
                {
                    error = $"Error parseando animación: {ex.Message}";
                    return false;
                }
            }
        }
    }
}