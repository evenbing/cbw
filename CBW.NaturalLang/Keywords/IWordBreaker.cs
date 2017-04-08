using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.NaturalLang
{
    interface IWordBreaker
    {
        IEnumerable<string> GetWords(string text);
    }
}
