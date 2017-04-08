using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class OperatorInfo
    {
        public DateTimeOperation Operation { get; set; }
        public OperatorType Type { get; set; }
        public OperatorReadDirection Direction { get; set; }
        public int Priority { get; set; }
    }
}
