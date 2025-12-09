// ============================================================================
// ARCHIVO: Parsing/TextParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicUI.Parsing
{
    /// <summary>
    /// Parser para archivos de texto (.txt) con sintaxis personalizada
    /// Soporta: defaults, grupos, propiedades, carga de archivos externos
    /// </summary>
    public static class TextParser
    {
        /// <summary>
        /// Parsea un archivo de texto y retorna descriptores de controles
        /// </summary>
        /// <param name="rutaArchivo">Ruta al archivo .txt</param>
        /// <param name="groupNormalizer">Función opcional para normalizar nombres de grupos</param>
        /// <returns>Lista de descriptores de controles</returns>
        public static IEnumerable<ControlDescriptor> ParseFile(string rutaArchivo, Func<string, string> groupNormalizer = null)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException($"Archivo no encontrado: {rutaArchivo}");

            var baseDirectory = Path.GetDirectoryName(rutaArchivo) ?? ".";
            var results = new List<ControlDescriptor>();
            string currentControlType = null;
            string currentGroup = null;
            var defaults = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            int lineno = 0;

            // Pila para manejar jerarquía por indentación
            var stack = new Stack<(int indent, ControlDescriptor desc)>();

            foreach (var raw in File.ReadLines(rutaArchivo))
            {
                lineno++;
                if (string.IsNullOrWhiteSpace(raw)) continue;

                // Contar indentación antes de hacer Trim
                int indent = raw.TakeWhile(Char.IsWhiteSpace).Count();
                var linea = raw.Trim();

                // Ignorar comentarios
                if (linea.StartsWith("#")) continue;

                try
                {
                    // Grupo=NombreGrupo
                    if (linea.StartsWith("Grupo=", StringComparison.OrdinalIgnoreCase))
                    {
                        currentGroup = linea.Substring(6).Trim().Trim('\"');
                        if (groupNormalizer != null)
                            currentGroup = groupNormalizer(currentGroup);
                        continue;
                    }

                    // Control=TipoControl (define defaults)
                    if (linea.StartsWith("Control=", StringComparison.OrdinalIgnoreCase))
                    {
                        currentControlType = linea.Substring(8).Trim();
                        if (!defaults.ContainsKey(currentControlType))
                            defaults[currentControlType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        continue;
                    }

                    // Defaults dentro de Control=
                    if (currentControlType != null && linea.Contains("=") && !linea.Contains(";"))
                    {
                        var pe = PropertyParser.ParsearPropiedad(linea);
                        if (pe.Value != null)
                            defaults[currentControlType][pe.Name] = pe.Value;
                        continue;
                    }

                    // Línea de control normal
                    var fragmentos = LineParser.ParsearLinea(linea);
                    if (fragmentos.Count == 0) continue;

                    string tipo = currentControlType;
                    var props = new List<PropertyEntry>();
                    var primer = fragmentos[0];

                    if (!primer.Contains("="))
                    {
                        tipo = primer;
                        for (int i = 1; i < fragmentos.Count; i++)
                            props.Add(PropertyParser.ParsearPropiedad(fragmentos[i]));
                    }
                    else
                    {
                        foreach (var f in fragmentos)
                            props.Add(PropertyParser.ParsearPropiedad(f));
                    }

                    // Cargar propiedades externas
                    var fileProps = props.Where(p =>
                        p.Name.Equals("PropertiesFile", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("LoadProps", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("PropsFile", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var fileProp in fileProps)
                    {
                        props.Remove(fileProp);
                        var externalFile = ResolveFilePath(fileProp.Value, baseDirectory);

                        if (File.Exists(externalFile))
                        {
                            var externalProps = LoadPropertiesFromFile(externalFile);
                            var existingNames = new HashSet<string>(
                                props.Select(p => p.Name),
                                StringComparer.OrdinalIgnoreCase);

                            foreach (var extProp in externalProps)
                            {
                                if (!existingNames.Contains(extProp.Name))
                                    props.Add(extProp);
                            }

                            Console.WriteLine($"  ✓ Propiedades cargadas desde: {Path.GetFileName(externalFile)}");
                        }
                        else
                        {
                            Console.WriteLine($"  ⚠ Archivo de propiedades no encontrado: {externalFile}");
                        }
                    }

                    // Cargar hijos externos
                    var childrenProps = props.Where(p =>
                        p.Name.Equals("Children", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("ChildrenFile", StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals("LoadChildren", StringComparison.OrdinalIgnoreCase) ||
                        (p.Name.Equals("Content", StringComparison.OrdinalIgnoreCase) &&
                         p.Value != null && p.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    List<ControlDescriptor> children = null;

                    foreach (var childProp in childrenProps)
                    {
                        props.Remove(childProp);
                        var childrenFile = ResolveFilePath(childProp.Value, baseDirectory);

                        if (File.Exists(childrenFile))
                        {
                            var loadedChildren = ParseFile(childrenFile, groupNormalizer).ToList();

                            if (children == null)
                                children = new List<ControlDescriptor>();

                            children.AddRange(loadedChildren);

                            Console.WriteLine($"  ✓ {loadedChildren.Count} controles hijos cargados desde: {Path.GetFileName(childrenFile)}");
                        }
                        else
                        {
                            Console.WriteLine($"  ⚠ Archivo de controles hijos no encontrado: {childrenFile}");
                        }
                    }

                    // Aplicar defaults
                    if (!string.IsNullOrEmpty(tipo) && defaults.TryGetValue(tipo, out var defs))
                    {
                        var present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var p in props)
                            present.Add(p.Name);

                        foreach (var kv in defs)
                        {
                            if (!present.Contains(kv.Key))
                                props.Insert(0, new PropertyEntry(kv.Key, kv.Value));
                        }
                    }

                    // Crear descriptor
                    var descriptor = new ControlDescriptor(tipo, props, currentGroup)
                    {
                        SourceFile = rutaArchivo
                    };
                    if (children != null && children.Count > 0)
                    {
                        descriptor.Children = children;
                    }

                    // NUEVO: jerarquía por indentación
                    if (stack.Count == 0)
                    {
                        results.Add(descriptor);
                        stack.Push((indent, descriptor));
                    }
                    else
                    {
                        var (prevIndent, parent) = stack.Peek();

                        if (indent > prevIndent)
                        {
                            parent.Children.Add(descriptor);
                            stack.Push((indent, descriptor));
                        }
                        else
                        {
                            while (stack.Count > 0 && stack.Peek().indent >= indent)
                                stack.Pop();

                            if (stack.Count > 0)
                            {
                                stack.Peek().desc.Children.Add(descriptor);
                            }
                            else
                            {
                                results.Add(descriptor);
                            }

                            stack.Push((indent, descriptor));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error parsing line {lineno} en {Path.GetFileName(rutaArchivo)}: {ex.Message}");
                }
            }

            return results;
        }


        /// <summary>
        /// Resuelve una ruta de archivo (absoluta o relativa)
        /// </summary>
        private static string ResolveFilePath(string filePath, string baseDirectory)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return filePath;
            
            // Limpiar comillas
            filePath = filePath.Trim().Trim('\"', '\'');
            
            // Si es ruta absoluta, devolverla tal cual
            if (Path.IsPathRooted(filePath))
                return filePath;
            
            // Si es relativa, combinar con directorio base
            return Path.Combine(baseDirectory, filePath);
        }
        
        /// <summary>
        /// Carga propiedades desde un archivo externo
        /// Formato: clave=valor (una por línea)
        /// </summary>
        private static List<PropertyEntry> LoadPropertiesFromFile(string filePath)
        {
            var properties = new List<PropertyEntry>();
            
            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    var trimmed = line.Trim();
                    
                    // Ignorar líneas vacías y comentarios
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                        continue;
                    
                    // Parsear propiedad
                    if (trimmed.Contains("="))
                    {
                        var prop = PropertyParser.ParsearPropiedad(trimmed);
                        if (!string.IsNullOrEmpty(prop.Name) && prop.Value != null)
                        {
                            properties.Add(prop);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Error leyendo archivo de propiedades {Path.GetFileName(filePath)}: {ex.Message}");
            }
            
            return properties;
        }
    }
    
    /// <summary>
    /// Parser de líneas individuales (separa por ';')
    /// </summary>
    public static class LineParser
    {
        public static List<string> ParsearLinea(string linea)
        {
            var fragmentos = new List<string>();
            var actual = "";
            bool entreComillas = false;
            
            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];
                
                if (c == '"')
                {
                    entreComillas = !entreComillas;
                    actual += c;
                }
                else if (c == ';' && !entreComillas)
                {
                    if (!string.IsNullOrWhiteSpace(actual))
                        fragmentos.Add(actual.Trim());
                    actual = "";
                }
                else
                {
                    actual += c;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(actual))
                fragmentos.Add(actual.Trim());
            
            return fragmentos;
        }
    }
    
    /// <summary>
    /// Parser de propiedades individuales (formato: clave=valor)
    /// </summary>
    public static class PropertyParser
    {
        public static PropertyEntry ParsearPropiedad(string fragmento)
        {
            var idx = fragmento.IndexOf('=');
            if (idx == -1)
                return new PropertyEntry(fragmento, null);
            
            var name = fragmento.Substring(0, idx).Trim();
            var value = fragmento.Substring(idx + 1).Trim().Trim('\"');
            
            return new PropertyEntry(name, value);
        }
    }
    
    /// <summary>
    /// Entrada de propiedad (nombre-valor)
    /// </summary>
    public class PropertyEntry
    {
        public string Name { get; }
        public string Value { get; }
        
        public PropertyEntry(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    
    /// <summary>
    /// Descriptor de control con todas sus propiedades y controles hijos
    /// </summary>
    public class ControlDescriptor
    {
        public string TypeName { get; }
        public List<PropertyEntry> Properties { get; }
        public string GroupName { get; }
        public List<ControlDescriptor> Children { get; set; }
        // NUEVO: archivo de origen
        public string SourceFile { get; set; }

        public ControlDescriptor(string typeName, List<PropertyEntry> properties, string groupName = null)
        {
            TypeName = typeName;
            Properties = properties ?? new List<PropertyEntry>();
            GroupName = groupName;
            Children = new List<ControlDescriptor>();
        }
        
        public override string ToString()
        {
            var propsStr = string.Join(", ", Properties.Select(p => $"{p.Name}={p.Value}"));
            var childrenStr = Children.Count > 0 ? $" [{Children.Count} children]" : "";
            return $"{TypeName}: {propsStr}{childrenStr}";
        }
    }
}