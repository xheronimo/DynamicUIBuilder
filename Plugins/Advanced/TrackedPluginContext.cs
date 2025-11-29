// ============================================================================
// ARCHIVO 2: Plugins/Advanced/TrackedPluginContext.cs
// ============================================================================
using System;
using DynamicUI.Setters;
using DynamicUI.Validation;
using DynamicUI.Conversion;
using DynamicUI.Logging;

namespace DynamicUI.Plugins.Advanced
{
    /// <summary>
    /// Contexto mejorado que rastrea todos los registros
    /// </summary>
    public class TrackedPluginContext : Plugins.PluginContext
    {
        private readonly PluginRegistration _registration;

        public TrackedPluginContext(DynamicUIBuilder builder, IUILogger logger, PluginRegistration registration)
        {
            Builder = builder;
            Logger = logger;
            _registration = registration;
        }

        public new void RegisterControl(string name, Type type)
        {
            Controls.ControlRegistry.Register(name, type);
            _registration.Controls.Add(new ControlRegistration { Name = name, Type = type });
            Logger.LogInfo($"Control '{name}' registrado y rastreado para plugin '{_registration.PluginName}'");
        }

        public new void RegisterPropertySetter(IPropertySetter setter)
        {
            Builder.RegisterSetter(setter);
            _registration.PropertySetters.Add(setter);
            Logger.LogInfo($"PropertySetter '{setter.GetType().Name}' registrado y rastreado");
        }

        public new void RegisterValidator(IPropertyValidator validator)
        {
            Builder.RegisterValidator(validator);
            _registration.Validators.Add(validator);
            Logger.LogInfo($"Validator '{validator.GetType().Name}' registrado y rastreado");
        }

        public new void RegisterConverter(IValueConverter converter)
        {
            Builder.RegisterConverter(converter);
            _registration.Converters.Add(converter);
            Logger.LogInfo($"Converter '{converter.GetType().Name}' registrado y rastreado");
        }

        public void RegisterCustomCleanup(Action cleanup)
        {
            _registration.CustomCleanup = cleanup;
        }
    }
}