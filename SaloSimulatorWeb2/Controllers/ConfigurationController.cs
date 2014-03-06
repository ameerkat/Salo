using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Salo.SaloSimulatorWeb2.Models;
using System.Data.Entity;

namespace SaloSimulatorWeb2.Controllers
{
    public class ConfigurationController : Controller
    {
        //
        // GET: /Configuration/
        public ActionResult Index()
        {
            return View();
        }

        public new ActionResult View(string id = "default")
        {
            using (var context = new ApplicationDbContext())
            {
                return View(context.Configurations.Include(x => x.Parent).Include(x => x.Settings).SingleOrDefault(x => x.Name == id));
            }
        }
	}
}