using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CBW.NaturalLang
{
    public interface ITagger
    {
        IEnumerable<TaggedItem> Tag(string[] tokens);
    }

    public class DefaultTagger : ITagger
    {
        public DefaultTagger()
        {
            this.IdentifiedItems = new Dictionary<string, Tag>();
        }

        private Dictionary<string, Tag> IdentifiedItems { get; set; }

        protected void Add(IEnumerable<string> identifiedItems, Tag tag)
        {
            foreach (string item in identifiedItems)
            {
                if (!this.IdentifiedItems.ContainsKey(item))
                {
                    this.IdentifiedItems.Add(item, tag);
                }
            }
        }

        public virtual IEnumerable<TaggedItem> Tag(string[] tokens)
        {
            for (int index = 0; index < tokens.Length; index++)
            {
                Tag tag;
                if (this.IdentifiedItems.TryGetValue(tokens[index], out tag))
                {
                    yield return new TaggedItem()
                    {
                        Term = tokens[index],
                        Tag = tag,
                        TokenIndex = index
                    };
                }
            }
        }
    }
}
