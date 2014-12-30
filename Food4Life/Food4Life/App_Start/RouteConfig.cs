using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Food4Life
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Category",
                url: "{controller}/{action}/{category}",
                defaults: new { controller = "Home", action = "Index", category = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Details",
            //    url: "{controller}/{action}/{description}/{categoryChoice}/{page}",
            //    defaults: new { controller = "Recipes", action = "Details", category = UrlParameter.Optional, description = UrlParameter.Optional, categoryChoice = UrlParameter.Optional, page = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
