// ============================================================================
// ARCHIVO 1: Plugins/IUIPlugin.cs
// ============================================================================
using System;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Interfaz base para todos los plugins de DynamicUI
    /// </summary>
    public interface IUIPlugin
    {
        /// <summary>
        /// Nombre del plugin
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Versión del plugin
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Descripción del plugin
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Inicializa el plugin y registra sus componentes
        /// </summary>
        /// <param name="context">Contexto con acceso al builder y logger</param>
        void Initialize(PluginContext context);
        
        /// <summary>
        /// Limpia recursos cuando el plugin se descarga
        /// </summary>
        void Shutdown();
    }
}