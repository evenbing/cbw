using CBW.Core;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace CBW.ApiWorker
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Setup camel case
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter()
            {
                CamelCaseText = true
            });

            // Setup ignoring null value properties
            jsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            config.Routes.MapHttpRoute(
                "Default",
                "{controller}/{uid}",
                new { uid = RouteParameter.Optional });

            app.UseWebApi(config);


            // Start up static variables
            Trace.WriteLine("Reading dictionary...");
            CoreWorker worker = new CoreWorker(RoleEnvironment.GetConfigurationSettingValue("cloudConn"));
            Trace.WriteLine("Reading filter list...");
            StringParser.Parse("");
        }
    }
}
