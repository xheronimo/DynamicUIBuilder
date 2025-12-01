// ============================================================================
// ARCHIVO 4: Logging/UILogger.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DynamicUI.Logging
{
    public class UILogger : IUILogger
    {
        private readonly List<LogEntry> _logs = new();
        private readonly List<ILogTarget> _targets = new();
        private readonly object _lock = new();
        private LogLevel _minLevel = LogLevel.Info;

        public bool IsEnabled(LogLevel level) => level >= _minLevel;

        public void AddTarget(ILogTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            lock (_lock)
            {
                _targets.Add(target);
            }
        }

        public void RemoveTarget(ILogTarget target)
        {
            lock (_lock)
            {
                _targets.Remove(target);
            }
        }

        public void SetMinLevel(LogLevel level)
        {
            lock (_lock)
            {
                _minLevel = level;
            }
        }

        public void Log(
            LogLevel level,
            string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            if (!IsEnabled(level))
                return;

            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            LogEntry entry;
            ILogTarget[] targetsSnapshot;

            lock (_lock)
            {
                entry = new LogEntry
                {
                    Level = level,
                    Message = message,
                    Exception = ex,
                    Source = $"{fileName}.{memberName}"
                };

                _logs.Add(entry);

                targetsSnapshot = _targets.ToArray();
            }

            foreach (var target in targetsSnapshot)
            {
                try
                {
                    target.Write(entry);
                }
                catch
                {
                    // Evita que un target roto dañe el sistema de logs
                }
            }
        }

        public void LogTrace(string message) => Log(LogLevel.Trace, message);
        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message, Exception ex = null) => Log(LogLevel.Error, message, ex);
        public void LogCritical(string message, Exception ex = null) => Log(LogLevel.Critical, message, ex);

        public IEnumerable<LogEntry> GetLogs()
        {
            lock (_lock)
            {
                return _logs.ToList();
            }
        }

        public void ClearLogs()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }
    }
}
