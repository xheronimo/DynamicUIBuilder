// ============================================================================
// ARCHIVO 9: Logging/CompositeLogTarget.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Target compuesto que escribe a múltiples targets
    /// </summary>
    public class CompositeLogTarget : ILogTarget
    {
        private readonly List<ILogTarget> _targets = new();

        public CompositeLogTarget(params ILogTarget[] targets)
        {
            _targets.AddRange(targets);
        }

        public void AddTarget(ILogTarget target)
        {
            _targets.Add(target);
        }

        public void Write(LogEntry entry)
        {
            foreach (var target in _targets)
            {
                try
                {
                    target.Write(entry);
                }
                catch
                {
                    // Si un target falla, continuar con los demás
                }
            }
        }
    }
}