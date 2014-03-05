using System.Data.Entity;
using Salo.SaloSimulatorWeb2.Models;
using SaloSimulatorWeb2.App_Start;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Salo.SaloSimulatorWeb2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // insert the configuration templates from the configuration *.ini files
            ConfigConfig.ParseConfigs();
        }
    }
}
