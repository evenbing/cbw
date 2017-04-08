using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public interface ITimeEvalNode
    {
        string Term { get; set; }
        Func<DateTime, DateTime, DateTimeOperation, DateTime> Evaluate { get; set; }
        DateTime GetCurrentValue(DateTime now);
    }
}
