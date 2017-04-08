using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class DateTimeTagger : ITagger
    {
        public static Dictionary<string, DateTimeType> KeywordsDictionary { get; set; }

        static DateTimeTagger()
        {
            KeywordsDictionary = new Dictionary<string, DateTimeType>();
            foreach (string line in File.ReadLines("Data\\Tags\\datetime.txt"))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                {
                    continue;
                }
                DateTimeType type;
                if (Enum.TryParse<DateTimeType>(parts[1], out type))
                {
                    KeywordsDictionary.Add(parts[0], type);
                }
            }
        }
        public IEnumerable<TaggedItem> Tag(string[] tokens)
        {
            for (int index = 0; index < tokens.Length; index++)
            {
                DateTimeType type;
                if (KeywordsDictionary.TryGetValue(tokens[index], out type))
                {
                    yield return new TaggedDateTimeItem()
                    {
                        TokenIndex = index,
                        Type = type,
                        Tag = NaturalLang.Tag.DateTime,
                        Term = tokens[index]
                    };
                }
            }
        }
    }
}
