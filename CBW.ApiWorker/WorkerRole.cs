using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Owin.Hosting;

namespace CBW.ApiWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IDisposable _webApp = null;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("CBW.ApiWorker entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //// For information on handling configuration changes
            //// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            //ServicePointManager.DefaultConnectionLimit = 12;

            //// New code:
            //var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Endpoint1"];
            //string baseUri = string.Format("{0}://{1}",
            //    endpoint.Protocol, endpoint.IPEndpoint);

            //Trace.TraceInformation(String.Format("Starting OWIN at {0}", baseUri),
            //    "Information");

            //_webApp = WebApp.Start<Startup>(new StartOptions(baseUri));

            return base.OnStart();
        }

        public override void OnStop()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
            }
            base.OnStop();
        }
    }
}
