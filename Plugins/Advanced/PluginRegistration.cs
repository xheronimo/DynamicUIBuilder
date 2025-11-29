// ============================================================================
// ARCHIVO 1: Plugins/Advanced/PluginRegistration.cs
// ============================================================================
using System;
using System.Collections.Generic;
using DynamicUI.Setters;
using DynamicUI.Validation;
using DynamicUI.Conversion;

namespace DynamicUI.Plugins.Advanced
{
    /// <summary>
    /// Registro de componentes agregados por un plugin
    /// </summary>
    public class PluginRegistration
    {
        public string PluginName { get; set; }
        public List<ControlRegistration> Controls { get; } = new();
        public List<IPropertySetter> PropertySetters { get; } = new();
        public List<IPropertyValidator> Validators { get; } = new();
        public List<IValueConverter> Converters { get; } = new();
        public Action CustomCleanup { get; set; }
    }

    public class ControlRegistration
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}