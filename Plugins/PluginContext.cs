
// ============================================================================
// ARCHIVO 2: Plugins/PluginContext.cs
// ============================================================================
using System;
using DynamicUI.Setters;
using DynamicUI.Validation;
using DynamicUI.Conversion;
using DynamicUI.Logging;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Contexto proporcionado a los plugins durante su inicialización
    /// </summary>
    public class PluginContext
    {
        public DynamicUIBuilder Builder { get; set; }
        public IUILogger Logger { get; set; }
        
        /// <summary>
        /// Registra un nuevo tipo de control
        /// </summary>
        public void RegisterControl(string name, Type type)
        {
            Controls.ControlRegistry.Register(name, type);
            Logger.LogInfo($"Control '{name}' registrado por plugin");
        }

        /// <summary>
        /// Registra un setter de propiedades personalizado
        /// </summary>
        public void RegisterPropertySetter(IPropertySetter setter)
        {
            Builder.RegisterSetter(setter);
            Logger.LogInfo($"PropertySetter '{setter.GetType().Name}' registrado por plugin");
        }

        /// <summary>
        /// Registra un validador de propiedades
        /// </summary>
        public void RegisterValidator(IPropertyValidator validator)
        {
            Builder.RegisterValidator(validator);
            Logger.LogInfo($"Validator '{validator.GetType().Name}' registrado por plugin");
        }

        /// <summary>
        /// Registra un conversor de tipos
        /// </summary>
        public void RegisterConverter(IValueConverter converter)
        {
            Builder.RegisterConverter(converter);
            Logger.LogInfo($"Converter '{converter.GetType().Name}' registrado por plugin");
        }
    }
}