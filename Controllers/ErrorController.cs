using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Not_Found()
        {
            return View();
        }
        public ActionResult Unauthorized()
        {
            var link = TempData["Link"];
            return View(link);
        }
    }
}