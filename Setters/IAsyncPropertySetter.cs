// ============================================================================
// ARCHIVO: Setters/IAsyncPropertySetter.cs
// ============================================================================
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace DynamicUI.Setters
{
    /// <summary>
    /// Interfaz para setters que soportan operaciones asincrónicas
    /// </summary>
    public interface IAsyncPropertySetter : IPropertySetter
    {
        /// <summary>
        /// Aplica una propiedad de forma asincrónica
        /// </summary>
        Task<bool> ApplyAsync(Control control, string propertyName, string value, CancellationToken ct);
    }
}