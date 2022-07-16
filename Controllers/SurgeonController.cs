using Project.Auth;
using Project.EF;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    [Authorization("Surgeon")]
    public class SurgeonController : Controller
    {
        readonly Project_databaseEntities db = new Project_databaseEntities();
        // GET: Surgeon
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();// remove logout butto
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var user = (from s in db.Surgeons where s.Id.ToString() == login.Username select s).SingleOrDefault();
                if (user == null)
                {
                    ViewBag.Message = "Incorrect Username/ID or Password";
                    return View(login);
                }
                else
                {
                    Session["role"] = "Surgeon";
                    Session["id"] = user.Id;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(login);
            }

        }

        public ActionResult Search(int id = 0)
        {
            var d = db.Drivers.ToList().Skip(id * 10).Take(10);
            return View(d);
        }
        [HttpGet]
        public ActionResult Search_Driver()
        {
            string name = this.Request.Params["name"];
            string regNumber = this.Request.Params["regNumber"];
            if (name.Equals(""))
            {
                var result = db.Drivers.Where(x => x.Driving_license_number.ToString().Contains(regNumber)).Select(x => new { x.Id, x.Photo, x.First_name, x.Last_name, x.Driving_license_number, x.Date_of_issue }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (regNumber.Equals(""))
            {
                var result = db.Drivers.Where(x => (x.First_name+" "+ x.Last_name).Contains(name)).Select(x => new { x.Id, x.Photo, x.First_name, x.Last_name, x.Driving_license_number, x.Date_of_issue }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = db.Drivers.Where(x => (x.First_name + " " + x.Last_name).Contains(name) || x.Driving_license_number.ToString().Contains(regNumber)).Select(x => new { x.Id, x.Photo, x.First_name, x.Last_name, x.Driving_license_number, x.Date_of_issue }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Driver_Details(int id)
        {
            Driver dr = (from d in db.Drivers where d.Id == id select d).SingleOrDefault();
            var offence = (from o in db.Offences select o);
            var offence_details = (from ol in db.Offence_info where ol.Driver_id == dr.Id select ol).ToList();
            //join o in db.Offences on ol.Offence_id equals o.Id
            //join s in db.Surgeons on ol.Surgeon_id equals s.Id
            // join d in db.Drivers on ol.Driver_id equals d.Id

            var data = (dr, offence, offence_details);
            return View(data);
        }
        [HttpPost]
        public ActionResult Add_Offence(Offence offence,int id)
        {
            int surgeonId = (int)Session["id"];
            Driver driver = (from d in db.Drivers where d.Id == id select d).SingleOrDefault();
            Surgeon sugeon = db.Surgeons.Where(s => s.Id == surgeonId).SingleOrDefault();
            db.Offence_info.Add(new Offence_info { Occuring_date = DateTime.Now.ToString(), Payment_status = "false", Driver = driver, Offence = offence, Surgeon = sugeon });
            _ = db.SaveChanges();
            return RedirectToAction("Search");
        }

        public ActionResult Rules()
        {
            var rules = db.Offences.ToList();
            return View(rules);
        }

        public ActionResult History(int id=0)
        {
            //id need to be dynamic
            int surgeonID = (int)Session["id"];
            var history = (from oi in db.Offence_info where oi.Surgeon_id == surgeonID select oi).ToList();
            return View(history);
        }

        [HttpGet]
        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Report(Notification report)
        {
            //report.Subject = "Emergency Report";
            report.Level = "Admin";
            try
            {
                _ = db.Notifications.Add(report);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception _)
            {
                ViewBag.ErrMsg = "Something Went Wrong";
                return View();
            }
            
        }
    }
}