using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public enum Tag
    {
        // o/zero, one, two
        Number,

        // First, second, third...
        NumberOrdinal,

        // Date/time related keywords...
        DateTime,

        // Next, previous, last...
        Operator,

        // the
        Ignore
    }
}
