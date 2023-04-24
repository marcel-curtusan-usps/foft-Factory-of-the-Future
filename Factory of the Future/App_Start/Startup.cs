using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

using Owin;
using System.Threading;

[assembly: OwinStartup(typeof(Factory_of_the_Future.Startup))]

namespace Factory_of_the_Future
{
    public class Startup : PersistentConnection
    {
        public void Configuration(IAppBuilder app)
        {
            AppParameters.Start();
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
            Thread.Sleep(1000);
            BackgroundThread.Start();
        }


    }
}
