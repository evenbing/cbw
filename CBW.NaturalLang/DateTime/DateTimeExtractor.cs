using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public static class DateTimeExtractor
    {
        public static IEnumerable<DateTime> Extract(string source)
        {
            return Extract(source, DateTime.Now);
        }

        public static IEnumerable<DateTime> Extract(string source, DateTime dateTimeEval)
        {
            string[] tokens = Tokenizer.Tokenize(source);
            TaggedItem[] items = new TagFactory(
                new NumberTagger(),
                new OperatorTagger(),
                new DateTimeTagger(),
                new IgnoreTagger())
                .Tag(tokens);
            TaggedItem[] aggregatedItems = NumberAggregator.Aggregate(items);

            bool[] coverageMap = new bool[aggregatedItems.Length];
            for (int i = 0; i < aggregatedItems.Length; i++)
            {
                for (int j = aggregatedItems.Length - 1; j >= i; j--)
                {
                    if (!isCovered(coverageMap, i, j))
                    {
                        TaggedItem[] partial = new TaggedItem[j - i + 1];
                        Array.Copy(aggregatedItems, i, partial, 0, j - i + 1);
                        ITimeEvalNode node;
                        if (DateTimeParser.TryParse(partial, out node))
                        {
                            for (int index = i; index <= j; index++)
                            {
                                coverageMap[index] = true;
                            }
                            yield return node.GetCurrentValue(dateTimeEval);
                        }
                    }
                }
            }
        }

        private static bool isCovered(bool[] map, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if (!map[i]) return false;
            }
            return true;
        }
    }
}
