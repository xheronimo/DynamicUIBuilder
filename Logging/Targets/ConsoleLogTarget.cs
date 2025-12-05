//ConsoleLogTarget.cs
using System;

namespace DynamicUI.Logging.Targets
{
    public class ConsoleLogTarget : ILogTarget
    {
        private readonly object _consoleLock = new();

        public void Write(LogEntry entry)
        {
            lock (_consoleLock)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = GetColor(entry.Level);
                Console.WriteLine(entry.ToString());
                Console.ForegroundColor = prev;
            }
        }
        private ConsoleColor GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Critical => ConsoleColor.Red,
                LogLevel.Error => ConsoleColor.DarkRed,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Debug => ConsoleColor.Cyan,
                LogLevel.Trace => ConsoleColor.DarkGray,
                _ => ConsoleColor.White
            };
        }
    }
}
