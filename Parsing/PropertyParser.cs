// ============================================================================
// ARCHIVO 5: Parsing/PropertyParser.cs
// ============================================================================
using System;
using System.Text;

namespace DynamicUI.Parsing
{
    public readonly struct PropertyEntry
    {
        public string Name { get; }
        public string Value { get; }
        public PropertyEntry(string name, string value) { Name = name; Value = value; }
    }

    public static class PropertyParser
    {
        public static PropertyEntry ParsearPropiedad(string propiedad)
        {
            if (string.IsNullOrWhiteSpace(propiedad)) 
                return new PropertyEntry(string.Empty, null);

            int equalIndex = -1;
            bool dentroComillas = false;
            bool escapeSiguiente = false;

            for (int i = 0; i < propiedad.Length; i++)
            {
                char c = propiedad[i];

                if (escapeSiguiente) { escapeSiguiente = false; continue; }
                if (c == '\\') { escapeSiguiente = true; continue; }
                if (c == '\"') { dentroComillas = !dentroComillas; continue; }
                if (c == '=' && !dentroComillas) { equalIndex = i; break; }
            }

            if (equalIndex < 0) return new PropertyEntry(propiedad.Trim(), null);

            string nombre = propiedad.Substring(0, equalIndex).Trim();
            string valor = propiedad.Substring(equalIndex + 1).Trim();

            if (valor.StartsWith("\"") && valor.EndsWith("\"") && valor.Length >= 2)
                valor = valor.Substring(1, valor.Length - 2);

            valor = ProcesarEscapes(valor);

            return new PropertyEntry(nombre, valor);
        }

        private static string ProcesarEscapes(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            var sb = new StringBuilder();
            bool escape = false;
            foreach (char c in texto)
            {
                if (escape)
                {
                    switch (c)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 't': sb.Append('\t'); break;
                        case 'r': sb.Append('\r'); break;
                        case '\\': sb.Append('\\'); break;
                        case '\"': sb.Append('\"'); break;
                        case ';': sb.Append(';'); break;
                        case '=': sb.Append('='); break;
                        default: sb.Append(c); break;
                    }
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}