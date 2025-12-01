// ============================================================================
// ARCHIVO 2: Logging/// ============================================================================
// ARCHIVO 3: Logging/IUILogger.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DynamicUI.Logging
{
    public interface IUILogger
    {
        void Log(
            LogLevel level,
            string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = ""
        );

        void LogTrace(string message);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception ex = null);
        void LogCritical(string message, Exception ex = null);

        void AddTarget(ILogTarget target);
        void RemoveTarget(ILogTarget target);
        void SetMinLevel(LogLevel level);
        IEnumerable<LogEntry> GetLogs();
        void ClearLogs();

        bool IsEnabled(LogLevel level);
    }
}
