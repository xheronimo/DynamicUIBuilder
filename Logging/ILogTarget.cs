// ============================================================================
// ARCHIVO 5: Logging/ILogTarget.cs
// ============================================================================
namespace DynamicUI.Logging
{
    public interface ILogTarget
    {
        void Write(LogEntry entry);
    }
}
