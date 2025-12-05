// ============================================================================
// ARCHIVO: Controls/ControlAutoRegistration.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DynamicUI.Controls
{
    /// <summary>
    /// Sistema de auto-registro de controles de Avalonia
    /// </summary>
    public static class ControlAutoRegistration
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Registra automáticamente todos los controles básicos de Avalonia
        /// </summary>
        public static void RegisterAllAvaloniaControls()
        {
            lock (_lock)
            {
                if (_initialized) return;

                Console.WriteLine("🔄 Auto-registrando controles de Avalonia...");

                var controlTypes = new Dictionary<string, Type>
                {
                    // Controles básicos
                    {"TextBlock", typeof(TextBlock)},
                    {"Label", typeof(Label)},
                    {"TextBox", typeof(TextBox)},
                    {"Button", typeof(Button)},
                    {"CheckBox", typeof(CheckBox)},
                    {"RadioButton", typeof(RadioButton)},
                    {"ToggleButton", typeof(ToggleButton)},
                    
                    // Controles de selección
                    {"ComboBox", typeof(ComboBox)},
                    {"ListBox", typeof(ListBox)},
                    {"Slider", typeof(Slider)},
                    
                    // Contenedores
                    {"Border", typeof(Border)},
                    {"Canvas", typeof(Canvas)},
                    {"StackPanel", typeof(StackPanel)},
                    {"Grid", typeof(Grid)},
                    {"DockPanel", typeof(DockPanel)},
                    {"WrapPanel", typeof(WrapPanel)},
                    {"Panel", typeof(Panel)},
                    
                    // Controles con pestañas/items
                    {"TabControl", typeof(TabControl)},
                    {"TabItem", typeof(TabItem)},
                    
                    // Scroll y Viewport
                    {"ScrollViewer", typeof(ScrollViewer)},
                    {"ScrollBar", typeof(ScrollBar)},
                    {"Viewbox", typeof(Viewbox)},
                    
                    // Media
                    {"Image", typeof(Image)},
                    
                    // Shapes (si los necesitas)
                    {"Rectangle", typeof(Border)}, // Alias
                    
                    // Otros
                    {"ProgressBar", typeof(ProgressBar)},
                    {"Separator", typeof(Separator)},
                    {"GridSplitter", typeof(GridSplitter)},
                    {"Expander", typeof(Expander)},
                };

                int count = 0;
                foreach (var kvp in controlTypes)
                {
                    try
                    {
                        ControlRegistry.Register(kvp.Key, kvp.Value);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠ No se pudo registrar {kvp.Key}: {ex.Message}");
                    }
                }

                Console.WriteLine($"✅ {count} controles registrados automáticamente");
                _initialized = true;
            }
        }

        /// <summary>
        /// Registra automáticamente todos los controles que hereden de Control
        /// en el ensamblado de Avalonia.Controls
        /// </summary>
        public static void RegisterAllAvaloniaControlsReflection()
        {
            lock (_lock)
            {
                if (_initialized) return;

                Console.WriteLine("🔄 Escaneando controles de Avalonia con reflexión...");

                try
                {
                    var avaloniaAssembly = typeof(Control).Assembly;
                    var controlTypes = avaloniaAssembly.GetTypes()
                        .Where(t => typeof(Control).IsAssignableFrom(t) 
                                 && !t.IsAbstract 
                                 && t.IsPublic
                                 && t.GetConstructor(Type.EmptyTypes) != null);

                    int count = 0;
                    foreach (var type in controlTypes)
                    {
                        try
                        {
                            ControlRegistry.Register(type.Name, type);
                            count++;
                        }
                        catch
                        {
                            // Ignorar si ya está registrado
                        }
                    }

                    Console.WriteLine($"✅ {count} controles descubiertos y registrados");
                    _initialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en auto-registro por reflexión: {ex.Message}");
                    // Fallback al método manual
                    RegisterAllAvaloniaControls();
                }
            }
        }

        /// <summary>
        /// Intenta auto-registrar un control por nombre si no existe
        /// </summary>
        public static bool TryAutoRegister(string controlName)
        {
            // Si ya está registrado, no hacer nada
            if (ControlRegistry.TryGet(controlName, out _))
                return true;

            Console.WriteLine($"🔍 Intentando auto-registrar: {controlName}");

            // Buscar en el namespace de Avalonia.Controls
            var avaloniaNamespace = "Avalonia.Controls";
            var fullTypeName = $"{avaloniaNamespace}.{controlName}";

            try
            {
                var type = Type.GetType($"{fullTypeName}, Avalonia.Controls");
                
                if (type == null)
                {
                    // Intentar en Avalonia.Controls.Primitives
                    type = Type.GetType($"Avalonia.Controls.Primitives.{controlName}, Avalonia.Controls");
                }

                if (type != null && typeof(Control).IsAssignableFrom(type))
                {
                    ControlRegistry.Register(controlName, type);
                    Console.WriteLine($"✅ Auto-registrado: {controlName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ No se pudo auto-registrar {controlName}: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Resetea el estado de inicialización (útil para tests)
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _initialized = false;
            }
        }
    }
}