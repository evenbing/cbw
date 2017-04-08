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
    public class MemoController : ApiController
    {
        [GET("memo/{uid}")]
        public IEnumerable<Memo> Get(string uid, int limit = 5, int begin = Int32.MaxValue - 1)
        {
            return new MemoProvider(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString).Get(uid, limit, begin);
        }

        [GET("memo/{uid}/{id}")]
        public Memo Get(string uid, int id)
        {
            return new MemoProvider(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString).Get(uid, id);
        }

        [PUT("memo/{uid}/{id}")]
        public string Put(string uid, int id, [FromBody] Memo memo)
        {
            if (memo.Id != id)
            {
                throw new Exception("Memo id not match.");
            }
            new CoreWorker(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString).Update(uid, memo);
            return "Success";
        }

        [DELETE("memo/{uid}/{id}")]
        public string Delete(string uid, int id)
        {
            new MemoProvider(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString).Delete(uid, id);
            return "Success";
        }
    }
}
