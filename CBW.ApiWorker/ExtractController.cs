using CBW.NaturalLang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

namespace CBW.ApiWorker
{
    public class ExtractController : ApiController
    {
        public IEnumerable<DateTime> Get(string s, string dt)
        {
            return DateTimeExtractor.Extract(
                s, 
                DateTime.ParseExact(dt,
                "s", 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None));
        }
    }
}
