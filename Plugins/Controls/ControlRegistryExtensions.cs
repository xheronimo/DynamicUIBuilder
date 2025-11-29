// ============================================================================
// ARCHIVO 6: Controls/ControlRegistryExtensions.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicUI.Controls
{
    /// <summary>
    /// Extensión de ControlRegistry para soportar unregister
    /// </summary>
    public static partial class ControlRegistry
    {
        private static readonly object _lock = new object();

        public static bool Unregister(string name)
        {
            lock (_lock)
            {
                // Acceder al diccionario privado mediante reflexión
                var field = typeof(ControlRegistry).GetField("_map", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (field != null)
                {
                    var map = field.GetValue(null) as Dictionary<string, Type>;
                    if (map != null && map.ContainsKey(name))
                    {
                        map.Remove(name);
                        return true;
                    }
                }
                return false;
            }
        }

        public static IEnumerable<string> GetRegisteredNames()
        {
            var field = typeof(ControlRegistry).GetField("_map",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (field != null)
            {
                var map = field.GetValue(null) as Dictionary<string, Type>;
                return map?.Keys.ToList() ?? Enumerable.Empty<string>();
            }

            return Enumerable.Empty<string>();
        }
    }
}