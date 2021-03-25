using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System.Text;

// add session to use useid
namespace LeSheTuanGo.Controllers {
    public class NewGroupOrderController : Controller {
        private readonly MidtermContext db;
        public NewGroupOrderController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            int userId = 1;//temp
            Member user = db.Members.Where(m => m.MemberId == userId).First();
            ViewData["Address"] = user.Address;
            //ViewData["DistrictId"] = user.DistrictId;
            //ViewData["CityId"] = user.District.CityId;
            //Console.WriteLine(user.District.CityId.ToString());
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View();
        }
    }
}
