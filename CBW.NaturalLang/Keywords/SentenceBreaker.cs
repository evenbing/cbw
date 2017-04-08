namespace CBW.NaturalLang
{
    using System;
    using System.Collections.Generic;

    public class SentenceBreaker
        : ISentenceBreaker
    {
        public IEnumerable<string> GetSentences(string text)
        {
            var input = ToInputBuffer(text);
            var output = new NativeMethods.SentenceReturnBuffer();
            int sentenceCount = 0;
            int endOfParagraph = 0;

            while (true)
            {
                NativeMethods.FindSentences(true, ref input, 1, ref output, ref sentenceCount, ref endOfParagraph);
                if (sentenceCount == 0)
                {
                    yield break;
                }

                // Yield the found sentence
                var sentence = text.Substring(output.iStart, output.cchSent);
                if (!string.IsNullOrWhiteSpace(sentence))
                {
                    yield return sentence;
                }

                // Skip over the first sentence in input
                input.cchStart += output.cchProc;
            }
        }

        private static NativeMethods.SentenceInputBuffer ToInputBuffer(string text)
        {
            return new NativeMethods.SentenceInputBuffer()
            {
                cchStart = 0,
                cchBuff = text.Length,
                rgchBuff = text,
            };
        }
    }
}
