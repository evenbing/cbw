
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace CBW.NaturalLang
{

    public class WordBreaker : Disposable, IWordBreaker
    {
        private WordBreakerInternal wbInternal;
        private Language language;

        public WordBreaker()
            : this(Thread.CurrentThread.CurrentUICulture)
        {
        }

        public WordBreaker(CultureInfo culture)
            : this(culture.GetLanguage())
        {
        }

        private WordBreaker(Language language)
        {
            this.language = language;
            this.wbInternal = new WordBreakerInternal(language);
        }

        public IEnumerable<string> GetWords(string text)
        {
            try
            {
                return this.wbInternal.BreakText(text).Select(w => w.TrimEnd('.'));
            }
            catch (InvalidComObjectException)
            {
                this.Recover();
                return this.wbInternal.BreakText(text);
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this.wbInternal != null)
            {
                this.wbInternal.Dispose();
                this.wbInternal = null;
            }
        }

        private void Recover()
        {
            Trace.TraceError("An exception occurred in the word breaker. Rebuilding COM object.");
            this.wbInternal = new WordBreakerInternal(this.language);
        }

        private class WordBreakerInternal
            : IDisposable
        {
            private NativeMethods.IComWordBreaker wb;

            public WordBreakerInternal(Language language)
            {
                var clsid = language.GetWordBreakerCLSID();
                object ppv;
                NativeMethods.DllGetClassObject(ref clsid, ref Constants.IID_IClassFactory, out ppv);
                uint registration = uint.MaxValue;

                try
                {
                    NativeMethods.CoRegisterClassObject(ref clsid, ppv, 1, 1, out registration);
                    this.wb = (NativeMethods.IComWordBreaker)NativeMethods.CoCreateInstance(clsid, null, 1, Constants.IID_IWordBreaker);
                    uint lic = 0;
                    uint result = this.wb.Init(0, Constants.MaxTokenSize, out lic);
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

            public string[] BreakText(string text)
            {
                var sink = new WordSink();
                var source = NativeMethods.TextSource.Create(text);
                lock (this)
                {
                    this.wb.BreakText(ref source, sink, null);
                }
                return sink.Words;
            }

            public void Dispose()
            {
                IntPtr pUnk = Marshal.GetIUnknownForObject(this.wb);
                Marshal.Release(pUnk);
                System.GC.SuppressFinalize(this);
            }
        }
    }
}
