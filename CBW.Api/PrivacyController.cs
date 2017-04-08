using AttributeRouting.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CBW.Api
{
    public class PrivacyController : ApiController
    {
        [GET("privacy")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = new HttpResponseMessage();
            message.Content = new StringContent(File.ReadAllText("Data\\Speech Memo Privacy Policy.htm"), Encoding.UTF8, "text/html");
            return message;
        }
    }
}
