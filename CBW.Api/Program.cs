using CBW.Core;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using AttributeRouting.Web.Http.SelfHost.Logging;

namespace CBW.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            // Endpoint
            var config = new HttpSelfHostConfiguration("http://localhost:8002");

            // Serialization
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter()
            {
                CamelCaseText = true
            });
            jsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            // Exception handling
            config.Filters.Add(new ApiExceptionFilterAttribute());

            // Routing
            AttributeRoutingConfig.RegisterRoutes(config);

            // Start reading dictionary
            Console.WriteLine("Reading dictionary...");
            CoreWorker worker = new CoreWorker(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);

            // Start server
            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                Console.WriteLine("Started server at endpoint: {0}", config.BaseAddress.AbsoluteUri);
                server.OpenAsync().Wait();
                config.Routes.Cast<HttpRoute>().ToArray().LogTo(Console.Out);
                Console.WriteLine("Press Enter to quit...");
                Console.ReadLine();
            }
        }
    }
}
