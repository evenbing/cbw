using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class OperatorTagger : ITagger
    {
        public static Dictionary<string, OperatorInfo> OperatorDictionary { get; set; }

        static OperatorTagger()
        {
            OperatorDictionary = new Dictionary<string, OperatorInfo>();
            foreach (string line in File.ReadLines("Data\\Tags\\operator.txt"))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 5)
                {
                    continue;
                }
                int x, y;
                OperatorType type;
                OperatorReadDirection dir;
                if (!Int32.TryParse(parts[1], out x))
                {
                    continue;
                }
                if (!Enum.TryParse<OperatorType>(parts[2], out type))
                {
                    continue;
                }
                if (!Enum.TryParse<OperatorReadDirection>(parts[3], out dir))
                {
                    continue;
                }
                if (!Int32.TryParse(parts[4], out y))
                {
                    continue;
                }

                OperatorDictionary.Add(parts[0], new OperatorInfo()
                {
                    Operation = (DateTimeOperation)(x),
                    Type = type,
                    Direction = dir,
                    Priority = y
                });
            }
        }

        public IEnumerable<TaggedItem> Tag(string[] tokens)
        {
            for (int index = 0; index < tokens.Length; index++)
            {
                OperatorInfo info;
                if (OperatorDictionary.TryGetValue(tokens[index], out info))
                {
                    yield return new TaggedOperatorItem()
                    {
                        TokenIndex = index,
                        Info = info,
                        Tag = NaturalLang.Tag.Operator,
                        Term = tokens[index]
                    };
                }
            }
        }
    }
}
