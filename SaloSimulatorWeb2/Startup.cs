using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SaloSimulatorWeb2.Startup))]
namespace SaloSimulatorWeb2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
