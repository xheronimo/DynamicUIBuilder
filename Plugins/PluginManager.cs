// ============================================================================
// ARCHIVO 3: Plugins/PluginManager.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DynamicUI.Logging;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Gestor básico de plugins
    /// </summary>
    public class PluginManager
    {
        private readonly List<IUIPlugin> _loadedPlugins = new();
        private readonly IUILogger _logger;

        public IReadOnlyList<IUIPlugin> LoadedPlugins => _loadedPlugins.AsReadOnly();

        public PluginManager(IUILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Carga un plugin
        /// </summary>
        public void LoadPlugin(IUIPlugin plugin, PluginContext context)
        {
            try
            {
                _logger.LogInfo($"Cargando plugin: {plugin.Name} v{plugin.Version}");
                plugin.Initialize(context);
                _loadedPlugins.Add(plugin);
                _logger.LogInfo($"Plugin '{plugin.Name}' cargado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cargando plugin '{plugin.Name}': {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Carga todos los plugins de un assembly
        /// </summary>
        public void LoadPluginsFromAssembly(Assembly assembly, PluginContext context)
        {
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IUIPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in pluginTypes)
            {
                try
                {
                    var plugin = (IUIPlugin)Activator.CreateInstance(type);
                    LoadPlugin(plugin, context);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error instanciando plugin de tipo '{type.Name}': {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Descarga todos los plugins
        /// </summary>
        public void UnloadAllPlugins()
        {
            foreach (var plugin in _loadedPlugins.ToList())
            {
                try
                {
                    _logger.LogInfo($"Descargando plugin: {plugin.Name}");
                    plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error descargando plugin '{plugin.Name}': {ex.Message}", ex);
                }
            }
            _loadedPlugins.Clear();
        }
    }
}