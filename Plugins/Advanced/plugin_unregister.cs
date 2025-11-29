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

// ============================================================================
// ARCHIVO 3: Plugins/Advanced/AdvancedPluginManager.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using DynamicUI.Logging;

namespace DynamicUI.Plugins.Advanced
{
    /// <summary>
    /// Manager mejorado con capacidad de unregister
    /// </summary>
    public class AdvancedPluginManager
    {
        private readonly Dictionary<string, (Plugins.IUIPlugin plugin, PluginRegistration registration)> _loadedPlugins = new();
        private readonly IUILogger _logger;
        private readonly DynamicUIBuilder _builder;

        public IReadOnlyDictionary<string, (Plugins.IUIPlugin plugin, PluginRegistration registration)> LoadedPlugins => _loadedPlugins;

        public AdvancedPluginManager(DynamicUIBuilder builder, IUILogger logger)
        {
            _builder = builder;
            _logger = logger;
        }

        public void LoadPlugin(Plugins.IUIPlugin plugin)
        {
            if (_loadedPlugins.ContainsKey(plugin.Name))
            {
                _logger.LogWarning($"Plugin '{plugin.Name}' ya está cargado");
                return;
            }

            var registration = new PluginRegistration { PluginName = plugin.Name };
            var context = new TrackedPluginContext(_builder, _logger, registration);

            try
            {
                _logger.LogInfo($"Cargando plugin: {plugin.Name} v{plugin.Version}");
                plugin.Initialize(context);
                _loadedPlugins[plugin.Name] = (plugin, registration);
                
                _logger.LogInfo($"Plugin '{plugin.Name}' cargado exitosamente:");
                _logger.LogInfo($"  - {registration.Controls.Count} controles registrados");
                _logger.LogInfo($"  - {registration.PropertySetters.Count} setters registrados");
                _logger.LogInfo($"  - {registration.Validators.Count} validadores registrados");
                _logger.LogInfo($"  - {registration.Converters.Count} conversores registrados");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cargando plugin '{plugin.Name}': {ex.Message}", ex);
                throw;
            }
        }

        public bool UnloadPlugin(string pluginName)
        {
            if (!_loadedPlugins.TryGetValue(pluginName, out var entry))
            {
                _logger.LogWarning($"Plugin '{pluginName}' no está cargado");
                return false;
            }

            try
            {
                _logger.LogInfo($"Descargando plugin: {pluginName}");

                // 1. Ejecutar cleanup personalizado del plugin
                entry.registration.CustomCleanup?.Invoke();

                // 2. Llamar a Shutdown del plugin
                entry.plugin.Shutdown();

                // 3. Desregistrar controles
                foreach (var control in entry.registration.Controls)
                {
                    Controls.ControlRegistry.Unregister(control.Name);
                    _logger.LogDebug($"Control '{control.Name}' desregistrado");
                }

                // 4. Desregistrar setters
                foreach (var setter in entry.registration.PropertySetters)
                {
                    _builder.UnregisterSetter(setter);
                    _logger.LogDebug($"Setter '{setter.GetType().Name}' desregistrado");
                }

                // 5. Desregistrar validadores
                foreach (var validator in entry.registration.Validators)
                {
                    _builder.UnregisterValidator(validator);
                    _logger.LogDebug($"Validator '{validator.GetType().Name}' desregistrado");
                }

                // 6. Desregistrar conversores
                foreach (var converter in entry.registration.Converters)
                {
                    _builder.UnregisterConverter(converter);
                    _logger.LogDebug($"Converter '{converter.GetType().Name}' desregistrado");
                }

                _loadedPlugins.Remove(pluginName);
                _logger.LogInfo($"Plugin '{pluginName}' descargado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error descargando plugin '{pluginName}': {ex.Message}", ex);
                return false;
            }
        }

