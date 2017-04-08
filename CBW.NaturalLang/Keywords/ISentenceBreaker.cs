using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.NaturalLang
{
    using System.Collections.Generic;

    public interface ISentenceBreaker
    {
        IEnumerable<string> GetSentences(string text);
    }
}

