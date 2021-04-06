using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using LeSheTuanGo.ViewModels;
using GeoCoordinatePortable;

namespace LeSheTuanGo.Controllers {
    public class BuyController : Controller {
        private readonly MidtermContext db;
        public BuyController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                int Memberid = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                var user = db.Members.Where(m => m.MemberId == Memberid).Include(m => m.District).First();
                ViewData["Address"] = user.Address;
                ViewData["DistrictId"] = user.DistrictId;
                ViewData["CityId"] = user.District.CityId;
            } else {
                ViewData["DistrictId"] = 1;
                ViewData["CityId"] = 1;
            }
            ViewData["CategoryId"] = 1;
            ViewData["ProductId"] = 1;
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            return View();
        }
        public string Search(int DistrictInput, string addressInput) {
            //search by endtime & price ?
            //add gorange in sent data

            //var distantMax = db.RangeRefs.Last().RangeInMeters;
            int distanceMax = 3000;
            //disable query and use the first user's location for testing
            //DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == DistrictInput)
            //    .Include(d => d.City).First();
            //string address = dist.City.CityName + dist.DistrictName + addressInput;
            //var latlong = cUtility.addressToLatlong(address);
            //GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);
            var tempUser = db.Members.First();
            GeoCoordinate userLocation = new GeoCoordinate((double)tempUser.Latitude, (double)tempUser.Longitude);
            var newObject = from o in db.Orders.Include(o => o.District).Include(o => o.District.City)
                            select new {
                                o.OrderId,
                                o.ProductId,
                                o.DistrictId,
                                o.District.DistrictName,
                                o.District.City.CityName,
                                o.Address,
                                o.EndTime,
                                o.CanGo,
                                o.AvailableCount,
                                o.Latitude,
                                o.Longitude,
                                Distance = userLocation.GetDistanceTo(new GeoCoordinate((double)o.Latitude, (double)o.Longitude)),
                            };
            var offerList = newObject.AsEnumerable().Where(o => o.Distance <= distanceMax).ToList();
            return JsonConvert.SerializeObject(offerList);
        }
        [HttpPost]
        public IActionResult Join(OrderBuyRecord r) {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) {
                return RedirectToAction("Login", "Member", new { from = "Buy/Index" });
            }
            int MemberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //need server side validation
            var order = db.Orders.Where(o => o.OrderId == r.OrderId).First();
            order.AvailableCount -= r.Count;
            db.Add(r);
            db.SaveChanges();
            //if server side validation is not passed, return view with user's filter option
            //if successed, redirect to history
            //todo 修改max總數
            return RedirectToAction("Index");
        }
        public IActionResult HistoryList() {
            var q = from n in (new MidtermContext()).OrderBuyRecords
                    where n.MemberId == HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value
                    select n;
            return View(q);
        }
    }
}
