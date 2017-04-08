namespace CBW.NaturalLang
{
    using System.Collections.Generic;

    internal class WordSink
        : NativeMethods.IComWordSink
    {
        private List<string> words = new List<string>();

        public void PutWord(uint cwc, string buff, uint cwcSrcLen, uint cwcSrcPos)
        {
            string word = buff.Substring(0, (int)cwc);
            this.words.Add(word);
        }

        public void PutAltWord(uint cwc, string buff, uint cwcSrcLen, uint cwcSrcPos)
        {
        }

        public void StartAltPhrase()
        {
        }

        public void EndAltPhrase()
        {
        }

        public void PutBreak(NativeMethods.WORDREP_BREAK_TYPE breakType)
        {
        }

        public string[] Words
        {
            get
            {
                return this.words.ToArray();
            }
        }
    }
}
