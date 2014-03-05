using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Salo;
using Salo.SaloSimulatorWeb2.Models;

namespace Salo.SaloSimulatorWeb2.Controllers
{
    public class BotController : Controller
    {
        public enum MessageType { Ok, Error }

        [Authorize]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var folder = Server.MapPath("~/App_Data/Bots/tmp/");
                var path = Path.Combine(folder, Guid.NewGuid().ToString() + ".dll");
                System.IO.Directory.CreateDirectory(folder);
                file.SaveAs(path);
                // load up the info
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(path);
                }
                catch (Exception exception)
                {
                    ViewBag.Message = "Failed to load assembly.";
                    ViewBag.MessageType = MessageType.Error;
                    System.IO.File.Delete(path); // remove from temporary
                    return View("Create");
                }

                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Any(x => x == typeof (ISaloBot)))
                        {
                            var bot = new Bot();
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

                            var newFolder =
                                Server.MapPath(String.Format("~/App_Data/Bots/{0}/{1}/{2}", this.User.Identity.Name,
                                    bot.BotName, bot.BotVersion));
                            var newPath = Path.Combine(newFolder, fileName);
                            bot.AssemblyPath = newPath;
                            System.IO.Directory.CreateDirectory(newFolder);
                            file.SaveAs(newPath);

                            using (var appContext = new ApplicationDbContext())
                            {
                                var existingBot =
                                    appContext.Bots.FirstOrDefault(
                                        x => x.BotName == bot.BotName && x.BotVersion == bot.BotVersion);
                                if (existingBot != null)
                                {
                                    bot.Id = existingBot.Id;
                                }

                                bot.Uploader = appContext.Users.Find(this.User.Identity.GetUserId());
                                appContext.Bots.Add(bot);
                                try
                                {
                                    appContext.SaveChanges();
                                }
                                catch (DbEntityValidationException ex)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                RedirectToAction("Index");
            }

            ViewBag.Message = "No file attached...";
            ViewBag.MessageType = MessageType.Error;
            return View("Create");
        }

        //
        // GET: /Bot/
        public ActionResult Index()
        {
            List<Bot> bots;
            using (var botContext = new ApplicationDbContext())
            {
                bots = botContext.Bots.Include(x => x.Uploader).ToList();
            }
            return View(bots);
        }

        public ActionResult Create()
        {
            return View();
        }
	}
}