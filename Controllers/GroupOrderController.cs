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
    public class GroupOrderController : Controller {
        private readonly MidtermContext db;
        public GroupOrderController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member",new {from = "GroupOrder/Index" });
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //使用使用者的地址
            var user = db.Members.Where(m => m.MemberId == userId).Include(m=>m.District).First();
            ViewData["Address"] = user.Address;
            ViewData["DistrictId"] = user.DistrictId;
            ViewData["CityId"] = user.District.CityId;

            Product p = db.Products.First();
            ViewData["CategoryId"] = p.CategoryId;
            ViewData["ProductId"] = p.ProductId;
            ViewData["ProductImage"] = p.ProductImagePath;
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }

        //see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Index(Order order) {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member");
            order.HostMemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            order.StartTime = DateTime.Now;
            DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == order.DistrictId)
                .Include(d => d.City).First();
            string address = dist.City.CityName + dist.DistrictName + order.Address;
            //need await or async
            //https://stackoverflow.com/questions/30419739/return-to-view-with-async-await
            var latlong = cUtility.addressToLatlong(address);
            order.Latitude = latlong[0];
            order.Longitude = latlong[1];
            order.AvailableCount = order.MaxCount;
            order.IsActive = true;
            if (order.OrderDescription == null) order.OrderDescription = "";
            db.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "ChatMessageRecords", new { grouptype = 1, groupid = order.OrderId });
        }

        public IActionResult List() {
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var list = db.Orders.Where(o => o.HostMemberId == userId).ToList();
            return View(list);
        }
    }
}
