using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.WindowsPhone
{
    public static class Api
    {
        public const string TimeExtractionUri = "http://cbwvm.cloudapp.net/extract?s={0}&dt={1}";
        public const string TeachUri = "http://cbwvm.cloudapp.net/teach/{0}";
        public const string RetrieveUri = "http://cbwvm.cloudapp.net/retrieve/{0}";
        public const string RecentUri = "http://cbwvm.cloudapp.net/memo/{0}";
        public const string MemoUri = "http://cbwvm.cloudapp.net/memo/{0}/{1}";
    }
}