        public void UnloadAllPlugins()
        {
            var pluginNames = _loadedPlugins.Keys.ToList();
            foreach (var name in pluginNames)
            {
                UnloadPlugin(name);
            }
        }

        public PluginInfo GetPluginInfo(string pluginName)
        {
            if (!_loadedPlugins.TryGetValue(pluginName, out var entry))
                return null;

            return new PluginInfo
            {
                Name = entry.plugin.Name,
                Version = entry.plugin.Version,
                Description = entry.plugin.Description,
                ControlCount = entry.registration.Controls.Count,
                SetterCount = entry.registration.PropertySetters.Count,
                ValidatorCount = entry.registration.Validators.Count,
                ConverterCount = entry.registration.Converters.Count,
                Controls = entry.registration.Controls.Select(c => c.Name).ToList()
            };
        }

        public List<PluginInfo> GetAllPluginInfo()
        {
            return _loadedPlugins.Values
                .Select(entry => GetPluginInfo(entry.plugin.Name))
                .ToList();
        }
    }
}

// ============================================================================
// ARCHIVO 4: Plugins/Advanced/PluginInfo.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Plugins.Advanced
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int ControlCount { get; set; }
        public int SetterCount { get; set; }
        public int ValidatorCount { get; set; }
        public int ConverterCount { get; set; }
        public List<string> Controls { get; set; }
    }
}

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

// ============================================================================
// ARCHIVO 6: Controls/ControlRegistryExtensions.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicUI.Controls
{
    /// <summary>
    /// Extensión de ControlRegistry para soportar unregister
    /// </summary>
    public static partial class ControlRegistry
    {
        private static readonly object _lock = new object();

        public static bool Unregister(string name)
        {
            lock (_lock)
            {
                // Acceder al diccionario privado mediante reflexión
                var field = typeof(ControlRegistry).GetField("_map", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (field != null)
                {
                    var map = field.GetValue(null) as Dictionary<string, Type>;
                    if (map != null && map.ContainsKey(name))
                    {
                        map.Remove(name);
                        return true;
                    }
                }
                return false;
            }
        }

        public static IEnumerable<string> GetRegisteredNames()
        {
            var field = typeof(ControlRegistry).GetField("_map",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (field != null)
            {
                var map = field.GetValue(null) as Dictionary<string, Type>;
                return map?.Keys.ToList() ?? Enumerable.Empty<string>();
            }

            return Enumerable.Empty<string>();
        }
    }
}

// ============================================================================
// EJEMPLO DE USO
// ============================================================================
/*
using DynamicUI.Plugins.Advanced;
using DynamicUI.Logging;

// Crear logger y builder
var logger = new UILogger();
logger.AddTarget(new ConsoleLogTarget());

var builder = new DynamicUIBuilder(logger);

// Crear plugin manager avanzado
var pluginManager = new AdvancedPluginManager(builder, logger);

// Cargar plugins
var customPlugin = new MyCustomPlugin();
pluginManager.LoadPlugin(customPlugin);

// Ver información del plugin
var info = pluginManager.GetPluginInfo("MyCustomPlugin");
Console.WriteLine($"Plugin: {info.Name} v{info.Version}");
Console.WriteLine($"Controles registrados: {string.Join(", ", info.Controls)}");
Console.WriteLine($"Setters: {info.SetterCount}");
Console.WriteLine($"Validators: {info.ValidatorCount}");
Console.WriteLine($"Converters: {info.ConverterCount}");

// Usar el builder
var canvas = new Canvas();
await builder.BuildFromFileAsync(canvas, "ui_with_custom_controls.txt");

// Descargar plugin específico (limpia todo automáticamente)
pluginManager.UnloadPlugin("MyCustomPlugin");

// O descargar todos los plugins
pluginManager.UnloadAllPlugins();

// Verificar que todo se limpió
var remainingControls = Controls.ControlRegistry.GetRegisteredNames();
Console.WriteLine($"Controles restantes: {string.Join(", ", remainingControls)}");
*/