using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public int Memberid = 2;
        readonly GeoCoordinate Geo = null;
        public GarbageServiceUseRecordsController(MidtermContext context)
        {
            var q = from n in (new MidtermContext()).Members
                    where n.MemberId == Memberid
                    select n;
            double userLatitude = (double)q.ToList()[0].Latitude;
            double userLongitude = (double)q.ToList()[0].Longitude;
            Geo=new GeoCoordinate(userLatitude, userLongitude);

            db = context;
        }
        public IActionResult Index()
        {
            //var garbageServiceOffer = db.GarbageServiceOffers.Select(n => n).ToList();
            var q =db.GarbageServiceOffers.Include(m => m.GoRange).Include(m=>m.District).ToList();
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View(q);
            //return View(garbageServiceOffer);

        }
        [HttpPost]
        public IActionResult Index(int distance)
        {
            //var q = from m in (new MidtermContext()).GarbageServiceOffers
            //        where Geo.GetDistanceTo(new GeoCoordinate((double)m.Latitude, (double)m.Longitude)) < distance
            //        select m;
            var q = db.GarbageServiceOffers.Where(m => Geo.GetDistanceTo(new GeoCoordinate((double)m.Latitude, (double)m.Longitude)) < distance);

            return View(q);
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
            var q = db.GarbageServiceUseRecords.Where(m => m.GarbageServiceOfferId == id).FirstOrDefault();
            return View(q);
        }

        [HttpPost]
        public IActionResult Edit(GarbageServiceUseRecord garbageServiceUseRecord)
        {
            if (ModelState.IsValid)
            {
                var q = db.GarbageServiceUseRecords.Where(m => m.ServiceUseRecordId == garbageServiceUseRecord.ServiceUseRecordId).FirstOrDefault();
                q.ServiceUseRecordId = garbageServiceUseRecord.ServiceUseRecordId;
                q.GarbageServiceOfferId = garbageServiceUseRecord.GarbageServiceOfferId;
                q.MemberId = garbageServiceUseRecord.MemberId;
                q.L3count = garbageServiceUseRecord.L3count;
                q.L5count = garbageServiceUseRecord.L5count;
                q.L14count = garbageServiceUseRecord.L14count;
                q.L25count = garbageServiceUseRecord.L25count;
                q.L33count = garbageServiceUseRecord.L33count;
                q.L75count = garbageServiceUseRecord.L75count;
                q.L120count = garbageServiceUseRecord.L120count;
                q.NeedCome = garbageServiceUseRecord.NeedCome;
                q.ComeDistrictId = garbageServiceUseRecord.ComeDistrictId;
                q.ComeAddress = garbageServiceUseRecord.ComeAddress;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(garbageServiceUseRecord);
        }
        public IActionResult Delete(int id,int id2)
        {
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

        [HttpPost]
        public IActionResult JoinGroup(int id2,int garbageserviceofferid, int L3, int L5, int L14, int L25, int L33, int L75, int L120, bool needcome, int comedistrictid, string comeaddress)
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

        public IActionResult HistoryList()
        {
            var q = db.GarbageServiceUseRecords.Include(m=>m.GarbageServiceOffer).Where(m => m.MemberId == Memberid);
            //var q = from n in (new MidtermContext()).GarbageServiceUseRecords
            //        where n.MemberId == Memberid
            //        select n;

            return View(q);
        }
        public string Refresh(string le)
        {
            var q = from n in (new MidtermContext()).RangeRefs
                    where n.RangeId.ToString() == le
                    select n;
            var 距離 = q.ToList().First().RangeInMeters;
            //var q2 = from n in (new MidtermContext()).GarbageServiceOffers.AsEnumerable()
                     
            //         where Geo.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude)) / 1000 <= (double)距離
            //         select n;
              var q2 = db.GarbageServiceOffers.AsEnumerable().Where(m => Geo.GetDistanceTo(new GeoCoordinate((double)m.Latitude, (double)m.Longitude)) / 1000 <= (double)距離);
            var q3 = q2.ToList();
            string ls= JsonConvert.SerializeObject(q3);

            //List<string> disName = new List<string>();
            //for (int i = 0; i < q3.Count; i++)
            //{
            //    var q4 = db.DistrictRefs.Where(n => n.DistrictId == q3[i].DistrictId).Select(n => n.DistrictName);
            //    disName.Add( q4.First());
            //}
            //ViewBag.disName = disName;

            return ls;
        }
       
      


    }
}
