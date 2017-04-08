using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.Core
{
    public static class StringParser
    {
        static HashSet<String> _wordsToBeFiltered = null;

        static StringParser()
        {
            _wordsToBeFiltered = new HashSet<String>();

            String line;
            System.IO.StreamReader file = new System.IO.StreamReader("FilterDictionary.txt");
            while ((line = file.ReadLine()) != null)
            {
                _wordsToBeFiltered.Add(line.ToLower());
            }

            file.Close();
        }

        public static string[] Parse(String content)
        {
            List<String> retval = new List<string>();
            string[] split = content.Split(new Char[] {' ', ',', '.', ':', '\t', '\r', '\n', '\'' });
            foreach (string st in split)
            {
                if (_wordsToBeFiltered.Contains(st.ToLower()) || st.Length == 0)
                {
                    continue;
                }
                retval.Add(st.ToLower());
            }
            return retval.ToArray();
        }

        
    }
}
