// ============================================================================
// ARCHIVO: Core/DynamicUIBuilderV3.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicUI.Parsing;
using DynamicUI.Parsing.Advanced;
using DynamicUI.Controls;
using DynamicUI.Setters;
using DynamicUI.Binding;
using DynamicUI.Logging;
using DynamicUI.Validation;
using DynamicUI.Conversion;
using DynamicUI.Plugins.Advanced;
using DynamicUI.Animation;

namespace DynamicUI.V3
{
    /// <summary>
    /// DynamicUIBuilder versión 3.0 - Integración completa de todos los sistemas
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
            // Registrar controles base
            ControlRegistry.Register("TextBlock", typeof(TextBlock));
            ControlRegistry.Register("Label", typeof(TextBlock));
            ControlRegistry.Register("TextBox", typeof(TextBox));
            ControlRegistry.Register("Image", typeof(Image));
            ControlRegistry.Register("ComboBox", typeof(ComboBox));
            ControlRegistry.Register("CheckBox", typeof(CheckBox));
            ControlRegistry.Register("Button", typeof(Button));
            ControlRegistry.Register("Border", typeof(Border));
            ControlRegistry.Register("Rectangle", typeof(Border));

            // Registrar setters (orden importante: más específicos primero)
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
        /// Construye la UI desde un archivo (auto-detecta formato)
        /// </summary>
        public void BuildFromFile(Canvas canvas, string filePath)
        {
            BuildFromFileAsync(canvas, filePath, CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Construye la UI desde un archivo de forma asíncrona
        /// </summary>
        public async Task BuildFromFileAsync(Canvas canvas, string filePath, CancellationToken ct = default)
        {
            if (canvas == null) 
                throw new ArgumentNullException(nameof(canvas));

            _logger.LogInfo($"=== Iniciando construcción de UI ===");
            _logger.LogInfo($"Archivo: {filePath}");
            var startTime = DateTime.UtcNow;

            try
            {
                // Parsear archivo (auto-detecta formato: TXT, JSON, XML, YAML)
                IEnumerable<ControlDescriptor> descriptors;
                
                try
                {
                    descriptors = await Task.Run(() => _multiFormatParser.ParseFile(filePath), ct);
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
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        var control = await CreateControlAsync(desc, ct);
                        if (control != null)
                        {
                            canvas.Children.Add(control);
                            successCount++;
                            _logger.LogDebug($"✓ Control '{desc.TypeName}' creado exitosamente");
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

        private async Task<Control> CreateControlAsync(ControlDescriptor desc, CancellationToken ct)
        {
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
                    groupContext = await Task.Run(() => DataContextResolver(desc.GroupName), ct);
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
                ct.ThrowIfCancellationRequested();

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
                bool applied = await ApplyPropertyAsync(control, prop.Name, prop.Value, ct);
                
                if (!applied)
                {
                    _logger.LogWarning($"No se pudo aplicar propiedad '{prop.Name}' en '{desc.TypeName}'");
                }
            }

            // 4. Asignar DataContext final
            control.DataContext = specificDC ?? groupContext;

            return control;
        }

        private async Task<bool> ApplyPropertyAsync(Control control, string propertyName, string value, CancellationToken ct)
        {
            foreach (var setter in _setters)
            {
                ct.ThrowIfCancellationRequested();

                if (!setter.CanHandle(control, propertyName))
                    continue;

                try
                {
                    // Si el setter soporta async, usarlo
                    if (setter is IAsyncPropertySetter asyncSetter)
                    {
                        return await asyncSetter.ApplyAsync(control, propertyName, value, ct);
                    }
                    else
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
                object converted = _converter.ConvertValue(value, prop.PropertyType);
                prop.SetValue(control, converted);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _logger.LogError($"Error convirtiendo '{value}' a {prop.PropertyType.Name}: {ex.Message}");
                return false;
            }
        }
    }
}