using Avalonia.Controls;
using System;

namespace DynamicUI.Setters
{
  

    public class ToolTipSetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
            => propertyName.Equals("ToolTip", StringComparison.OrdinalIgnoreCase);

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            try
            {
                ToolTip.SetTip(control, value);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }

}
