using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class IgnoreTagger : DefaultTagger
    {
        public IgnoreTagger()
            : base()
        {
            this.Add(new string[] { "the", "this" }, NaturalLang.Tag.Ignore);
        }
    }
}
