using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SaloSimulatorWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Actions",
                routeTemplate: "api/actions/{action}/{id}",
                defaults: new { controller = "Actions" }
            );

            config.Routes.MapHttpRoute(
                name: "Game",
                routeTemplate: "api/games/{action}",
                defaults: new { controller = "Games" }
            );
        }
    }
}
