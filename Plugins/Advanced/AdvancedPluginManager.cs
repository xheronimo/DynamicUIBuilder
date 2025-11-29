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