
// ============================================================================
// ARCHIVO 7: Parsing/FileParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicUI.Parsing
{
    public record ControlDescriptor(string TypeName, List<PropertyEntry> Properties, string GroupName = null);

    public static class FileParser
    {
        public static IEnumerable<ControlDescriptor> ParseFile(string rutaArchivo, Func<string, string> groupNormalizer = null)
        {
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException(rutaArchivo);

            string currentControlType = null;
            string currentGroup = null;
            var defaults = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            int lineno = 0;
            foreach (var raw in File.ReadLines(rutaArchivo))
            {
                lineno++;
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var linea = raw.Trim();
                if (linea.StartsWith("#")) continue;

                try
                {
                    if (linea.StartsWith("Grupo=", StringComparison.OrdinalIgnoreCase))
                    {
                        currentGroup = linea.Substring(6).Trim().Trim('\"');
                        if (groupNormalizer != null) currentGroup = groupNormalizer(currentGroup);
                        continue;
                    }

                    if (linea.StartsWith("Control=", StringComparison.OrdinalIgnoreCase))
                    {
                        currentControlType = linea.Substring(8).Trim();
                        if (!defaults.ContainsKey(currentControlType))
                            defaults[currentControlType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        continue;
                    }

                    // Defaults: lineas con = pero sin ;
                    if (currentControlType != null && linea.Contains("=") && !linea.Contains(";"))
                    {
                        var pe = PropertyParser.ParsearPropiedad(linea);
                        if (pe.Value != null) defaults[currentControlType][pe.Name] = pe.Value;
                        continue;
                    }

                    // Linea de control normal
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

                    // Aplicar defaults (sin sobreescribir)
                    if (!string.IsNullOrEmpty(tipo) && defaults.TryGetValue(tipo, out var defs))
                    {
                        var present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var p in props) present.Add(p.Name);

                        foreach (var kv in defs)
                        {
                            if (!present.Contains(kv.Key))
                                props.Insert(0, new PropertyEntry(kv.Key, kv.Value));
                        }
                    }

                    yield return new ControlDescriptor(tipo, props, currentGroup);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing line {lineno}: {ex.Message}");
                }
            }
        }
    }
}