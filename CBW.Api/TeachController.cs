using AttributeRouting.Web.Http;
using CBW.Core;
using CBW.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CBW.Api
{
    public class TeachController : ApiController
    {
        [POST("teach/{uid}")]
        public string Post(string uid, [FromBody]TeachRequest request)
        {
            return new CoreWorker(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString).Teach(uid, request);
        }
    }
}
