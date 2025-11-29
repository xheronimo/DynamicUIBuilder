// ============================================================================
// ARCHIVO 2: Controls/ControlFactory.cs
// ============================================================================
using System;
using Avalonia.Controls;

namespace DynamicUI.Controls
{
    public static class ControlFactory
    {
        public static Control Create(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return null;
            if (!ControlRegistry.TryGet(tipo, out var t)) return null;

            var ctor = t.GetConstructor(Type.EmptyTypes);
            if (ctor == null) throw new InvalidOperationException($"Tipo {t.Name} no tiene constructor público sin parámetros.");
            return (Control)ctor.Invoke(null);
        }
    }
}