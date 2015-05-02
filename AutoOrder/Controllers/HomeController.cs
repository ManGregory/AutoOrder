using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoOrder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "О программе";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "О нас";

            return View();
        }
    }
}