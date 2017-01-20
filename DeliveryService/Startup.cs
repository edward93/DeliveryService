using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeliveryService.Startup))]
namespace DeliveryService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
