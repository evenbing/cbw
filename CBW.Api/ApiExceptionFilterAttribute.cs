using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace CBW.Api
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = FormatMessage(HttpStatusCode.NotFound, context.Exception.Message);
        }

        private static HttpResponseMessage FormatMessage(HttpStatusCode code, string message)
        {
            return new HttpResponseMessage(code)
            {
                ReasonPhrase = "An exception occurred",
                Content = new StringContent("{" + String.Format("\"message\":\"{0}\"", message) + "}")
            };
        }
    }
}
