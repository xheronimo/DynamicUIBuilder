//samples/ProgramConsole.cs (Ejemplo de uso en consola)
using System;
using DynamicUI.Logging;
using DynamicUI.Logging.Targets;

class Program
{
    static void Main()
    {
        var logger = new UILogger();
        logger.SetMinLevel(LogLevel.Debug);
        logger.AddTarget(new ColoredConsoleLogTarget());
        logger.AddTarget(new FileLogTarget("logs/app.log"));
        logger.AddTarget(new JsonFileLogTarget("logs/app.json"));

        logger.LogInfo("Aplicación iniciada");
        logger.LogDebug("Debug info");
        try
        {
            throw new InvalidOperationException("Ejemplo");
        }
        catch (Exception ex)
        {
            logger.LogError("Ocurrió un error", ex);
        }

        Console.WriteLine("Pulsa tecla para salir...");
        Console.ReadKey();
    }
}
