using Avalonia.Controls;
using System;

namespace DynamicUI.Setters
{
  
    public class GridDefinitionSetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
            => control is Grid &&
               (propertyName.Equals("ColumnDefinitions", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("RowDefinitions", StringComparison.OrdinalIgnoreCase));

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            if (control is not Grid grid) return false;

            try
            {
                var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (propertyName.Equals("ColumnDefinitions", StringComparison.OrdinalIgnoreCase))
                {
                    grid.ColumnDefinitions.Clear();
                    foreach (var p in parts)
                        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse(p.Trim())));
                }
                else if (propertyName.Equals("RowDefinitions", StringComparison.OrdinalIgnoreCase))
                {
                    grid.RowDefinitions.Clear();
                    foreach (var p in parts)
                        grid.RowDefinitions.Add(new RowDefinition(GridLength.Parse(p.Trim())));
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
