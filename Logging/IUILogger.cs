// ============================================================================
// ARCHIVO 2: Logging/// ============================================================================
// ARCHIVO 3: Logging/IUILogger.cs
// ============================================================================
using System;
using System.Collections.Generic;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Interfaz principal para el sistema de logging
    /// </summary>
    public interface IUILogger
    {
        /// <summary>
        /// Registra un log con nivel y mensaje específicos
        /// </summary>
        void Log(LogLevel level, string message, Exception ex = null);
        
        /// <summary>
        /// Registra un log de nivel Debug
        /// </summary>
        void LogDebug(string message);
        
        /// <summary>
        /// Registra un log de nivel Info
        /// </summary>
        void LogInfo(string message);
        
        /// <summary>
        /// Registra un log de nivel Warning
        /// </summary>
        void LogWarning(string message);
        
        /// <summary>
        /// Registra un log de nivel Error
        /// </summary>
        void LogError(string message, Exception ex = null);
        
        /// <summary>
        /// Registra un log de nivel Critical
        /// </summary>
        void LogCritical(string message, Exception ex = null);
        
        /// <summary>
        /// Obtiene todos los logs registrados
        /// </summary>
        IEnumerable<LogEntry> GetLogs();
    }
}
