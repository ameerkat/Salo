using Salo;
using Salo.SaloSimulatorWeb2.Models;
using System.IO;

namespace SaloSimulatorWeb2.App_Start
{
    public class ConfigConfig
    {
        public static void ParseConfigs()
        {
            var folder = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Configuration/");
            if (folder == null)
                return;
            foreach (string fileName in Directory.GetFiles(folder, "*.ini"))
            {
                var configuration = ConfigurationHelper.LoadConfigurationFromIni(fileName);
                using (var context = new ApplicationDbContext())
                {
                    context.Configurations.Add(configuration);
                    // hopefully setting the parent name will be enough to make this magic work
                    context.SaveChanges();
                }
            }
        }
    }
}