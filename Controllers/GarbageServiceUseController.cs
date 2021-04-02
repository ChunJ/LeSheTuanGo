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
    public class GarbageServiceUseController : Controller{
        private readonly MidtermContext db;
        public GarbageServiceUseController(MidtermContext context){
            db = context;
        }
        public IActionResult Index(){
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
            //DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == DistrictInput)
            //    .Include(d => d.City).First();
            //string address = dist.City.CityName + dist.DistrictName + addressInput;
            //var latlong = cUtility.addressToLatlong(address);
            //GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);
            var tempUser = db.Members.First();
            GeoCoordinate userLocation = new GeoCoordinate((double)tempUser.Latitude, (double)tempUser.Longitude);
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
                                Distance = userLocation.GetDistanceTo(new GeoCoordinate((double)o.Latitude, (double)o.Longitude)),
                            };
            var offerList = newObject.AsEnumerable().Where(o => o.Distance <= distanceMax).ToList();
            string jsonString = JsonConvert.SerializeObject(offerList);
            return jsonString;
        }

        public IActionResult Join(int id)
        {
            var q = db.GarbageServiceOffers.Where(m => m.GarbageServiceId == id);
            var q2 = q.FirstOrDefault();

            ViewData["L3count"] = q2.L3maxCount;
            ViewData["L5count"] = q2.L5maxCount;
            ViewData["L14count"] = q2.L14maxCount;
            ViewData["L25count"] = q2.L25maxCount;
            ViewData["L33count"] = q2.L33maxCount;
            ViewData["L75count"] = q2.L75maxCount;
            ViewData["L120count"] = q2.L120maxCount;

            return View(q);
            //var q = from n in (new MidtermContext()).GarbageServiceOffers
            //        where n.GarbageServiceId == id
            //        select n;
            //return View(q);

        }

        [HttpPost]
        public IActionResult Join(int id2,int garbageserviceofferid, int L3, int L5, int L14, int L25, int L33, int L75, int L120, bool needcome, int comedistrictid, string comeaddress)
        {
            GarbageServiceUseRecord GB = new GarbageServiceUseRecord();
            GB.L3count = (Byte)L3;
            GB.L5count = (Byte)L5;
            GB.L14count = (Byte)L14;
            GB.L25count = (Byte)L25;
            GB.L33count = (Byte)L33;
            GB.L75count = (Byte)L75;
            GB.L120count = (Byte)L120;
            GB.MemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            GB.ComeAddress = comeaddress;
            GB.NeedCome = needcome;
            GB.ComeDistrictId = (short)comedistrictid;
            GB.GarbageServiceOfferId = garbageserviceofferid;

            db.Add(GB);
            var q = db.GarbageServiceOffers.Where(m => m.GarbageServiceId == id2).FirstOrDefault();
            q.L3available =(byte)(q.L3maxCount - GB.L3count);
            q.L5available = (byte)(q.L5maxCount - GB.L5count);
            q.L14available = (byte)(q.L14maxCount - GB.L14count);
            q.L25available = (byte)(q.L25maxCount - GB.L25count);
            q.L33available = (byte)(q.L33maxCount - GB.L33count);
            q.L75available = (byte)(q.L75maxCount - GB.L75count);
            q.L120available = (byte)(q.L120maxCount - GB.L120count);
            db.SaveChanges();

            //var q = db.GarbageServiceUseRecords.Where(m => m.ServiceUseRecordId == id);
            //return View(q);

         
            return RedirectToAction("Index");
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
        public IActionResult HistoryList()
        {
            var q = db.GarbageServiceUseRecords.Include(m=>m.GarbageServiceOffer).Where(m => m.MemberId == HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value);
            //var q = from n in (new MidtermContext()).GarbageServiceUseRecords
            //        where n.MemberId == Memberid
            //        select n;

            return View(q);
        }
    }
}
