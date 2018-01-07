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
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Auth",
                url: "Auth/{action}",
                defaults: new { controller = "Auth", action = "Index" }
            );

            routes.MapRoute(
                name: "User",
                url: "Users/Edit/{userId}",
                defaults: new { controller = "User", action = "Edit", userId = @"\d+" }
            );

            routes.MapRoute(
                name: "Coins purchase",
                url: "Users/PurchaseCoins/{userId}",
                defaults: new { controller = "User", action = "PurchaseCoins", userId = @"\d+" }
            );

            routes.MapRoute(
                name: "Catalog",
                url: "Products",
                defaults: new { controller = "Product", action = "Index" }
            );

            routes.MapRoute(
                name: "CreateProduct-admin",
                url: "Products/Create",
                defaults: new { controller = "Product", action = "Create" }
            );
            routes.MapRoute(
                 name: "BidExpired",
                 url: "Products/BidExpired",
                 defaults: new { controller = "Product", action = "BidExpired" }
            );
            routes.MapRoute(
                 name: "ShowProduct",
                 url: "Products/{productId}",
                 defaults: new { controller = "Product", action = "Show", productId = @"\d+" }
            );
        }
    }
}
