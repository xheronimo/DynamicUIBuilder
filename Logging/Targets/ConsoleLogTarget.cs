//ConsoleLogTarget.cs
using System;

namespace DynamicUI.Logging.Targets
{
    public class ConsoleLogTarget : ILogTarget
    {
        public void Write(LogEntry entry)
        {
            Console.WriteLine(entry.ToString());
        }
    }
}
