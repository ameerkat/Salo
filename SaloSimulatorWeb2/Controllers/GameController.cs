using Microsoft.AspNet.Identity;
using Salo.SaloSimulatorWeb2.Models;
using System.Linq;
using System.Web.Mvc;

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
            return View();
        }
    }
}