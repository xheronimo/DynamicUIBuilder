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