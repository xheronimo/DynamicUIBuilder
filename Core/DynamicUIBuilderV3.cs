// ============================================================================
// ARCHIVO: Core/DynamicUIBuilderV3.cs
// ============================================================================
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using DynamicUI.Animation;
using DynamicUI.Binding;
using DynamicUI.Controls;
using DynamicUI.Conversion;
using DynamicUI.Logging;
using DynamicUI.Logging.Targets;
using DynamicUI.Parsing;
using DynamicUI.Parsing.Advanced;
using DynamicUI.Plugins.Advanced;
using DynamicUI.Setters;
using DynamicUI.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicUI.V3
{
    /// <summary>
    /// DynamicUIBuilder versión 3.0 - Integración completa de todos los sistemas
    /// Soporta: TXT, JSON, XML, YAML
    /// Auto-registro de controles
    /// Carga recursiva de archivos
    /// </summary>
    public class DynamicUIBuilderV3
    {
        private readonly List<IPropertySetter> _setters = new();
        private readonly List<IPropertyValidator> _validators = new();
        private readonly List<IValueConverter> _converters = new();

        private readonly IUILogger _logger;
        private readonly PropertyValidationEngine _validationEngine;
        private readonly TypeConversionEngine _conversionEngine;
        private readonly MultiFormatParser _multiFormatParser;
        private readonly AdvancedPluginManager _pluginManager;
        private readonly AnimationEngine _animationEngine;

        public Func<string, object> DataContextResolver { get; set; }
        public bool StopOnError { get; set; } = false;
        public bool ValidateProperties { get; set; } = true;
        public bool EnableAnimations { get; set; } = true;

        public AdvancedPluginManager PluginManager => _pluginManager;
        public IUILogger Logger => _logger;

        public DynamicUIBuilderV3(IUILogger logger = null)
        {
            _logger = logger ?? CreateDefaultLogger();
            _validationEngine = new PropertyValidationEngine();
            _conversionEngine = new TypeConversionEngine();
            _multiFormatParser = new MultiFormatParser();
            _pluginManager = new AdvancedPluginManager(this, _logger);
            _animationEngine = new AnimationEngine(_logger);

            RegisterDefaultComponents();
        }

        private IUILogger CreateDefaultLogger()
        {
            var logger = new UILogger();
            logger.AddTarget(new ConsoleLogTarget());
            logger.SetMinLevel(LogLevel.Info);
            return logger;
        }

        private void RegisterDefaultComponents()
        {
            // AUTO-REGISTRAR todos los controles de Avalonia
            ControlAutoRegistration.RegisterAllAvaloniaControls();

            // Registrar setters (orden importante: más específicos primero)
            _setters.Add(new ClassesSetter());
            _setters.Add(new GridDefinitionSetter());
            _setters.Add(new ToolTipSetter());
            _setters.Add(new ImageSetter());
            _setters.Add(new AttachedPropertySetter());
            if (EnableAnimations)
            {
                _setters.Add(new AnimationSetter(_animationEngine, _logger));
            }
            _setters.Add(new ImprovedDefaultPropertySetter(_conversionEngine, _logger));

           


            _logger.LogInfo("DynamicUIBuilder v3.0 inicializado");
        }

        /// <summary>
        /// Registra un setter de propiedades personalizado
        /// </summary>
        public void RegisterSetter(IPropertySetter setter)
        {
            if (setter == null)
                throw new ArgumentNullException(nameof(setter));

            _setters.Insert(0, setter);
            _logger.LogDebug($"Setter registrado: {setter.GetType().Name}");
        }

        /// <summary>
        /// Registra un validador de propiedades
        /// </summary>
        public void RegisterValidator(IPropertyValidator validator)
        {
            _validationEngine.RegisterValidator(validator);
            _validators.Add(validator);
            _logger.LogDebug($"Validator registrado: {validator.GetType().Name}");
        }

        /// <summary>
        /// Registra un conversor de tipos
        /// </summary>
        public void RegisterConverter(IValueConverter converter)
        {
            _conversionEngine.RegisterConverter(converter);
            _converters.Add(converter);
            _logger.LogDebug($"Converter registrado: {converter.GetType().Name}");
        }

        /// <summary>
        /// Desregistra un setter
        /// </summary>
        public void UnregisterSetter(IPropertySetter setter)
        {
            _setters.Remove(setter);
            _logger.LogDebug($"Setter desregistrado: {setter.GetType().Name}");
        }

        /// <summary>
        /// Desregistra un validador
        /// </summary>
        public void UnregisterValidator(IPropertyValidator validator)
        {
            _validators.Remove(validator);
            _logger.LogDebug($"Validator desregistrado: {validator.GetType().Name}");
        }

        /// <summary>
        /// Desregistra un conversor
        /// </summary>
        public void UnregisterConverter(IValueConverter converter)
        {
            _converters.Remove(converter);
            _logger.LogDebug($"Converter desregistrado: {converter.GetType().Name}");
        }

        /// <summary>
        /// Construye la UI desde un archivo en cualquier contenedor
        /// Soporta: Panel, ContentControl, ItemsControl, Decorator, Window
        /// Auto-detecta formato: TXT, JSON, XML, YAML
        /// </summary>
        public void BuildFromFile(Control container, string filePath)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _logger.LogInfo($"=== Iniciando construcción de UI ===");
            _logger.LogInfo($"Archivo: {filePath}");
            _logger.LogInfo($"Contenedor: {container.GetType().Name}");
            var startTime = DateTime.UtcNow;

            try
            {
                // Parsear archivo (auto-detecta formato)
                IEnumerable<ControlDescriptor> descriptors;

                try
                {
                    descriptors = _multiFormatParser.ParseFile(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Error al parsear archivo: {ex.Message}", ex);
                    throw;
                }

                int successCount = 0;
                int errorCount = 0;
                var descriptorList = descriptors.ToList();

                _logger.LogInfo($"Controles a crear: {descriptorList.Count}");

                foreach (var desc in descriptorList)
                {
                    try
                    {
                        var control = CreateControl(desc);
                        if (control != null)
                        {
                            if (AddControlToContainer(container, control))
                            {
                                successCount++;
                                _logger.LogDebug($"✓ Control '{desc.TypeName}' creado y agregado");
                            }
                            else
                            {
                                errorCount++;
                                _logger.LogError($"✗ No se pudo agregar '{desc.TypeName}' al contenedor");
                            }
                        }
                        else
                        {
                            errorCount++;
                            _logger.LogError($"✗ Control '{desc.TypeName}' falló en la creación");

                            if (StopOnError)
                            {
                                _logger.LogError("Deteniendo construcción debido a StopOnError=true");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError($"✗ Error creando control '{desc.TypeName}': {ex.Message}", ex);

                        if (StopOnError)
                        {
                            _logger.LogError("Deteniendo construcción debido a StopOnError=true");
                            throw;
                        }
                    }
                }

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInfo($"=== Construcción completada ===");
                _logger.LogInfo($"Tiempo: {duration.TotalMilliseconds:F2}ms");
                _logger.LogInfo($"Exitosos: {successCount}");
                _logger.LogInfo($"Errores: {errorCount}");
                _logger.LogInfo($"Total: {descriptorList.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error crítico durante construcción: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Sobrecarga para mantener compatibilidad con Canvas
        /// </summary>
        public void BuildFromFile(Canvas canvas, string filePath)
        {
            BuildFromFile((Control)canvas, filePath);
        }

        /// <summary>
        /// Agrega un control hijo a un contenedor, detectando automáticamente el tipo
        /// </summary>
        private bool AddControlToContainer(Control container, Control child)
        {
            try
            {
                // Panel (Canvas, StackPanel, Grid, DockPanel, WrapPanel, etc.)
                if (container is Panel panel)
                {
                    panel.Children.Add(child);
                    _logger.LogDebug($"  → Agregado a Panel.Children");
                    return true;
                }

                // ContentControl (Button, Border, ScrollViewer, TabItem, Window, etc.)
                if (container is ContentControl contentControl)
                {
                    if (contentControl.Content != null)
                    {
                        _logger.LogWarning($"  ⚠ ContentControl ya tiene contenido, creando StackPanel contenedor");

                        var existingContent = contentControl.Content;
                        var stackPanel = new StackPanel();

                        if (existingContent is Control existingControl)
                        {
                            stackPanel.Children.Add(existingControl);
                        }

                        stackPanel.Children.Add(child);
                        contentControl.Content = stackPanel;
                    }
                    else
                    {
                        contentControl.Content = child;
                    }

                    _logger.LogDebug($"  → Agregado a ContentControl.Content");
                    return true;
                }

                // ItemsControl (ListBox, ComboBox, TabControl, etc.)
                if (container is ItemsControl itemsControl)
                {
                    itemsControl.Items.Add(child);
                    _logger.LogDebug($"  → Agregado a ItemsControl.Items");
                    return true;
                }

                // Decorator (Border, Viewbox, etc.)
                if (container is Decorator decorator)
                {
                    if (decorator.Child != null)
                    {
                        _logger.LogWarning($"  ⚠ Decorator ya tiene hijo, creando StackPanel contenedor");

                        var existingChild = decorator.Child;
                        var stackPanel = new StackPanel();

                        if (existingChild != null)
                        {
                            stackPanel.Children.Add(existingChild);
                        }

                        stackPanel.Children.Add(child);
                        decorator.Child = stackPanel;
                    }
                    else
                    {
                        decorator.Child = child;
                    }

                    _logger.LogDebug($"  → Agregado a Decorator.Child");
                    return true;
                }

                // Window (caso especial)
                if (container is Window window)
                {
                    if (window.Content != null)
                    {
                        _logger.LogWarning($"  ⚠ Window ya tiene contenido, creando StackPanel contenedor");

                        var existingContent = window.Content;
                        var stackPanel = new StackPanel();

                        if (existingContent is Control existingControl)
                        {
                            stackPanel.Children.Add(existingControl);
                        }

                        stackPanel.Children.Add(child);
                        window.Content = stackPanel;
                    }
                    else
                    {
                        window.Content = child;
                    }

                    _logger.LogDebug($"  → Agregado a Window.Content");
                    return true;
                }

                _logger.LogError($"  ✗ Tipo de contenedor no soportado: {container.GetType().Name}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"  ✗ Error agregando control: {ex.Message}");
                return false;
            }
        }

        private Control CreateControl(ControlDescriptor desc)
        {
            if (!string.IsNullOrEmpty(desc.SourceFile))
            {
                _logger.LogInfo($"Procesando fichero: {desc.SourceFile}");
            }
            // 1. Crear control
            var control = ControlFactory.Create(desc.TypeName);
            if (control == null)
            {
                _logger.LogError($"Tipo de control '{desc.TypeName}' no está registrado");
                return null;
            }

            // 2. Resolver DataContext del grupo
            object groupContext = null;
            if (!string.IsNullOrEmpty(desc.GroupName) && DataContextResolver != null)
            {
                try
                {
                    groupContext = DataContextResolver(desc.GroupName);
                    _logger.LogDebug($"DataContext resuelto para grupo '{desc.GroupName}'");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error resolviendo DataContext para grupo '{desc.GroupName}': {ex.Message}");
                }
            }

            object specificDC = null;

            // 3. Aplicar propiedades
            foreach (var prop in desc.Properties)
            {
                if (string.IsNullOrWhiteSpace(prop.Name))
                    continue;

                if (prop.Value == null)
                {
                    _logger.LogWarning($"Propiedad '{prop.Name}' no tiene valor");
                    continue;
                }

                // DataContext explícito
                if (prop.Name.Equals("DataContext", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        specificDC = DataContextResolver?.Invoke(prop.Value);
                        _logger.LogDebug($"DataContext específico resuelto: '{prop.Value}'");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error resolviendo DataContext '{prop.Value}': {ex.Message}", ex);
                    }
                    continue;
                }

                // Binding: Binding:PropertyName=Path
                if (prop.Name.StartsWith("Binding:", StringComparison.OrdinalIgnoreCase))
                {
                    var propName = prop.Name.Substring(8);
                    try
                    {
                        BindingApplier.ApplyBinding(control, propName, prop.Value);
                        _logger.LogDebug($"Binding aplicado: {propName} -> {prop.Value}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error aplicando binding {propName}: {ex.Message}", ex);
                    }
                    continue;
                }

                // Validar propiedad
                if (ValidateProperties)
                {
                    var validationResult = _validationEngine.Validate(control, prop.Name, prop.Value);

                    if (!validationResult.IsValid)
                    {
                        foreach (var error in validationResult.Errors)
                        {
                            _logger.LogError($"Validación fallida [{desc.TypeName}.{prop.Name}]: {error}");
                        }

                        if (StopOnError)
                        {
                            throw new ValidationException(
                                $"Validación fallida en {desc.TypeName}.{prop.Name}: {string.Join(", ", validationResult.Errors)}");
                        }

                        continue;
                    }

                    foreach (var warning in validationResult.Warnings)
                    {
                        _logger.LogWarning($"Advertencia [{desc.TypeName}.{prop.Name}]: {warning}");
                    }
                }

                // Aplicar propiedad
                bool applied = ApplyProperty(control, prop.Name, prop.Value);

                if (!applied)
                {
                    _logger.LogWarning($"No se pudo aplicar propiedad '{prop.Name}' en '{desc.TypeName}'");
                }
            }

            // 4. Asignar DataContext final
            control.DataContext = specificDC ?? groupContext;

            // 5. Procesar controles hijos
            if (desc.Children != null && desc.Children.Count > 0)
            {
                ProcessChildren(control, desc.Children);
            }

            return control;
        }

        /// <summary>
        /// Procesa y agrega controles hijos a cualquier tipo de contenedor
        /// </summary>
        private void ProcessChildren(Control parent, List<ControlDescriptor> children)
        {
            _logger.LogDebug($"Procesando {children.Count} controles hijos para {parent.GetType().Name}");

            if (children == null || children.Count == 0)
                return;

            // Panel (Canvas, StackPanel, Grid, DockPanel, WrapPanel, etc.)
            if (parent is Panel panel)
            {
                foreach (var childDesc in children)
                {
                    try
                    {
                        var child = CreateControl(childDesc);
                        if (child != null)
                        {
                            panel.Children.Add(child);
                            _logger.LogDebug($"  ✓ Hijo '{childDesc.TypeName}' agregado a Panel");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"  ✗ Error creando hijo '{childDesc.TypeName}': {ex.Message}", ex);
                        if (StopOnError) throw;
                    }
                }
                return;
            }

            // ContentControl (Button, TabItem, GroupBox, ScrollViewer, etc.)
            if (parent is ContentControl contentControl)
            {
                if (children.Count == 1)
                {
                    try
                    {
                        var child = CreateControl(children[0]);
                        if (child != null)
                        {
                            contentControl.Content = child;
                            _logger.LogDebug($"  ✓ Hijo '{children[0].TypeName}' asignado a Content");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"  ✗ Error creando hijo: {ex.Message}", ex);
                        if (StopOnError) throw;
                    }
                }
                else
                {
                    var container = new StackPanel();

                    foreach (var childDesc in children)
                    {
                        try
                        {
                            var child = CreateControl(childDesc);
                            if (child != null)
                            {
                                container.Children.Add(child);
                                _logger.LogDebug($"  ✓ Hijo '{childDesc.TypeName}' agregado a contenedor");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"  ✗ Error creando hijo '{childDesc.TypeName}': {ex.Message}", ex);
                            if (StopOnError) throw;
                        }
                    }

                    contentControl.Content = container;
                    _logger.LogDebug($"  ✓ StackPanel contenedor creado con {container.Children.Count} hijos");
                }
                return;
            }

            // ItemsControl (ListBox, ComboBox, TabControl, etc.)
            if (parent is ItemsControl itemsControl)
            {
                foreach (var childDesc in children)
                {
                    try
                    {
                        var child = CreateControl(childDesc);
                        if (child != null)
                        {
                            itemsControl.Items.Add(child);
                            _logger.LogDebug($"  ✓ Hijo '{childDesc.TypeName}' agregado a Items");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"  ✗ Error creando hijo '{childDesc.TypeName}': {ex.Message}", ex);
                        if (StopOnError) throw;
                    }
                }
                return;
            }

            // Decorator (Border, Viewbox, etc.)
            if (parent is Decorator decorator)
            {
                if (children.Count == 1)
                {
                    try
                    {
                        var child = CreateControl(children[0]);
                        if (child != null)
                        {
                            decorator.Child = child;
                            _logger.LogDebug($"  ✓ Hijo '{children[0].TypeName}' asignado a Child");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"  ✗ Error creando hijo: {ex.Message}", ex);
                        if (StopOnError) throw;
                    }
                }
                else
                {
                    var container = new StackPanel();

                    foreach (var childDesc in children)
                    {
                        try
                        {
                            var child = CreateControl(childDesc);
                            if (child != null)
                            {
                                container.Children.Add(child);
                                _logger.LogDebug($"  ✓ Hijo '{childDesc.TypeName}' agregado a contenedor");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"  ✗ Error creando hijo '{childDesc.TypeName}': {ex.Message}", ex);
                            if (StopOnError) throw;
                        }
                    }

                    decorator.Child = container;
                    _logger.LogDebug($"  ✓ StackPanel contenedor creado con {container.Children.Count} hijos");
                }
                return;
            }

            _logger.LogWarning($"  ⚠ El control {parent.GetType().Name} no soporta hijos");
        }

        private bool ApplyProperty(Control control, string propertyName, string value)
        {
            foreach (var setter in _setters)
            {
                if (!setter.CanHandle(control, propertyName))
                    continue;

                try
                {
                    if (setter.Apply(control, propertyName, value, out var error))
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"Error aplicando {propertyName}: {error}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Excepción aplicando {propertyName}: {ex.Message}", ex);
                    return false;
                }
            }

            _logger.LogWarning($"Ningún setter pudo manejar la propiedad '{propertyName}'");
            return false;
        }

        /// <summary>
        /// Obtiene estadísticas de la última construcción
        /// </summary>
        public BuildStatistics GetStatistics()
        {
            var logs = _logger.GetLogs();
            int errorCount = 0;
            int warningCount = 0;
            int infoCount = 0;

            foreach (var log in logs)
            {
                if (log.Level == LogLevel.Error || log.Level == LogLevel.Critical)
                    errorCount++;
                else if (log.Level == LogLevel.Warning)
                    warningCount++;
                else if (log.Level == LogLevel.Info)
                    infoCount++;
            }

            return new BuildStatistics
            {
                ErrorCount = errorCount,
                WarningCount = warningCount,
                InfoCount = infoCount,
                TotalLogs = logs.Count(),
                LoadedPlugins = _pluginManager.GetAllPluginInfo(),
                RegisteredSetters = _setters.Count,
                RegisteredValidators = _validators.Count,
                RegisteredConverters = _converters.Count
            };
        }

        /// <summary>
        /// Imprime un reporte completo del sistema
        /// </summary>
        public void PrintSystemReport()
        {
            var stats = GetStatistics();

            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          DynamicUI v3.0 - System Report                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("📊 Build Statistics:");
            Console.WriteLine($"   Errors:   {stats.ErrorCount}");
            Console.WriteLine($"   Warnings: {stats.WarningCount}");
            Console.WriteLine($"   Info:     {stats.InfoCount}");
            Console.WriteLine($"   Total:    {stats.TotalLogs}");
            Console.WriteLine();
            Console.WriteLine("🔧 Registered Components:");
            Console.WriteLine($"   Setters:    {stats.RegisteredSetters}");
            Console.WriteLine($"   Validators: {stats.RegisteredValidators}");
            Console.WriteLine($"   Converters: {stats.RegisteredConverters}");
            Console.WriteLine();
            Console.WriteLine($"🔌 Loaded Plugins: {stats.LoadedPlugins.Count}");

            foreach (var plugin in stats.LoadedPlugins)
            {
                Console.WriteLine($"   ├─ {plugin.Name} v{plugin.Version}");
                Console.WriteLine($"   │  Controls: {plugin.ControlCount}, Setters: {plugin.SetterCount}");
            }

            Console.WriteLine();
            Console.WriteLine("✅ System Status: Operational");
            Console.WriteLine("══════════════════════════════════════════════════════════");
        }
    }

    /// <summary>
    /// Estadísticas de construcción
    /// </summary>
    public class BuildStatistics
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int InfoCount { get; set; }
        public int TotalLogs { get; set; }
        public List<PluginInfo> LoadedPlugins { get; set; } = new();
        public int RegisteredSetters { get; set; }
        public int RegisteredValidators { get; set; }
        public int RegisteredConverters { get; set; }
    }

    /// <summary>
    /// Excepción de validación
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Setter mejorado que usa el TypeConverter
    /// </summary>
    public class ImprovedDefaultPropertySetter : IPropertySetter
    {
        private readonly TypeConversionEngine _converter;
        private readonly IUILogger _logger;

        public ImprovedDefaultPropertySetter(TypeConversionEngine converter, IUILogger logger)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool CanHandle(Control control, string propertyName) => true; // Fallback

        public bool Apply(Control control, string propertyName, string value, out string error)
        {
            error = null;
            var prop = CachedReflection.GetProperty(control.GetType(), propertyName);

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
                object converted;

                // Caso especial: object → asignar directamente el string
                if (prop.PropertyType == typeof(object))
                {
                    converted = value;
                }
                // Caso especial: string → asignar directamente
                else if (prop.PropertyType == typeof(string))
                {
                    converted = value;
                }
                // Caso especial: enum → parsear
                else if (prop.PropertyType.IsEnum)
                {
                    converted = Enum.Parse(prop.PropertyType, value, ignoreCase: true);
                }
                else
                {
                    // Usar el motor de conversión para otros tipos (int, double, etc.)
                    converted = _converter.ConvertValue(value, prop.PropertyType);
                }

                prop.SetValue(control, converted);
                return true;
            }

            catch (Exception ex)
            {
                // Fallback: si falla la conversión, asignar directamente el string
                try
                {
                    prop.SetValue(control, value);
                    return true;
                }
                catch
                {
                    error = ex.Message;
                    _logger.LogError($"Error convirtiendo '{value}' a {prop.PropertyType.Name}: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
