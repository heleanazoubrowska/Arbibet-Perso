using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(ArbibetProgram.SignalR.Startup))]

namespace ArbibetProgram.SignalR
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        public void Configuration(IAppBuilder prm_AppBuilder)
        {
            prm_AppBuilder.Map("/signalr", prv_Map =>
            {
                var prv_HubConfiguration = new HubConfiguration
                {
                    EnableJavaScriptProxies = true,
                    EnableDetailedErrors = true
                };
                prv_HubConfiguration.EnableDetailedErrors = true;
                prv_Map.UseCors(CorsOptions.AllowAll);
                prv_Map.RunSignalR(prv_HubConfiguration);
            });

            
            //prm_AppBuilder.UseCors(CorsOptions.AllowAll);
            //var prv_HubConfiguration = new HubConfiguration();
            //prv_HubConfiguration.EnableDetailedErrors = true;
            //prm_AppBuilder.MapSignalR(prv_HubConfiguration);
        }
    }
}
