using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAuctionSystem.Controllers
{
    public class UserMVCController : Controller
    {
        // GET: UserMVC
        public ActionResult Index()
        {
            return View();
        }
    }
}