using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Salo;
using SaloSimulatorWeb2.Models;

namespace SaloSimulatorWeb2.Controllers
{
    public class BotController : Controller
    {
        public enum MessageType { Ok, Error }

        [Authorize]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Bots/tmp/"), Guid.NewGuid().ToString());
                file.SaveAs(path);
                // load up the info
                var assembly = Assembly.LoadFrom(path);
                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Any(x => x == typeof (ISaloBot)))
                        {
                            var bot = new BotModel();
                            bot.Created = DateTime.UtcNow;
                            var attributes = type.GetCustomAttributes(true);
                            var botName = (BotName) attributes.FirstOrDefault(a => a is BotName);
                            var botDescription = (BotDescription) attributes.FirstOrDefault(a => a is BotDescription);
                            if (botName != null)
                            {
                                bot.BotName = botName.Name;
                                bot.BotVersion = botName.Version;
                            }
                            else
                            {
                                // use class name as bot name
                                bot.BotName = type.Name;
                                // use assembly version for version info
                                var versionString =
                                    Regex.Match(assembly.FullName, @"Version=\s*(\d[\d\.]*)\s*,").Groups[1].Value;
                                bot.BotVersion = versionString;
                            }

                            if (botDescription != null)
                            {
                                bot.BotDescription = botDescription.Description;
                            }
                            else
                            {
                                // use no description
                                bot.BotDescription = String.Empty;
                            }

                            var newPath =
                                Path.Combine(
                                    Server.MapPath(String.Format("~/App_Data/Bots/{0}/{1}/{2}", this.User.Identity.Name,
                                        bot.BotName, bot.BotVersion)), fileName);
                            bot.AssemblyPath = newPath;
                            file.SaveAs(newPath);

                            using (var appContext = new ApplicationDbContext())
                            {
                                bot.Uploader = appContext.Users.Find(this.User.Identity.GetUserId());
                            }

                            using (var botContext = new SaloDbContext())
                            {
                                botContext.Bots.Add(bot);
                                botContext.SaveChanges();
                            }
                        }
                    }
                }

                ViewBag.Message = "Upload successfull!";
                ViewBag.MessageType = MessageType.Ok;
                return View("Bot/Index.cshtml");
            }

            ViewBag.Message = "No file attached...";
            ViewBag.MessageType = MessageType.Error;
            return View("Bot/Index.cshtml");
        }

        //
        // GET: /Bot/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
	}
}