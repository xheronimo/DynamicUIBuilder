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
            
            // Intentar obtener del registro
            if (!ControlRegistry.TryGet(tipo, out var t))
            {
                // Si no existe, intentar auto-registrar
                if (!ControlAutoRegistration.TryAutoRegister(tipo))
                {
                    Console.WriteLine($"❌ Control '{tipo}' no encontrado y no se pudo auto-registrar");
                    return null;
                }
                
                // Intentar de nuevo después del auto-registro
                if (!ControlRegistry.TryGet(tipo, out t))
                    return null;
            }

            var ctor = t.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException($"Tipo {t.Name} no tiene constructor público sin parámetros.");
            }
            
            return (Control)ctor.Invoke(null);
        }
    }
}