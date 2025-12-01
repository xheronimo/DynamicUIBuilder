//DynamicUI.Logging.Targets / ColoredConsoleLogTarget.cs
using System;

namespace DynamicUI.Logging.Targets
{
    /// <summary>
    /// Consola con color por nivel.
    /// </summary>
    public class ColoredConsoleLogTarget : ILogTarget
    {
        public void Write(LogEntry entry)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = GetColor(entry.Level);
            Console.WriteLine(entry.ToString());
            Console.ForegroundColor = prev;
        }

        private ConsoleColor GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => ConsoleColor.DarkGray,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }
    }
}
