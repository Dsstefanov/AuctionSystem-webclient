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
                ViewBag.user = authController.GetUserLoggedUser(Request);
                ViewBag.userId = new AuthController().GetUserIdByCookie(Request.Cookies["auth"]);
                return View("Index");
            }
            return Redirect("~/Auth/Login");
        }
    }
}