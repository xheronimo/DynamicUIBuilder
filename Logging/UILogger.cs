// ============================================================================
// ARCHIVO 4: Logging/UILogger.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Implementación principal del logger con soporte multi-target
    /// </summary>
    public class UILogger : IUILogger
    {
        private readonly List<LogEntry> _logs = new();
        private readonly List<ILogTarget> _targets = new();
        private LogLevel _minLevel = LogLevel.Info;

        /// <summary>
        /// Agrega un target de salida para los logs
        /// </summary>
        public void AddTarget(ILogTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            _targets.Add(target);
        }

        /// <summary>
        /// Elimina un target de salida
        /// </summary>
        public void RemoveTarget(ILogTarget target)
        {
            _targets.Remove(target);
        }

        /// <summary>
        /// Establece el nivel mínimo de log a registrar
        /// </summary>
        public void SetMinLevel(LogLevel level)
        {
            _minLevel = level;
        }

        /// <summary>
        /// Registra un log
        /// </summary>
        public void Log(LogLevel level, string message, Exception ex = null)
        {
            if (level < _minLevel)
                return;

            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message,
                Exception = ex,
                Source = GetCallerInfo()
            };

            _logs.Add(entry);

            // Enviar a todos los targets
            foreach (var target in _targets)
            {
                try
                {
                    target.Write(entry);
                }
                catch (Exception targetEx)
                {
                    // Si un target falla, no debemos fallar todo el logging
                    // Intentar escribir en consola como último recurso
                    try
                    {
                        Console.WriteLine($"Error en log target: {targetEx.Message}");
                    }
                    catch
                    {
                        // Si ni siquiera podemos escribir en consola, ignorar silenciosamente
                    }
                }
            }
        }

        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message, Exception ex = null) => Log(LogLevel.Error, message, ex);
        public void LogCritical(string message, Exception ex = null) => Log(LogLevel.Critical, message, ex);

        public IEnumerable<LogEntry> GetLogs() => _logs;

        /// <summary>
        /// Limpia todos los logs almacenados
        /// </summary>
        public void ClearLogs()
        {
            _logs.Clear();
        }

        /// <summary>
        /// Obtiene información del caller para tracking
        /// </summary>
        private string GetCallerInfo()
        {
            try
            {
                var stackTrace = new StackTrace(2, false);
                var frame = stackTrace.GetFrame(0);
                var method = frame?.GetMethod();
                
                if (method != null)
                {
                    var className = method.DeclaringType?.Name ?? "Unknown";
                    var methodName = method.Name;
                    return $"{className}.{methodName}";
                }
            }
            catch
            {
                // Si falla la obtención del stack trace, ignorar
            }

            return "Unknown";
        }
    }
}