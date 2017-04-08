using CBW.Core;
using CBW.Models;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CBW.ApiWorker
{
    public class RecentController : ApiController
    {
        public IEnumerable<Memo> Get(string uid, int limit = 5, int offset = 0)
        {
            return new MemoProvider(RoleEnvironment.GetConfigurationSettingValue("cloudConn")).Get(uid, limit, offset);
        }
    }
}
