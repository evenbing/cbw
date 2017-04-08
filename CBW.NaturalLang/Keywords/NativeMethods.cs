namespace CBW.NaturalLang
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct TextSource
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            FillTextDelegate pfnFillTextBuffer;

            [MarshalAs(UnmanagedType.LPWStr)]
            string buffer;
            [MarshalAs(UnmanagedType.U4)]
            uint iEnd;
            [MarshalAs(UnmanagedType.U4)]
            uint iCur;

            internal static TextSource Create(string buff)
            {
                TextSource ts = new TextSource();
                ts.buffer = buff;
                ts.iCur = 0;
                ts.iEnd = (uint)buff.Length;
                ts.pfnFillTextBuffer = new FillTextDelegate(TextSourceAux.FillText);
                return ts;
            }
        }

        internal static class TextSourceAux
        {
            private const uint WBREAK_E_END_OF_TEXT = 0x80041780;

            internal static uint FillText(ref TextSource textSource)
            {
                return WBREAK_E_END_OF_TEXT;
            }
        }

        internal delegate uint FillTextDelegate([MarshalAs(UnmanagedType.Struct)] ref  TextSource textSource);

        public enum WORDREP_BREAK_TYPE
        {
            WORDREP_BREAK_EOW = 0,
            WORDREP_BREAK_EOS = 1,
            WORDREP_BREAK_EOP = 2,
            WORDREP_BREAK_EOC = 3
        }

        [Guid("CC907054-C058-101A-B554-08002B33B0E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IComWordSink
        {
            void PutWord(uint cwc, [MarshalAs(UnmanagedType.LPWStr)] string buff, uint cwcSrcLen, uint cwcSrcPos);
            void PutAltWord(uint cwc, [MarshalAs(UnmanagedType.LPWStr)] string buff, uint cwcSrcLen, uint cwcSrcPos);
            void StartAltPhrase();
            void EndAltPhrase();
            void PutBreak(WORDREP_BREAK_TYPE breakType);
        }

        [Guid("D53552C8-77E3-101A-B552-08002B33B0E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IComWordBreaker
        {
            uint Init(uint fQuery, uint ulMaxTokenSize, out uint license);
            void BreakText([MarshalAs(UnmanagedType.Struct)] ref TextSource textSouce, [MarshalAs(UnmanagedType.Interface)] IComWordSink wordSink, [MarshalAs(UnmanagedType.Interface)] IComPhraseSink phraseSink);
            void ComposePhrase(string noun, uint nNoun, string modifier, uint nModifier, uint attachmentType, [MarshalAs(UnmanagedType.LPWStr)] ref StringBuilder phrase, ref uint nPhrase);
            void GetLicenseToUse([MarshalAs(UnmanagedType.LPWStr)] ref StringBuilder sb);
        };

        [Guid("CC906FF0-C058-101A-B554-08002B33B0E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IComPhraseSink
        {
            void PutSmallPhrase(string noun, uint nNoun, string modifier, uint nModifier, uint attachmentType);
            void PutPhrase(string phrase, uint nPhrase);
        }


        [Guid("efbaf140-7f42-11ce-be57-00aa0051fe20")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IComStemmer
        {
            uint Init(uint maxTokenSize, out bool license);
            void GenerateWordForms([MarshalAs(UnmanagedType.LPWStr)] string buff, uint cwc, IComWordFormSink wordFormSink);
            void GetLicenseToUse([MarshalAs(UnmanagedType.LPWStr)] ref StringBuilder license);
        }

        [Guid("fe77c330-7f42-11ce-be57-00aa0051fe20")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IComWordFormSink
        {
            void PutAltWord([MarshalAs(UnmanagedType.LPWStr)] string buff, uint cwc);
            void PutWord([MarshalAs(UnmanagedType.LPWStr)] string buff, uint cwc);
        }

        [DllImport("NaturalLanguage6.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void DllGetClassObject(
            [In] ref Guid rclsid,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)][Out] out object ppv);

        [ComImport]
        [Guid("00000001-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IClassFactory
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateInstance(
                [MarshalAs(UnmanagedType.IUnknown)]object pOuterUnk,
                ref Guid iid);

            void LockServer(bool Lock);
        }

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void CoRegisterClassObject(
            [In] ref Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            uint dwClsContext,
            uint flags,
            out uint lpdwRegister
            );

        [DllImport("ole32.dll", PreserveSig = false)]
        public static extern void CoRevokeClassObject(uint dwRegister);

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern object CoCreateInstance(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
           [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
           uint dwClsContext,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct SentenceInputBuffer
        {
            public int cchBuff;
            public int cchStart;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string rgchBuff;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct SentenceReturnBuffer
        {
            public int iStart;
            public int cchSent;
            public int cchProc;
            public int cNonSpaceChars;
            public int cWords;
            public int cWordChars;
            public int cSemicolonFlesch;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fIsSentence;
            public int ctcActualLengthSpacesBetweenSentences;
        }

        [DllImport("sentsepenu_64.dll", ExactSpelling = true, PreserveSig = false, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FindSentences(
            [In, MarshalAs(UnmanagedType.Bool)] bool fRefind,
            ref SentenceInputBuffer pSentenceInputBuffer,
            int maxSentencesToFind,
            ref SentenceReturnBuffer pSentenceReturnBuffer,
            ref int pNumSentencesFound,
            ref int pfHitEndOfParagraph);
    }
}
