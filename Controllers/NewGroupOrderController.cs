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
            //this view is login user only
            //if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null{
            //    return RedirectToAction("Login");
            //}
            //暫時造假用，設定session裡的 user id
            HttpContext.Session.SetInt32(cUtility.Current_User_Id, 1);

            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //使用使用者的地址
            var user = db.Members.Where(m => m.MemberId == userId).Include(m=>m.District).First();
            ViewData["Address"] = user.Address;
            ViewData["DistrictId"] = user.DistrictId;
            ViewData["CityId"] = user.District.CityId;

            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRangeId"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }

        //see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Index(Order order) {
            order.HostMemberId = 1;
            order.StartTime = DateTime.Now;
            Debug.WriteLine(order.EndTime);
            
            db.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
