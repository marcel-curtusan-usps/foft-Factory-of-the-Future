using Microsoft.AspNet.SignalR;
using Microsoft.Owin;

using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Factory_of_the_Future.Startup))]

namespace Factory_of_the_Future
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(typeof(ClaimsMiddleware));
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
            //load all configuration 
            Task.Run(() =>
           AppParameters.Start()
            ).ConfigureAwait(false);
            ////start the clean up process
            Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(180)).ConfigureAwait(false);
                BackgroundThread.Start();
            }).ConfigureAwait(false);
        }
        private sealed class ClaimsMiddleware : OwinMiddleware
        {
            public ClaimsMiddleware(OwinMiddleware next)
                : base(next)
            {
            }

            public override Task Invoke(IOwinContext context)
            {
                string username = context.Request.Headers.Get("username");

                if (!String.IsNullOrEmpty(username))
                {
                    var authenticated = username == "john" ? "true" : "false";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Authentication, authenticated)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims);
                    context.Request.User = new ClaimsPrincipal(claimsIdentity);
                }

                return Next.Invoke(context);
            }
        }

    }
}
