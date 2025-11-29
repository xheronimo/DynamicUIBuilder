
// ============================================================================
// ARCHIVO 4: Animation/AnimationPlugin.cs
// ============================================================================
using DynamicUI.Plugins;
using DynamicUI.Logging;

namespace DynamicUI.Animation
{
    /// <summary>
    /// Plugin de animaciones para DynamicUI
    /// </summary>
    public class AnimationPlugin : IUIPlugin
    {
        private AnimationEngine _engine;
        
        public string Name => "Animation System";
        public string Version => "2.0.0";
        public string Description => "Sistema completo de animaciones con soporte para Avalonia.Animation";

        public void Initialize(PluginContext context)
        {
            _engine = new AnimationEngine(context.Logger);
            context.RegisterPropertySetter(new AnimationSetter(_engine, context.Logger));
            context.Logger.LogInfo("Sistema de animaciones inicializado con soporte completo de Avalonia");
        }

        public void Shutdown()
        {
            _engine = null;
        }
    }
}