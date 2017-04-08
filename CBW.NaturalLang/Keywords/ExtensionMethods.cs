using System;
using System.Globalization;
namespace CBW.NaturalLang
{
    internal static class ExtensionMethods
    {
        public static Language GetLanguage(this CultureInfo cultureInfo)
        {
            Language language;
            if (Constants.LanguageMapping.TryGetValue(cultureInfo.TwoLetterISOLanguageName, out language))
            {
                return language;
            }

            return Language.Neutral;
        }

        public static Guid GetWordBreakerCLSID(this Language language)
        {
            Guid guid;
            if (Constants.WordBreakerCLSIDMapping.TryGetValue(language, out guid))
            {
                return guid;
            }

            return Guid.Empty;
        }

        public static Guid GetStemmerCLSID(this Language language)
        {
            Guid guid;
            if (Constants.StemmerCLSIDMapping.TryGetValue(language, out guid))
            {
                return guid;
            }

            return Guid.Empty;
        }
    }
}