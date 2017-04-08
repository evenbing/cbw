using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public static class Tokenizer
    {
        public static string[] Tokenize(string source)
        {
            return source.ToLowerInvariant()
                .Replace(".", " .")
                .Replace(",", " ,")
                .Split(' ', '\t', '\r', '\n', '-', ':')
                .Where(
                x => !String.IsNullOrEmpty(x))
                .ToArray();
        }
    }
}
