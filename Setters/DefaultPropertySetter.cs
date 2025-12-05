
// ============================================================================
// ARCHIVO 2: Setters/DefaultPropertySetter.cs
// ============================================================================
using System;
using Avalonia.Controls;

namespace DynamicUI.Setters
{
    /// <summary>
    /// Setter por reflexión para propiedades normales (versión básica)
    /// </summary>
    public class DefaultPropertySetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName) => true; // fallback

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            var prop = Controls.CachedReflection.GetProperty(control.GetType(), propertyName);
            
            if (prop == null)
            {
                error = "Propiedad no encontrada";
                return false;
            }
            
            if (!prop.CanWrite)
            {
                error = "Propiedad de solo lectura";
                return false;
            }

            try
            {
                object converted = ConvertValue(value, prop.PropertyType);
                if (converted == null)
                {
                    error = $"No se pudo convertir '{value}' a {prop.PropertyType.Name}";
                    return false;
                }
                prop.SetValue(control, converted);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private object ConvertValue(string valor, Type type)
        {
            if (type == typeof(string)) 
                return valor;
            
            var underlying = Nullable.GetUnderlyingType(type) ?? type;

            try
            {
                var conv = System.ComponentModel.TypeDescriptor.GetConverter(underlying);
                if (conv != null && conv.CanConvertFrom(typeof(string)))
                    return conv.ConvertFromInvariantString(valor);
            }
            catch { }

            // Intentar conversiones básicas
            if (underlying == typeof(double) && double.TryParse(valor, out var d)) 
                return d;
            
            if (underlying == typeof(int) && int.TryParse(valor, out var i)) 
                return i;
            
            if (underlying == typeof(bool) && bool.TryParse(valor, out var b)) 
                return b;

            return null;
        }
    }
}