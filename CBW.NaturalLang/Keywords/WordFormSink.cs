
namespace CBW.NaturalLang
{
    using System;

    internal class WordFormSink
        : NativeMethods.IComWordFormSink
    {
        private string stem;

        public void PutAltWord(string buff, uint cwc)
        {
            this.CheckStem(buff, cwc);
        }

        public void PutWord(string buff, uint cwc)
        {
            this.CheckStem(buff, cwc);
        }

        private void CheckStem(string word, uint len)
        {
            if (this.stem == null ||
                len < this.stem.Length ||
                string.Compare(word, this.stem, StringComparison.Ordinal) < 0)
            {
                this.stem = word;
            }
        }

        public string Stem
        {
            get { return stem; }
            set { this.stem = value; }
        }
    }
}
