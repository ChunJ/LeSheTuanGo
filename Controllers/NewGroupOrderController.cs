using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

// add session to use useid
namespace LeSheTuanGo.Controllers {
    public class NewGroupOrderController : Controller {
        private readonly MidtermContext db;
        public NewGroupOrderController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            HttpContext.Session.SetInt32(cDictionary.Current_User_Id, 1);
            int userId;
            if (HttpContext.Session.GetInt32(cDictionary.Current_User_Id) != null) {
                userId = HttpContext.Session.GetInt32(cDictionary.Current_User_Id).Value;
                Member user = db.Members.Where(m => m.MemberId == userId).First();
                ViewData["Address"] = user.Address;
                ViewData["DistrictId"] = user.DistrictId;
                Debug.WriteLine(user.District);
                //ViewData["CityId"] = user.District.CityId;
            } else {
                ViewData["Address"] = "";
                ViewData["DistrictId"] = 1;
                ViewData["CityId"] = 1;
            }
            //Console.WriteLine(user.District.CityId.ToString());
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View();
        }
    }
}
