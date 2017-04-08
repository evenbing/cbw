using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class TaggedItem
    {
        public string Term { get; set; }
        public int TokenIndex { get; set; }
        public Tag? Tag { get; set; }
        public override string ToString()
        {
            return String.Format("{0} ({1})", this.Term, this.Tag);
        }
    }

    public class TaggedNumberItem : TaggedItem
    {
        public double Number { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", this.Number, this.Tag);
        }
    }

    public class TaggedOperatorItem : TaggedItem
    {
        public OperatorInfo Info { get; set; }
    }

    public class TaggedDateTimeItem : TaggedItem
    {
        public DateTimeType Type { get; set; }
    }
}
