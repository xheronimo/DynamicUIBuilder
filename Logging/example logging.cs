// ============================================================================
// EJEMPLO DE USO
// ============================================================================
/*
using System;
using DynamicUI.Logging;

public class LoggingExample
{
    public static void Main()
    {
        // 1. Crear logger
        var logger = new UILogger();
        
        // 2. Configurar nivel mínimo
        logger.SetMinLevel(LogLevel.Debug);
        
        // 3. Agregar targets
        logger.AddTarget(new ConsoleLogTarget(useColors: true));
        logger.AddTarget(new FileLogTarget("logs/dynamicui.log", appendTimestamp: true));
        
        var memoryTarget = new MemoryLogTarget(maxEntries: 500);
        logger.AddTarget(memoryTarget);
        
        // 4. Usar el logger
        logger.LogInfo("Aplicación iniciada");
        logger.LogDebug("Configuración cargada correctamente");
        logger.LogWarning("Archivo de configuración no encontrado, usando valores por defecto");
        
        try
        {
            // Código que puede fallar
            throw new InvalidOperationException("Error de ejemplo");
        }
        catch (Exception ex)
        {
            logger.LogError("Error en la operación", ex);
        }
        
        logger.LogCritical("Error crítico del sistema");
        
        // 5. Obtener logs
        var allLogs = logger.GetLogs();
        Console.WriteLine($"\nTotal de logs: {allLogs.Count()}");
        
        var errorLogs = memoryTarget.GetEntriesByLevel(LogLevel.Error);
        Console.WriteLine($"Logs de error: {errorLogs.Count()}");
        
        // 6. Limpiar logs
        logger.ClearLogs();
        memoryTarget.Clear();
        
        Console.WriteLine("Logs limpiados");
    }
}

// Ejemplo con múltiples targets
public class AdvancedLoggingExample
{
    public static void ConfigureLogging()
    {
        var logger = new UILogger();
        
        // Target compuesto para logging estructurado
        var compositeTarget = new CompositeLogTarget(
            new ConsoleLogTarget(useColors: true),
            new FileLogTarget("logs/app.log"),
            new FileLogTarget("logs/errors.log") // Solo errores
        );
        
        logger.AddTarget(compositeTarget);
        logger.SetMinLevel(LogLevel.Info);
        
        return logger;
    }
}
*/