// ============================================================================
// ARCHIVO 1: Setters/ImageSetter.cs
// ============================================================================
using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace DynamicUI.Setters
{
    public class ImageSetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
        {
            return control is Image && 
                   (propertyName.Equals("Source", StringComparison.OrdinalIgnoreCase) || 
                    propertyName.Equals("Imagen", StringComparison.OrdinalIgnoreCase));
        }

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            try
            {
                var img = control as Image;
                img.Source = new Bitmap(value);
                return true;
            }
            catch (Exception ex)
            {
                error = $"No se pudo cargar la imagen: {ex.Message}";
                return false;
            }
        }
    }
}