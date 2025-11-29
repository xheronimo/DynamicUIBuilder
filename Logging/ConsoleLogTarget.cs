
// ============================================================================
// ARCHIVO 6: Logging/ConsoleLogTarget.cs
// ============================================================================
using System;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Target que escribe logs a la consola con colores
    /// </summary>
    public class ConsoleLogTarget : ILogTarget
    {
        private readonly bool _useColors;

        public ConsoleLogTarget(bool useColors = true)
        {
            _useColors = useColors;
        }

        public void Write(LogEntry entry)
        {
            if (_useColors)
            {
                var color = GetColorForLevel(entry.Level);
                var originalColor = Console.ForegroundColor;
                
                try
                {
                    Console.ForegroundColor = color;
                    WriteEntry(entry);
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                }
            }
            else
            {
                WriteEntry(entry);
            }
        }

        private void WriteEntry(LogEntry entry)
        {
            Console.WriteLine($"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {entry.Message}");
            
            if (entry.Exception != null)
            {
                Console.WriteLine($"  Exception: {entry.Exception.Message}");
                
                if (entry.Level >= LogLevel.Error)
                {
                    Console.WriteLine($"  StackTrace: {entry.Exception.StackTrace}");
                }
            }
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };
        }
    }
}