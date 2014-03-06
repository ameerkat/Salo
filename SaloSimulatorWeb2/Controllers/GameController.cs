using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Salo;
using Salo.SaloSimulatorWeb2.Models;
using System.Linq;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using System.Data.Entity;

namespace SaloSimulatorWeb2.Controllers
{
    public class GameController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.Find(this.User.Identity.GetUserId());
                var games = context.Games.Where(x => x.Creator == user).ToList();
                return View(games);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> configurations = new List<SelectListItem>();
            string bots;
            using (var context = new ApplicationDbContext())
            {
                context.Configurations.ForEach(x => configurations.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = x.Name,
                    Value = x.Name
                }));
                var jsonResultBotsList = context.Bots.Include(x => x.Uploader).ToList();
                var jsonResultBots = Json(jsonResultBotsList.Select(x => x.BotName + "v" + x.BotVersion + " [@"+  (x.Uploader == null ? "null" : x.Uploader.UserName) + "]").ToArray());
                bots = new JavaScriptSerializer().Serialize(jsonResultBots.Data);
            }

            ViewBag.ConfigurationTemplate = configurations;
            ViewBag.BotJs = bots;

            return View();
        }
    }
}