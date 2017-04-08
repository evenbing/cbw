using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.Models
{
    [Serializable]
    public class WordDictionary
    {
        public WordDictionary()
        {
            this.Dict = new Dictionary<string, DictionaryEntry>();
        }

        public Dictionary<string, DictionaryEntry> Dict { get; set; }
        public DictionaryEntry this[string s]
        {
            get
            {
                return Dict[s];
            }
            set
            {
                DictionaryEntry entry;
                if (!Dict.TryGetValue(s, out entry))
                {
                    Dict.Add(s, value);
                }
                else
                {
                    entry = value;
                }
            }
        }

        public Dictionary<string, DictionaryEntry>.KeyCollection Keys
        {
            get
            {
                return this.Dict.Keys;
            }
        }

        public bool TryGet(string s, out DictionaryEntry entry)
        {
            return this.Dict.TryGetValue(s, out entry);
        }

        public bool ContainsKey(string s)
        {
            return this.Dict.ContainsKey(s);
        }
    }

    [Serializable]
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string Type { get; set; }

        [NonSerialized]
        public string Definition;
    }
}
