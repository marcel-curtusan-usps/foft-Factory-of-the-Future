using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

using Owin;
using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Factory_of_the_Future.Startup))]

namespace Factory_of_the_Future
{
    public class Startup : PersistentConnection
    {
        public void Configuration(IAppBuilder app)
        {
            _ = AppParameters.Start();
            GlobalHost.Configuration.DefaultMessageBufferSize = 32;
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
          
            Task.Run(async delegate {                
                await Task.Delay(TimeSpan.FromSeconds(5));
                BackgroundThread.Start();
            }).ConfigureAwait(true);
        }


    }
}
