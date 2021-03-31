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
        private Member currentMember = null;
        private GeoCoordinate Geo = null;
        public GarbageServiceUseController(MidtermContext context){
            db = context;
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                int Memberid = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                currentMember = db.Members.Where(m => m.MemberId == userId).First();
            }
        }
        public IActionResult Index(){
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }
        public string search(
            int districtInput,
            string addressInput,
            int rangeIdInput,
            bool l3,
            bool l5,
            bool l14,
            bool l25,
            bool l33,
            bool l75,
            bool l120) {
            var distant = db.RangeRefs.Where(r => r.RangeId == rangeIdInput).First().RangeInMeters;
            DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == districtInput)
                .Include(d => d.City).First();
            string address = dist.City.CityName + dist.DistrictName + addressInput;
            var latlong = cUtility.addressToLatlong(address);
            GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);

            var bagTypeSearch = from n in (new MidtermContext()).Orders.AsEnumerable()
                                where userLocation.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude)) / 1000 <= (double)distant
                                select n;

            var rangeSearch = from n in bagTypeSearch.AsEnumerable()
                     where userLocation.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude)) / 1000 <= (double)distant
                              select n;
            var A = bagTypeSearch.ToList();
            string ls = JsonConvert.SerializeObject(A);

            return ls;
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
