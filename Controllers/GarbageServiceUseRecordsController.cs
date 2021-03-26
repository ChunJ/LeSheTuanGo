using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




namespace CoreMVC.Controllers
{
    public class GarbageServiceUseRecordsController : Controller
    {
        private readonly MidtermContext db;
        public int Memberid = 3;
        readonly GeoCoordinate Geo = null;
        public GarbageServiceUseRecordsController(MidtermContext context)
        {
            //var q = from n in (new MidtermContext()).Members
            //        where n.MemberId == Memberid
            //        select n;
            //double userLatitude = (double)q.ToList()[0].Latitude;
            //double userLongitude = (double)q.ToList()[0].Longitude;
            //= new GeoCoordinate(userLatitude, userLongitude);
            //A
            db = context;
        }
        public IActionResult Index()
        {
            var garbageServiceOffer = db.GarbageServiceOffers.Select(n => n).ToList();

            return View(garbageServiceOffer);

        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(GarbageServiceOffer garbageServiceOffer)
        {
            db.GarbageServiceOffers.Add(garbageServiceOffer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var garbageServiceOffer = db.GarbageServiceOffers.Where(m => m.HostMemberId == id).FirstOrDefault();
            return View(garbageServiceOffer);
        }

        [HttpPost]
        public IActionResult Edit(GarbageServiceOffer garbageServiceOffer)
        {
            if (ModelState.IsValid)
            {
                var modify = db.GarbageServiceOffers.Where(m => m.HostMemberId == garbageServiceOffer.HostMemberId).FirstOrDefault();
                modify.ServiceTypeId = garbageServiceOffer.ServiceTypeId;
                modify.HostMemberId = garbageServiceOffer.HostMemberId;
                modify.DistrictId = garbageServiceOffer.DistrictId;
                modify.Address = garbageServiceOffer.Address;
                modify.StartTime = garbageServiceOffer.StartTime;
                modify.EndTime = garbageServiceOffer.EndTime;
                modify.IsActive = garbageServiceOffer.IsActive;
                modify.Latitude = garbageServiceOffer.Latitude;
                modify.Longitude = garbageServiceOffer.Longitude;
                modify.CanGo = garbageServiceOffer.CanGo;
                modify.GoRange = garbageServiceOffer.GoRange;
                modify.L3maxCount = garbageServiceOffer.L3maxCount;
                modify.L5maxCount = garbageServiceOffer.L5maxCount;
                modify.L14maxCount = garbageServiceOffer.L14maxCount;
                modify.L25maxCount = garbageServiceOffer.L25maxCount;
                modify.L33maxCount = garbageServiceOffer.L33maxCount;
                modify.L75maxCount = garbageServiceOffer.L75maxCount;
                modify.L120maxCount = garbageServiceOffer.L120maxCount;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(garbageServiceOffer);
        }
        public IActionResult Delete(int id)
        {
            var garbageServiceOffer = db.GarbageServiceOffers.Where(m => m.GarbageServiceId == id).FirstOrDefault();
            db.GarbageServiceOffers.Remove(garbageServiceOffer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult JoinGroup(int id)
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

        public IActionResult HistoryList()
        {
            var q = db.GarbageServiceUseRecords.Where(m => m.MemberId == Memberid);
            //var q = from n in (new MidtermContext()).GarbageServiceUseRecords
            //        where n.MemberId == Memberid
            //        select n;

            return View(q);
        }
        public string Refresh(string re)
        {
            var q = from n in (new MidtermContext()).RangeRefs
                    where n.RangeId.ToString() == re
                    select n;
            var 距離 = q.ToList().First().RangeInMeters;
            var q2 = from n in (new MidtermContext()).Orders.AsEnumerable()
                     where Geo.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude)) / 1000 <= (double)距離
                     select n;
            var A = q2.ToList();
            string ls = JsonConvert.SerializeObject(A);

            return ls;
        }
        [HttpPost]
        public IActionResult JoinGroup(int garbageserviceofferid, int L3, int L5, int L14, int L25, int L33, int L75, int L120, bool needcome, int comedistrictid, string comeaddress)
        {
            GarbageServiceUseRecord GB = new GarbageServiceUseRecord();
            GB.L3count = (Byte)L3;
            GB.L5count = (Byte)L5;
            GB.L14count = (Byte)L14;
            GB.L25count = (Byte)L25;
            GB.L33count = (Byte)L33;
            GB.L75count = (Byte)L75;
            GB.L120count = (Byte)L120;
            GB.MemberId = Memberid;
            GB.ComeAddress = comeaddress;
            GB.NeedCome = needcome;
            GB.ComeDistrictId = (short)comedistrictid;
            GB.GarbageServiceOfferId = garbageserviceofferid;

            db.Add(GB);
            db.SaveChanges();
            //var q = db.GarbageServiceUseRecords.Where(m => m.ServiceUseRecordId == id);
            //return View(q);


            //ViewData["L3count"] = (db.GarbageServiceUseRecords, "L3MaxCount");
            return RedirectToAction("Index");
        }
        //public IActionResult GarbageServiceUseRecordsViewModel()
        //{
        //    ViewData["L3count"] = new SelectList(db.GarbageServiceUseRecords, "L3MaxCount");
        //}


    }
}
