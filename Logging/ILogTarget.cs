// ============================================================================
// ARCHIVO 5: Logging/ILogTarget.cs
// ============================================================================
namespace DynamicUI.Logging
{
    /// <summary>
    /// Interfaz para destinos de log (consola, archivo, etc.)
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// Escribe una entrada de log
        /// </summary>
        void Write(LogEntry entry);
    }
}
