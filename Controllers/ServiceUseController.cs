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
            //若使用者有登入，載入使用者地址資訊
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                int Memberid = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
                var user = db.Members.Where(m => m.MemberId == Memberid).Include(m => m.District).First();
                ViewData["Address"] = user.Address;
                ViewData["DistrictId"] = user.DistrictId;
                ViewData["CityId"] = user.District.CityId;
            } else {
                //否則預設為台北市中正區
                ViewData["DistrictId"] = 1;
                ViewData["CityId"] = 1;
            }
            //載入其他資料
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            return View();
        }
        
        public string Search(
            int DistrictInput, string addressInput) {
            //設定最大搜尋範圍(寫死為3000)
            //var distantMax = db.RangeRefs.Last().RangeInMeters;
            int distanceMax = 3000;
            //從參數求出完整地址，並轉成經緯度
            DistrictRef dist = db.DistrictRefs.Where(d => d.DistrictId == DistrictInput)
                .Include(d => d.City).First();
            string address = dist.City.CityName + dist.DistrictName + addressInput;
            var latlong = cUtility.addressToLatlong(address);
            GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);
            //disable query and use the first user's location for testing
            //var tempUser = db.Members.First();
            //GeoCoordinate userLocation = new GeoCoordinate((double)tempUser.Latitude, (double)tempUser.Longitude);
            //若使用者有登入，則先過濾掉使用者自己新增的服務
            IQueryable<GarbageServiceOffer> firstPass = db.GarbageServiceOffers;
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                firstPass = db.GarbageServiceOffers.Where(o => o.HostMemberId!= HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value);
            }
            //過濾到期、已額滿或在搜尋範圍外的服務，並轉成json
            var bagTypeSearch = from o in firstPass
                                where o.IsActive == true && 
                                !(o.L3available == 0
                                && o.L5available == 0 && o.L14available == 0
                                && o.L25available == 0 && o.L33available == 0
                                && o.L75available == 0 && o.L120available == 0)
                                select o;
            var newObject = from o in bagTypeSearch.Include(o=>o.District).Include(o=>o.District.City).Include(o=>o.GoRange)
                            select new {
                                o.GarbageServiceId,
                                o.DistrictId,
                                o.District.DistrictName,
                                o.District.City.CityName,
                                o.Address,
                                o.EndTime,
                                o.CanGo,
                                o.GoRange.RangeInMeters,
                                o.L3available,
                                o.L5available,
                                o.L14available,
                                o.L25available,
                                o.L33available,
                                o.L75available,
                                o.L120available,
                                o.Latitude,
                                o.Longitude,
                                //目前先將查詢位置的經緯度存入每筆資料中
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
            ////未登入只能搜尋服務，不能使用(加入)，若按下加入會跳轉登入頁
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) {
                return RedirectToAction("Login", "Member", new { from = "ServiceUse/Index" });
            }
            //填入使用者id到使用紀錄
            r.MemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //取得被使用服務
            var offer = db.GarbageServiceOffers.Where(o => o.GarbageServiceId == r.GarbageServiceOfferId).First();
            //some testing here need to be done
            //將被使用服務的可用袋數減去被使用袋數
            offer.L3available -= r.L3count;
            offer.L5available -= r.L5count;
            offer.L14available -= r.L14count;
            offer.L25available -= r.L25count;
            offer.L33available -= r.L33count;
            offer.L75available -= r.L75count;
            offer.L120available -= r.L120count;
            //加入使用紀錄，將修改寫入資料庫
            db.Add(r);
            db.SaveChanges();
            //傳到新加入服務團的聊天室頁面
            return RedirectToAction("Index", "ChatMessageRecords", new { grouptype = 2, groupid = r.GarbageServiceOfferId });
        }

        //編輯加團內容
        public void EditServiceUse(int garbageServiceID, GarbageServiceUseRecord garbageServiceUseRecord)
        {
            var g = db.GarbageServiceUseRecords.Where(n => n.GarbageServiceOfferId == garbageServiceID).Select(n => new
            {
                
            }).ToList();

            //if (garbageServiceID == garbageServiceUseRecord.GarbageServiceId)
            //{
            //    decimal[] s = cUtility.addressToLatlong(garbageServiceUseRecord.Address);
            //    garbageServiceUseRecord.Latitude = s[0];
            //    garbageServiceUseRecord.Longitude = s[1];
            //    garbageServiceUseRecord.L3available = (byte)(g[0].L3available + (garbageServiceUseRecord.L3maxCount - g[0].L3maxCount));
            //    garbageServiceUseRecord.L5available = (byte)(g[0].L5available + (garbageServiceUseRecord.L5maxCount - g[0].L5maxCount));
            //    garbageServiceUseRecord.L14available = (byte)(g[0].L14available + (garbageServiceUseRecord.L14maxCount - g[0].L14maxCount));
            //    garbageServiceUseRecord.L25available = (byte)(g[0].L25available + (garbageServiceUseRecord.L25maxCount - g[0].L25maxCount));
            //    garbageServiceUseRecord.L33available = (byte)(g[0].L33available + (garbageServiceUseRecord.L33maxCount - g[0].L33maxCount));
            //    garbageServiceUseRecord.L75available = (byte)(g[0].L75available + (garbageServiceUseRecord.L75maxCount - g[0].L75maxCount));
            //    garbageServiceUseRecord.L120available = (byte)(g[0].L120available + (garbageServiceUseRecord.L120maxCount - g[0].L120maxCount));
            //    db.Update(garbageServiceUseRecord);
            //    db.SaveChanges();
            //}
        }
        
        public IActionResult Delete(int id, int id2) {
            //目前沒用到
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
