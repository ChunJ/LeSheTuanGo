using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly MidtermContext db;
        public HomeController(MidtermContext context)
        {
            db = context;
        }


        public IActionResult Index() {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null)
            {
                int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                var user = db.Members.Where(m => m.MemberId == userId).FirstOrDefault();
                ViewBag.Name = user.LastName;
                ViewBag.Image = user.ProfileImagePath;
                ViewData
            }
            else
            {
                ViewBag.Name = "訪客";
                ViewBag.Image = "/images/profilePic.jpg";
            }

            return View();
        }
        public IActionResult OrderNavi() {
            return View();
        }
        public IActionResult ServiceNavi() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
