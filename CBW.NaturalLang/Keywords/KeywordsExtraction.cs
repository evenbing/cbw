using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.NaturalLang
{
    public static class KeywordsExtraction
    {
        static SentenceBreaker SB = new SentenceBreaker();
        static WordBreaker WB = new WordBreaker(CultureInfo.GetCultureInfo(1033));
        static Stemmer Stm = new Stemmer(CultureInfo.GetCultureInfo(1033));
        const int MaximumWordLength = 255;
        static string[] _excludedWords = { "just", "how", "what", "all", "can't", "will", "would", "its", "had", "his","her", "the", "to", "in", "by", "is", "and", "this", "gt", "of", "lt", "for", "not", "it", "on", "with", "new", "be", "as", "that", "from", "we", "you", "am", "when", "if", "are", "no", "see", "at", "or", "up", "will", "an", "have", "but", "after", "was", "text", "can", "like", "there", "then", "has", "http", "so", "do", "one", "same", "which", "steps", "now", "also", "into", "still", "back", "need", "end", "use", "set", "due", "our", "out", "could", "should", "expect", "more", "any" };
        static HashSet<string> ExcludedWords;

        static KeywordsExtraction()
        {
            ExcludedWords = new HashSet<string>();
            ExcludedWords.UnionWith(_excludedWords);
        }

        public static string[] ExtractUniqueKeywords(string content)
        {

            string text = String.Concat(ReplaceNonCharacters(content, '?')
                                              .Normalize(NormalizationForm.FormD)
                                              .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                                              .Normalize(NormalizationForm.FormC);

            HashSet<string> results = new HashSet<string>();
            // Break the free form comment into sentences 
            foreach (var sentenceBody in SB.GetSentences(text))
            {
                // Break the sentence into words 
                foreach (var wordBody in WB.GetWords(sentenceBody))
                {
                    string word = wordBody;
                    if (word.Length < 2)
                    {
                        continue;
                    }
                    if (word.Length > MaximumWordLength)
                    {
                        word = word.Substring(0, MaximumWordLength);
                    }
                    else
                    {
                        word = Stm.StemWord(word);
                    }
                    if (!results.Contains(word) && !ExcludedWords.Contains(word))
                    {
                        if (word == "gat")
                        {
                            word = "get";
                        }
                        results.Add(word);
                    }
                }
            }

            return results.ToArray();
        }

        static string ReplaceNonCharacters(string aString, char replacement)
        {
            var sb = new StringBuilder(aString.Length);
            for (var i = 0; i < aString.Length; i++)
            {
                if (char.IsSurrogatePair(aString, i))
                {
                    int c = char.ConvertToUtf32(aString, i);
                    i++;
                    if (IsCharacter(c))
                        sb.Append(char.ConvertFromUtf32(c));
                    else
                        sb.Append(replacement);
                }
                else
                {
                    char c = aString[i];
                    if (IsCharacter(c))
                        sb.Append(c);
                    else
                        sb.Append(replacement);
                }
            }
            return sb.ToString();
        }


        static bool IsCharacter(int point)
        {
            return point < 0xFDD0 || // everything below here is fine
                point > 0xFDEF &&    // exclude the 0xFFD0...0xFDEF non-characters
                (point & 0xfffE) != 0xFFFE; // exclude all other non-characters
        }
    }
}
