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
using Microsoft.EntityFrameworkCore;

// add session to use useid
namespace LeSheTuanGo.Controllers {
    public class NewGroupOrderController : Controller {
        private readonly MidtermContext db;
        public NewGroupOrderController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            //暫時造假用，設定session裡的 user id
            HttpContext.Session.SetInt32(cUtility.Current_User_Id, 1);

            int userId;
            //若有登入則使用使用者資訊，否則手動給預設值。
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                var user = db.Members.Where(m => m.MemberId == userId).Include(m=>m.District).First();
                ViewData["Address"] = user.Address;
                ViewData["DistrictId"] = user.DistrictId;
                ViewData["CityId"] = user.District.CityId;
            } else {
                ViewData["Address"] = "";
                ViewData["DistrictId"] = 1;
                ViewData["CityId"] = 1;
            }
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRangeId"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }

        //see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Index(Order order) {
            order.EndTime = DateTime.Now;
            order.StartTime = DateTime.Now;
            Debug.WriteLine(order.EndTime);
            //db.Add(order);
            //await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
