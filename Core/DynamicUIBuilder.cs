// ============================================================================
// ARCHIVO: Core/DynamicUIBuilder.cs
// ============================================================================
using DynamicUI.Logging;

namespace DynamicUI
{
    /// <summary>
    /// Alias simple. Usa DynamicUIBuilderV3 directamente donde lo necesites.
    /// </summary>
    public class DynamicUIBuilder : V3.DynamicUIBuilderV3
    {
        public DynamicUIBuilder() : base(null)
        {
        }

        public DynamicUIBuilder(IUILogger logger) : base(logger)
        {
        }
    }
}