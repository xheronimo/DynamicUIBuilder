// ============================================================================
// ARCHIVO 8: Logging/MemoryLogTarget.cs
// ============================================================================
using System.Collections.Generic;
using System.Linq;

namespace DynamicUI.Logging
{
    /// <summary>
    /// Target que mantiene logs en memoria con buffer circular
    /// </summary>
    public class MemoryLogTarget : ILogTarget
    {
        private readonly Queue<LogEntry> _buffer;
        private readonly int _maxEntries;
        private readonly object _lock = new();

        public MemoryLogTarget(int maxEntries = 1000)
        {
            _maxEntries = maxEntries;
            _buffer = new Queue<LogEntry>(maxEntries);
        }

        public void Write(LogEntry entry)
        {
            lock (_lock)
            {
                // Si el buffer está lleno, eliminar el más antiguo
                if (_buffer.Count >= _maxEntries)
                {
                    _buffer.Dequeue();
                }
                
                _buffer.Enqueue(entry);
            }
        }

        /// <summary>
        /// Obtiene todas las entradas almacenadas
        /// </summary>
        public IEnumerable<LogEntry> GetEntries()
        {
            lock (_lock)
            {
                return _buffer.ToArray();
            }
        }

        /// <summary>
        /// Obtiene entradas filtradas por nivel
        /// </summary>
        public IEnumerable<LogEntry> GetEntriesByLevel(LogLevel level)
        {
            lock (_lock)
            {
                return _buffer.Where(e => e.Level == level).ToArray();
            }
        }

        /// <summary>
        /// Limpia el buffer
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _buffer.Clear();
            }
        }

        /// <summary>
        /// Obtiene el número de entradas almacenadas
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _buffer.Count;
                }
            }
        }
    }
}
