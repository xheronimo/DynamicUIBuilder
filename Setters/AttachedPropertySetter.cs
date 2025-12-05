using System;
using Avalonia.Controls;

namespace DynamicUI.Setters
{
    public class AttachedPropertySetter : IPropertySetter
    {
        public bool CanHandle(Control control, string propertyName)
        {
            // Propiedades directas sin punto
            if (propertyName.Equals("X", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("ZIndex", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("Row", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("Column", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("RowSpan", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("ColumnSpan", StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals("Dock", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Propiedades con sintaxis de punto
            if (propertyName.Contains('.'))
            {
                var parts = propertyName.Split('.', 2);
                if (parts.Length == 2)
                {
                    var container = parts[0];
                    return container.Equals("Grid", StringComparison.OrdinalIgnoreCase) ||
                           container.Equals("Canvas", StringComparison.OrdinalIgnoreCase) ||
                           container.Equals("DockPanel", StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;
        }

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;

            try
            {
                // ═══════════════════════════════════════════════════════
                // CANVAS - Posicionamiento absoluto
                // ═══════════════════════════════════════════════════════
                if (propertyName.Equals("X", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(value, out var d))
                    {
                        Canvas.SetLeft(control, d);
                        return true;
                    }
                    error = "Valor X inválido";
                    return false;
                }

                if (propertyName.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(value, out var d))
                    {
                        Canvas.SetTop(control, d);
                        return true;
                    }
                    error = "Valor Y inválido";
                    return false;
                }

                if (propertyName.Equals("ZIndex", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var z))
                    {
                        // ZIndex está en Panel, no en Canvas
                        control.ZIndex = z;
                        return true;
                    }
                    error = "Valor ZIndex inválido";
                    return false;
                }

                // ═══════════════════════════════════════════════════════
                // GRID - Layout en filas y columnas
                // ═══════════════════════════════════════════════════════
                if (propertyName.Equals("Row", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var row))
                    {
                        Grid.SetRow(control, row);
                        return true;
                    }
                    error = "Valor Row inválido";
                    return false;
                }

                if (propertyName.Equals("Column", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var col))
                    {
                        Grid.SetColumn(control, col);
                        return true;
                    }
                    error = "Valor Column inválido";
                    return false;
                }

                if (propertyName.Equals("RowSpan", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var span))
                    {
                        Grid.SetRowSpan(control, span);
                        return true;
                    }
                    error = "Valor RowSpan inválido";
                    return false;
                }

                if (propertyName.Equals("ColumnSpan", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var span))
                    {
                        Grid.SetColumnSpan(control, span);
                        return true;
                    }
                    error = "Valor ColumnSpan inválido";
                    return false;
                }

                // ═══════════════════════════════════════════════════════
                // DOCKPANEL - Anclaje a bordes
                // ═══════════════════════════════════════════════════════
                if (propertyName.Equals("Dock", StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse<Dock>(value, true, out var dock))
                    {
                        DockPanel.SetDock(control, dock);
                        return true;
                    }
                    error = "Valor Dock inválido. Use: Left, Right, Top, Bottom";
                    return false;
                }

                // ═══════════════════════════════════════════════════════
                // SINTAXIS CON PUNTO: Container.Property
                // ═══════════════════════════════════════════════════════
                if (propertyName.Contains('.'))
                {
                    var parts = propertyName.Split('.', 2);
                    if (parts.Length != 2)
                    {
                        error = "Sintaxis inválida para propiedad adjunta";
                        return false;
                    }

                    var container = parts[0];
                    var prop = parts[1];

                    // Grid.Row, Grid.Column, etc.
                    if (container.Equals("Grid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(value, out var idx))
                        {
                            if (prop.Equals("Row", StringComparison.OrdinalIgnoreCase))
                            {
                                Grid.SetRow(control, idx);
                                return true;
                            }
                            if (prop.Equals("Column", StringComparison.OrdinalIgnoreCase))
                            {
                                Grid.SetColumn(control, idx);
                                return true;
                            }
                            if (prop.Equals("RowSpan", StringComparison.OrdinalIgnoreCase))
                            {
                                Grid.SetRowSpan(control, idx);
                                return true;
                            }
                            if (prop.Equals("ColumnSpan", StringComparison.OrdinalIgnoreCase))
                            {
                                Grid.SetColumnSpan(control, idx);
                                return true;
                            }
                        }
                        error = $"Propiedad Grid.{prop} no soportada o valor inválido";
                        return false;
                    }

                    // Canvas.Left, Canvas.Top, etc.
                    if (container.Equals("Canvas", StringComparison.OrdinalIgnoreCase))
                    {
                        if (double.TryParse(value, out var d))
                        {
                            if (prop.Equals("Left", StringComparison.OrdinalIgnoreCase))
                            {
                                Canvas.SetLeft(control, d);
                                return true;
                            }
                            if (prop.Equals("Top", StringComparison.OrdinalIgnoreCase))
                            {
                                Canvas.SetTop(control, d);
                                return true;
                            }
                            if (prop.Equals("Right", StringComparison.OrdinalIgnoreCase))
                            {
                                Canvas.SetRight(control, d);
                                return true;
                            }
                            if (prop.Equals("Bottom", StringComparison.OrdinalIgnoreCase))
                            {
                                Canvas.SetBottom(control, d);
                                return true;
                            }
                        }
                        if (int.TryParse(value, out var z) && prop.Equals("ZIndex", StringComparison.OrdinalIgnoreCase))
                        {
                            // ZIndex está en Control directamente
                            control.ZIndex = z;
                            return true;
                        }
                        error = $"Propiedad Canvas.{prop} no soportada o valor inválido";
                        return false;
                    }

                    // DockPanel.Dock
                    if (container.Equals("DockPanel", StringComparison.OrdinalIgnoreCase))
                    {
                        if (prop.Equals("Dock", StringComparison.OrdinalIgnoreCase))
                        {
                            if (Enum.TryParse<Dock>(value, true, out var dock))
                            {
                                DockPanel.SetDock(control, dock);
                                return true;
                            }
                        }
                        error = $"Propiedad DockPanel.{prop} no soportada o valor inválido";
                        return false;
                    }

                    error = $"Contenedor '{container}' no soportado";
                    return false;
                }

                error = "Propiedad adjunta no reconocida";
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

// ============================================================================
// EJEMPLOS DE USO CON DIFERENTES CONTENEDORES
// ============================================================================

/*
═══════════════════════════════════════════════════════════════════════════
EJEMPLO 1: Canvas (Posicionamiento Absoluto)
═══════════════════════════════════════════════════════════════════════════
# Usando atajos
TextBlock Text="Atajo X,Y"; X=10; Y=20

# Usando sintaxis completa
TextBlock Text="Sintaxis completa"; Canvas.Left=100; Canvas.Top=50

# Con ZIndex (orden de capas)
Border Background=Red; X=50; Y=50; Width=100; Height=100; ZIndex=1
Border Background=Blue; X=70; Y=70; Width=100; Height=100; ZIndex=2
# El azul aparece encima del rojo


═══════════════════════════════════════════════════════════════════════════
EJEMPLO 2: Grid (Filas y Columnas)
═══════════════════════════════════════════════════════════════════════════
# En C# primero defines el Grid:
var grid = new Grid();
grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

# Luego en el archivo:
TextBlock Text="Fila 0, Columna 0"; Row=0; Column=0
TextBlock Text="Fila 0, Columna 1"; Row=0; Column=1
TextBlock Text="Ocupa 2 columnas"; Row=1; Column=0; ColumnSpan=2

# O con sintaxis explícita:
Button Text="Click"; Grid.Row=1; Grid.Column=1


═══════════════════════════════════════════════════════════════════════════
EJEMPLO 3: DockPanel (Anclaje)
═══════════════════════════════════════════════════════════════════════════
# En C# defines el DockPanel:
var dockPanel = new DockPanel();

# Luego en el archivo:
Menu Dock=Top; Height=30
StatusBar Dock=Bottom; Height=25
TreeView Dock=Left; Width=200
TextBox (sin Dock, rellena el espacio restante)

# O con sintaxis explícita:
Button Text="Izquierda"; DockPanel.Dock=Left


═══════════════════════════════════════════════════════════════════════════
EJEMPLO 4: Combinando Contenedores
═══════════════════════════════════════════════════════════════════════════
# Canvas con DockPanel dentro
var canvas = new Canvas();
var dockPanel = new DockPanel();
Canvas.SetLeft(dockPanel, 50);
Canvas.SetTop(dockPanel, 50);
canvas.Children.Add(dockPanel);

# Archivo para el DockPanel:
Menu Dock=Top; Height=30; Background=#2196F3
Button Text="Contenido"; Background=#F5F5F5


═══════════════════════════════════════════════════════════════════════════
COMPARACIÓN DE CONTENEDORES
═══════════════════════════════════════════════════════════════════════════

┌─────────────┬──────────────────┬────────────────────────────────┐
│ Contenedor  │ Uso típico       │ Propiedades                    │
├─────────────┼──────────────────┼────────────────────────────────┤
│ Canvas      │ Juegos, gráficos │ X, Y, ZIndex                   │
│             │ Animaciones      │ Canvas.Left, Canvas.Top        │
├─────────────┼──────────────────┼────────────────────────────────┤
│ Grid        │ Formularios      │ Row, Column                    │
│             │ Layouts complejos│ RowSpan, ColumnSpan            │
├─────────────┼──────────────────┼────────────────────────────────┤
│ StackPanel  │ Listas verticales│ (posición automática)          │
│             │ Menús horizontales│                               │
├─────────────┼──────────────────┼────────────────────────────────┤
│ DockPanel   │ App layout       │ Dock (Top/Bottom/Left/Right)   │
│             │ IDE-style        │                                │
├─────────────┼──────────────────┼────────────────────────────────┤
│ WrapPanel   │ Tags, badges     │ (posición automática con wrap) │
├─────────────┼──────────────────┼────────────────────────────────┤
│ UniformGrid │ Calculadoras     │ (posición automática uniforme) │
│             │ Grids de botones │                                │
└─────────────┴──────────────────┴────────────────────────────────┘


═══════════════════════════════════════════════════════════════════════════
CUÁNDO USAR CADA UNO
═══════════════════════════════════════════════════════════════════════════

✅ USA CANVAS cuando:
   - Necesitas posicionamiento pixel-perfect
   - Haces animaciones
   - Creas juegos o gráficos
   - Control total sobre la posición

✅ USA GRID cuando:
   - Tienes formularios estructurados
   - Necesitas filas y columnas
   - Layouts responsive
   - Interfaces tipo aplicación

✅ USA STACKPANEL cuando:
   - Lista simple de elementos
   - Menú de botones
   - Listas verticales/horizontales

✅ USA DOCKPANEL cuando:
   - Layout estilo IDE
   - Menú arriba, contenido centro, status abajo
   - Paneles laterales

✅ USA WRAPPANEL cuando:
   - Tags que se envuelven
   - Badges o chips
   - Contenido que fluye

✅ USA UNIFORMGRID cuando:
   - Todos los elementos mismo tamaño
   - Calculadora
   - Grid de botones uniforme
*/