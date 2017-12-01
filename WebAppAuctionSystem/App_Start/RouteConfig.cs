using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAppAuctionSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Auth",
                url: "Auth/{action}",
                defaults: new {controller="Auth", action = "Index" }
            );

            routes.MapRoute(
                name: "User",
                url: "User/Edit/{userId}",
                defaults: new { controller="User", action = "Edit", userId = @"\d+" }
            );
        }
    }
}
