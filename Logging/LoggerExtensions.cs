//DynamicUI.Logging / LoggerExtensions.cs
using System;
using DynamicUI.Logging.Targets;

namespace DynamicUI.Logging
{
    public static class LoggerExtensions
    {
        public static void AddConsole(this IUILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.AddTarget(new Targets.ConsoleLogTarget());
        }

        public static void AddColoredConsole(this IUILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.AddTarget(new Targets.ColoredConsoleLogTarget());
        }

        public static void AddFile(this IUILogger logger, string path)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.AddTarget(new Targets.FileLogTarget(path));
        }

        public static void AddJsonFile(this IUILogger logger, string path)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.AddTarget(new Targets.JsonFileLogTarget(path));
        }

        public static void AddMemoryTarget(this IUILogger logger, out MemoryLogTarget memoryTarget)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            memoryTarget = new MemoryLogTarget();
            logger.AddTarget(memoryTarget);
        }
    }
}
