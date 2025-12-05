
// ============================================================================
// ARCHIVO 3: Controls/ControlRegistry.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace DynamicUI.Controls
{
    public static class ControlRegistry
    {
        private static readonly Dictionary<string, Type> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            {"TextBlock", typeof(TextBlock)},
            {"Label", typeof(TextBlock)},
            {"TextBox", typeof(TextBox)},
            {"Image", typeof(Image)},
            {"ComboBox", typeof(ComboBox)},
            {"CheckBox", typeof(CheckBox)},
            {"Button", typeof(Button)},
            {"Border", typeof(Border)},
            {"Rectangle", typeof(Border)},
        };

        private static readonly object _lock = new object();

        public static void Register(string name, Type type)
        {
            if (!typeof(Control).IsAssignableFrom(type)) 
                throw new ArgumentException("Type must inherit Control", nameof(type));
            
            lock (_lock)
            {
                _map[name] = type;
            }
        }

        public static bool TryGet(string name, out Type type) => _map.TryGetValue(name, out type);

        public static bool Unregister(string name)
        {
            lock (_lock)
            {
                return _map.Remove(name);
            }
        }

        public static IEnumerable<string> GetRegisteredNames()
        {
            lock (_lock)
            {
                return _map.Keys.ToList();
            }
        }
    }
}