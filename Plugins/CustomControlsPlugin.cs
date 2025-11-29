// ============================================================================
// ARCHIVO 4: Plugins/CustomControlsPlugin.cs (EJEMPLO)
// ============================================================================
using System;

namespace DynamicUI.Plugins
{
    /// <summary>
    /// Ejemplo de plugin para controles personalizados
    /// </summary>
    public class CustomControlsPlugin : IUIPlugin
    {
        public string Name => "Custom Controls";
        public string Version => "1.0.0";
        public string Description => "Agrega controles personalizados al sistema";

        public void Initialize(PluginContext context)
        {
            // Ejemplo: Registrar controles personalizados
            // context.RegisterControl("MyCustomButton", typeof(MyCustomButton));
            // context.RegisterControl("MyCustomPanel", typeof(MyCustomPanel));
            
            // Ejemplo: Registrar setters especializados
            // context.RegisterPropertySetter(new MyCustomSetter());
            
            context.Logger.LogInfo("CustomControlsPlugin inicializado");
        }

        public void Shutdown()
        {
            // Limpiar recursos si es necesario
        }
    }
}