using CBW.Core;
using CBW.Models;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CBW.ApiWorker
{
    public class RetrieveController : ApiController
    {
        public IEnumerable<string> Post(string uid, [FromBody]RetrieveRequest request)
        {
            return new CoreWorker(RoleEnvironment.GetConfigurationSettingValue("cloudConn")).Retrieve(uid, request);
        }
    }
}
