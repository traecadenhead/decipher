﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Decipher.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AppOriginal",
                url: "app-original/{*path}",
                defaults: new { controller = "Home", action = "AppOriginal", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "App",
                url: "app/{*path}",
                defaults: new { controller = "Home", action = "App", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "City",
                url: "city/{city}",
                defaults: new { controller = "Home", action = "City", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
