
// ============================================================================
// EJEMPLO DE USO COMPLETO
// ============================================================================
/*
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicUI;
using DynamicUI.Logging;
using DynamicUI.Plugins;

public class PluginSystemExample
{
    public static async Task Main()
    {
        // 1. Crear logger
        var logger = new UILogger();
        logger.AddTarget(new ConsoleLogTarget());
        logger.SetMinLevel(LogLevel.Info);

        // 2. Crear builder
        var builder = new DynamicUIBuilder(logger)
        {
            DataContextResolver = name => new { Title = "Mi Aplicación" }
        };

        // 3. Crear plugin manager
        var pluginManager = new PluginManager(logger);
        
        // 4. Crear contexto para plugins
        var context = new PluginContext
        {
            Builder = builder,
            Logger = logger
        };

        // 5. Cargar plugins
        pluginManager.LoadPlugin(new CustomControlsPlugin(), context);
        pluginManager.LoadPlugin(new SecurityValidationPlugin(), context);
        pluginManager.LoadPlugin(new AdvancedConvertersPlugin(), context);
        pluginManager.LoadPlugin(new AnimationPlugin(), context);

        // 6. Ver plugins cargados
        Console.WriteLine($"Plugins cargados: {pluginManager.LoadedPlugins.Count}");
        foreach (var plugin in pluginManager.LoadedPlugins)
        {
            Console.WriteLine($"  - {plugin.Name} v{plugin.Version}: {plugin.Description}");
        }

        // 7. Usar el builder con los plugins cargados
        var canvas = new Canvas();
        await builder.BuildFromFileAsync(canvas, "interface_with_plugins.txt");

        // 8. Descargar todos los plugins al terminar
        pluginManager.UnloadAllPlugins();
    }
}

// Ejemplo de archivo interface_with_plugins.txt:
/*
# Interfaz usando características de plugins

TextBlock Text="<script>alert('test')</script>"; X=10; Y=10
# El SecurityValidationPlugin detectará el XSS

Button Text="Animate me"; X=10; Y=50; Animate:Opacity=0,1000
# El AnimationPlugin procesará la animación

TextBox Text="SELECT * FROM users"; X=10; Y=100
# El SecurityValidationPlugin advertirá sobre SQL
*/
*/