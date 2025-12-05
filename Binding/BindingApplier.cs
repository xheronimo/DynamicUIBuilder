
// ============================================================================
// ARCHIVO 8: Binding/BindingApplier.cs
// ============================================================================
using System;
using Avalonia.Data;
using Avalonia.Controls;
using DynamicUI.Controls;

namespace DynamicUI.Binding
{
    public static class BindingApplier
    {
        public static void ApplyBinding(Control control, string propertyName, string path)
        {
            var prop = CachedReflection.GetProperty(control.GetType(), propertyName);
            if (prop == null)
            {
                Console.WriteLine($"Binding: propiedad '{propertyName}' no encontrada en {control.GetType().Name}");
                return;
            }

            var binding = new Avalonia.Data.Binding(path);
            
            // Buscar el AvaloniaProperty correspondiente
            var avaloniaProperty = FindAvaloniaProperty(control.GetType(), propertyName);
            if (avaloniaProperty != null)
            {
                control.Bind(avaloniaProperty, binding);
            }
            else
            {
                Console.WriteLine($"Binding: No se encontró AvaloniaProperty para '{propertyName}'");
            }
        }

        private static Avalonia.AvaloniaProperty FindAvaloniaProperty(Type type, string propertyName)
        {
            var fieldName = propertyName + "Property";
            var field = type.GetField(fieldName, 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Static | 
                System.Reflection.BindingFlags.FlattenHierarchy);

            return field?.GetValue(null) as Avalonia.AvaloniaProperty;
        }
    }
}