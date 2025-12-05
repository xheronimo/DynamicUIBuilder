// ============================================================================
// ARCHIVO 2: Controls/Custom/AutoFitTextBlockSetter.cs
// ============================================================================
using System;
using Avalonia.Controls;
using DynamicUI.Setters;

namespace DynamicUI.Controls.Custom
{
    /// <summary>
    /// Setter especializado para AutoFitTextBlock
    /// </summary>
    public class AutoFitTextBlockSetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
        {
            if (control is not AutoFitTextBlock)
                return false;

            return propertyName.Equals("MinFontSize", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("MaxFontSize", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("AutoFitEnabled", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("ShowDebugInfo", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("ShowDebug", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Debug", StringComparison.OrdinalIgnoreCase) ||
                   propertyName.Equals("Name", StringComparison.OrdinalIgnoreCase);
        }

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            var autoFit = control as AutoFitTextBlock;

            try
            {
                if (propertyName.Equals("MinFontSize", StringComparison.OrdinalIgnoreCase))
                {
                    if (!double.TryParse(value, out var size) || size <= 0)
                    {
                        error = "MinFontSize debe ser un número positivo";
                        return false;
                    }
                    autoFit.MinFontSize = size;
                    return true;
                }

                if (propertyName.Equals("MaxFontSize", StringComparison.OrdinalIgnoreCase))
                {
                    if (!double.TryParse(value, out var size) || size <= 0)
                    {
                        error = "MaxFontSize debe ser un número positivo";
                        return false;
                    }
                    autoFit.MaxFontSize = size;
                    return true;
                }

                if (propertyName.Equals("AutoFitEnabled", StringComparison.OrdinalIgnoreCase))
                {
                    if (!bool.TryParse(value, out var enabled))
                    {
                        error = "AutoFitEnabled debe ser true o false";
                        return false;
                    }
                    autoFit.AutoFitEnabled = enabled;
                    return true;
                }

                if (propertyName.Equals("ShowDebugInfo", StringComparison.OrdinalIgnoreCase) ||
                    propertyName.Equals("ShowDebug", StringComparison.OrdinalIgnoreCase) ||
                    propertyName.Equals("Debug", StringComparison.OrdinalIgnoreCase))
                {
                    if (!bool.TryParse(value, out var show))
                    {
                        error = "ShowDebugInfo debe ser true o false";
                        return false;
                    }
                    autoFit.ShowDebugInfo = show;
                    return true;
                }

                if (propertyName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    autoFit.Name = value;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}