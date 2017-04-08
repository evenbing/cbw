namespace CBW.NaturalLang
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class Stemmer : Disposable, IStemmer
    {
        private StemmerInternal stemmer;
        private Language language;

        public Stemmer()
            : this(Thread.CurrentThread.CurrentUICulture)
        {
        }

        public Stemmer(CultureInfo culture)
            : this(culture.GetLanguage())
        {
        }

        private Stemmer(Language language)
        {
            this.language = language;
            this.stemmer = new StemmerInternal(language);
        }

        public string StemWord(string word)
        {
            try
            {
                return this.stemmer.StemWord(word);
            }
            catch (InvalidComObjectException)
            {
                this.Recover();
                return this.stemmer.StemWord(word);
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this.stemmer != null)
            {
                this.stemmer.Dispose();
            }
        }

        private void Recover()
        {
            Trace.TraceError("An exception occurred in the word breaker. Rebuilding COM object.");
            this.stemmer = new StemmerInternal(this.language);
        }

        private class StemmerInternal
            : IDisposable
        {
            private NativeMethods.IComStemmer stemmer;
            private WordFormSink sink = new WordFormSink();

            public StemmerInternal(Language language)
            {
                var clsid = language.GetStemmerCLSID();
                object ppv;
                NativeMethods.DllGetClassObject(ref clsid, ref Constants.IID_IClassFactory, out ppv);
                uint registration = 0;

                try
                {
                    NativeMethods.CoRegisterClassObject(ref clsid, ppv, 1, 1, out registration);
                    this.stemmer = (NativeMethods.IComStemmer)NativeMethods.CoCreateInstance(clsid, null, 1, Constants.IID_IStemmer);
                    bool lic;
                    uint result = this.stemmer.Init(Constants.MaxTokenSize, out lic);
                    if (result != 0)
                    {
                        throw new InvalidOperationException("Could not initialize word breaker.");
                    }
                }
                finally
                {
                    NativeMethods.CoRevokeClassObject(registration);
                }
            }

            public string StemWord(string word)
            {
                lock (this)
                {
                    this.sink.Stem = null;
                    stemmer.GenerateWordForms(word, (uint)word.Length, this.sink);
                }
                return this.sink.Stem;
            }

            public void Dispose()
            {
                IntPtr pUnk = Marshal.GetIUnknownForObject(this.stemmer);
                Marshal.Release(pUnk);
                System.GC.SuppressFinalize(this);
            }
        }
    }
}
