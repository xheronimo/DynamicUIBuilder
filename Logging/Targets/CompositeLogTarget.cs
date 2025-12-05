//DynamicUI.Logging.Targets / CompositeLogTarget.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicUI.Logging.Targets
{
    /// <summary>
    /// Envía un LogEntry a múltiples targets en paralelo seguro.
    /// </summary>
    public class CompositeLogTarget : ILogTarget, IDisposable
    {
        private readonly List<ILogTarget> _targets = new();

        public CompositeLogTarget() { }

        public CompositeLogTarget(IEnumerable<ILogTarget> targets)
        {
            if (targets == null) return;
            _targets.AddRange(targets);
        }

        public void Add(ILogTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            _targets.Add(target);
        }

        public bool Remove(ILogTarget target)
        {
            return _targets.Remove(target);
        }

        public void Write(LogEntry entry)
        {
            // Se escribe en cada target; no permitimos que uno falle y detenga los demás.
            var snapshot = _targets.ToArray();
            foreach (var t in snapshot)
            {
                try
                {
                    t.Write(entry);
                }
                catch
                {
                    // swallow: no queremos excepciones desde targets
                }
            }
        }

        public void Dispose()
        {
            foreach (var t in _targets.OfType<IDisposable>())
            {
                try { t.Dispose(); } catch { }
            }
            _targets.Clear();
        }
    }
}
