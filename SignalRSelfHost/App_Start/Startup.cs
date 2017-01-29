using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;


[assembly: OwinStartup(typeof(SignalRSelfHost.App_Start.Startup))]
namespace SignalRSelfHost.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var config = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            UnityConfig.RegisterComponents();

            app.MapSignalR("/signalr", config);
        }
    }
}