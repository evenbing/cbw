using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CBW.NaturalLang
{
    public class NumberTagger : ITagger
    {
        public const string NumberTagPath = @"Data\Tags\numbers.txt";
        public const string NumberOrdTagPath = @"Data\Tags\numberOrd.txt";
        public static Regex NumberOrdRegex { get; set; }
        public static Dictionary<string, double> NumberDictionary { get; set; }
        public static Dictionary<string, int> NumberOrdDictionary { get; set; }

        static NumberTagger()
        {
            NumberOrdRegex = new Regex("^([0-9]*)(([4567890]th)|([0-9^1]1st)|([0-9^1]2nd)|([0-9^1]3rd)|(1[123]th))$", RegexOptions.Compiled);
            NumberDictionary = new Dictionary<string, double>();
            foreach (string line in File.ReadLines(NumberTagPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                {
                    continue;
                }
                else
                {
                    double x;
                    if (Double.TryParse(parts[1], out x))
                    {
                        NumberDictionary.Add(parts[0], x);
                    }
                }
            }
            NumberOrdDictionary = new Dictionary<string, int>();
            foreach (string line in File.ReadLines(NumberOrdTagPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                {
                    continue;
                }
                else
                {
                    NumberOrdDictionary.Add(parts[0], Int32.Parse(parts[1]));
                }
            }
        }

        public NumberTagger()
        {
        }

        public IEnumerable<TaggedItem> Tag(string[] tokens)
        {
            List<TaggedItem> results = new List<TaggedItem>();
            for (int index = 0; index < tokens.Length; index++)
            {
                if (String.IsNullOrEmpty(tokens[index]))
                {
                    continue;
                }

                double x;
                int y;
                if (NumberDictionary.TryGetValue(tokens[index], out x))
                {
                    results.Add(new TaggedNumberItem()
                    {
                        TokenIndex = index,
                        Term = tokens[index],
                        Number = x,
                        Tag = NaturalLang.Tag.Number
                    });
                }
                else if (NumberOrdDictionary.TryGetValue(tokens[index], out y))
                {
                    results.Add(new TaggedNumberItem()
                    {
                        TokenIndex = index,
                        Term = tokens[index],
                        Number = y,
                        Tag = NaturalLang.Tag.NumberOrdinal
                    });
                }
                else if (Int32.TryParse(tokens[index], out y))
                {
                    results.Add(new TaggedNumberItem()
                    {
                        TokenIndex = index,
                        Term = tokens[index],
                        Number = y,
                        Tag = NaturalLang.Tag.Number
                    });
                }
                else if (NumberOrdRegex.IsMatch(tokens[index]))
                {
                    Int32.TryParse(tokens[index].Substring(0, tokens[index].Length - 2), out y);
                    results.Add(new TaggedNumberItem()
                    {
                        TokenIndex = index,
                        Term = tokens[index],
                        Number = y,
                        Tag = NaturalLang.Tag.NumberOrdinal
                    });
                }
            }
            return results;
        }
    }
}
