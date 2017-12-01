using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAuctionSystem.Controllers
{
    public class HomeController : Controller
    {
        UserServiceReference.UserServiceClient userServiceClient;
        AuthController authController;

        public HomeController()
        {
            userServiceClient = new UserServiceReference.UserServiceClient();
            authController = new AuthController();
        }
        public ActionResult Index()
        {
            if (authController.IsUserLoggedIn(Request, Response))
            {
                ViewBag.userId = new AuthController().GetUserIdByCookie(Request.Cookies["auth"]);
                return View("Index");
            }
            else
            {
                return View("Index");
            }
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}