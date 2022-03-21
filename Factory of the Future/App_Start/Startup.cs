using Microsoft.AspNet.SignalR;
using System.Security.Claims;
using Microsoft.Owin;

using Owin;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

[assembly: OwinStartup(typeof(Factory_of_the_Future.Startup))]

namespace Factory_of_the_Future
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AppParameters.Start();
            app.Map("/signalr", map =>
            {
                 
                map.RunSignalR();
            });
            var config = new HubConfiguration
            {
                EnableJSONP = false,
                EnableJavaScriptProxies = true,
                EnableDetailedErrors = true
            };
            app.MapSignalR(config);
            Thread.Sleep(1000);
            BackgroundThread.Start();
        }


    }
}
