// ============================================================================
// ARCHIVO 4: Controls/Custom/AutoFitTextBlockPlugin.cs
// ============================================================================
using DynamicUI.Plugins;

namespace DynamicUI.Controls.Custom
{
    /// <summary>
    /// Plugin que registra AutoFitTextBlock en el sistema
    /// </summary>
    public class AutoFitTextBlockPlugin : IUIPlugin
    {
        public string Name => "AutoFitTextBlock";
        public string Version => "1.0.0";
        public string Description => "TextBlock con ajuste automático de tamaño de fuente y modo debug visual";

        public void Initialize(PluginContext context)
        {
            // Registrar el control
            context.RegisterControl("AutoFitTextBlock", typeof(AutoFitTextBlock));
            
            // Registrar setter especializado
            context.RegisterPropertySetter(new AutoFitTextBlockSetter());
            
            // Registrar validador
            context.RegisterValidator(new AutoFitTextBlockValidator());
            
            context.Logger.LogInfo("AutoFitTextBlock plugin inicializado correctamente con soporte de debug");
        }

        public void Shutdown()
        {
            // Cleanup si es necesario
        }
    }
}