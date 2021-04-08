using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers{
    public class ServiceUseController : Controller{
        private readonly MidtermContext db;
        public ServiceUseController(MidtermContext context){
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
                //ViewData["Address"] = "";
                ViewData["DistrictId"] = 1;
                ViewData["CityId"] = 1;
            }
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }
        
        public string Search(
            int DistrictInput, string addressInput) {
            //var distantMax = db.RangeRefs.Last().RangeInMeters;
            int distanceMax = 3000;
            //disable query and use the first user's location for testing
            DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == DistrictInput)
                .Include(d => d.City).First();
            string address = dist.City.CityName + dist.DistrictName + addressInput;
            var latlong = cUtility.addressToLatlong(address);
            GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);
            //var tempUser = db.Members.First();
            //GeoCoordinate userLocation = new GeoCoordinate((double)tempUser.Latitude, (double)tempUser.Longitude);
            var bagTypeSearch = from o in db.GarbageServiceOffers
                                where o.IsActive == true && 
                                !(o.L3available == 0
                                && o.L5available == 0 && o.L14available == 0
                                && o.L25available == 0 && o.L33available == 0
                                && o.L75available == 0 && o.L120available == 0)
                                select o;
            var newObject = from o in bagTypeSearch.Include(o=>o.District).Include(o=>o.District.City)
                            select new {
                                o.GarbageServiceId,
                                o.DistrictId,
                                o.District.DistrictName,
                                o.District.City.CityName,
                                o.Address,
                                o.EndTime,
                                o.CanGo,
                                o.L3available,
                                o.L5available,
                                o.L14available,
                                o.L25available,
                                o.L33available,
                                o.L75available,
                                o.L120available,
                                o.Latitude,
                                o.Longitude,
                                //put user location in every data, not worth putting it elsewhere when the data count is small.
                                userLat = latlong[0],
                                userLong = latlong[1],
                                Distance = userLocation.GetDistanceTo(new GeoCoordinate((double)o.Latitude, (double)o.Longitude)),
                            };
            var offerList = newObject.AsEnumerable().Where(o => o.Distance <= distanceMax).ToList();
            return JsonConvert.SerializeObject(offerList);
        }
        [HttpPost]
        public IActionResult Join(GarbageServiceUseRecord r) {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) {
                return RedirectToAction("Login", "Member", new { from = "ServiceUse/Index" });
            }
            r.MemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var offer = db.GarbageServiceOffers.Where(o => o.GarbageServiceId == r.GarbageServiceOfferId).First();
            //some testing here need to be done
            offer.L3available -= r.L3count;
            offer.L5available -= r.L5count;
            offer.L14available -= r.L14count;
            offer.L25available -= r.L25count;
            offer.L33available -= r.L33count;
            offer.L75available -= r.L75count;
            offer.L120available -= r.L120count;
            db.Add(r);
            db.SaveChanges();
            return RedirectToAction("Index", "ChatMessageRecords");
        }

        public IActionResult Delete(int id, int id2) {
            var q = db.GarbageServiceUseRecords.Where(m => m.GarbageServiceOfferId == id).FirstOrDefault();
            db.GarbageServiceUseRecords.Remove(q);
            var q2 = db.GarbageServiceOffers.Where(m => m.GarbageServiceId == id2).FirstOrDefault();
            q2.L3available = (byte)(q2.L3available + q.L3count);
            q2.L5available = (byte)(q2.L5available + q.L5count);
            q2.L14available = (byte)(q2.L14available + q.L14count);
            q2.L25available = (byte)(q2.L25available + q.L25count);
            q2.L33available = (byte)(q2.L33available + q.L33count);
            q2.L75available = (byte)(q2.L75available + q.L75count);
            q2.L120available = (byte)(q2.L120available + q.L120count);

            db.SaveChanges();
            return RedirectToAction("HistoryList");
        }
    }
}
