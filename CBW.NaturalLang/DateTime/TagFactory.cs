using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class TagFactory
    {
        protected ITagger[] Taggers { get; set; }
        protected Dictionary<int, TaggedItem> IdentifiedItems { get; set; }

        public TagFactory(params ITagger[] taggers)
        {
            this.Taggers = taggers;
            this.IdentifiedItems = new Dictionary<int, TaggedItem>();
        }

        public TaggedItem[] Tag(string[] tokens)
        {
            TaggedItem[] items = new TaggedItem[tokens.Length];

            foreach (ITagger tagger in this.Taggers)
            {
                foreach (TaggedItem item in tagger.Tag(tokens))
                {
                    if (!this.IdentifiedItems.ContainsKey(item.TokenIndex))
                    {
                        this.IdentifiedItems.Add(item.TokenIndex, item);
                    }
                }
            }
            for (int index = 0; index < tokens.Length; index++)
            {
                TaggedItem item;
                if (this.IdentifiedItems.TryGetValue(index, out item))
                {
                    items[index] = item;
                }
                else
                {
                    items[index] = new TaggedItem()
                    {
                        TokenIndex = index,
                        Tag = null,
                        Term = tokens[index]
                    };
                }
            }
            return items;
        }
    }
}
