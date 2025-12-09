using Avalonia.Controls;
using DynamicUI.Setters;
using System;
namespace DynamicUI.Setters
{
    public class ClassesSetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
            => propertyName.Equals("Classes", StringComparison.OrdinalIgnoreCase);

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    foreach (var cls in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        control.Classes.Add(cls.Trim());
                }
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
