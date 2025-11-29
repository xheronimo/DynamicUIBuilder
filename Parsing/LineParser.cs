// ============================================================================
// ARCHIVO 6: Parsing/LineParser.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicUI.Parsing
{
    public static class LineParser
    {
        public static List<string> ParsearLinea(string linea)
        {
            var resultado = new List<string>();
            if (string.IsNullOrEmpty(linea)) return resultado;

            var actual = new StringBuilder();
            bool dentroComillas = false;
            bool escapeSiguiente = false;

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                if (escapeSiguiente)
                {
                    actual.Append(c);
                    escapeSiguiente = false;
                    continue;
                }

                if (c == '\\')
                {
                    escapeSiguiente = true;
                    continue;
                }

                if (c == '\"')
                {
                    dentroComillas = !dentroComillas;
                    continue;
                }

                if (c == ';' && !dentroComillas)
                {
                    if (actual.Length > 0)
                    {
                        resultado.Add(actual.ToString().Trim());
                        actual.Clear();
                    }
                    continue;
                }

                actual.Append(c);
            }

            if (actual.Length > 0)
                resultado.Add(actual.ToString().Trim());

            return resultado;
        }
    }
}