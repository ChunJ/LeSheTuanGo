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
            //如果使用者有登入，載入使用者地址
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
            //載入其他資訊
            Product p = db.Products.First();
            ViewData["CategoryId"] = p.CategoryId;
            ViewData["ProductId"] = p.ProductId;
            ViewData["ProductImage"] = p.ProductImagePath;
            ViewData["GoRange"] = new SelectList(db.RangeRefs, "RangeId", "RangeInMeters");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            return View();
        }

        public void editOrderOffer(int OrderId,Order order) {   
            //取得被修改資料"修改前"的可購買數和購買上限
            var o = db.Orders.Where(n => n.OrderId == OrderId).Select(n=>new { n.AvailableCount,n.MaxCount }).First();
            //計算團購修改後新的可購買數
            order.AvailableCount = (byte)(o.AvailableCount + (order.MaxCount - o.MaxCount));
            //取得新地址的經緯度
            decimal[] s = cUtility.addressToLatlong(order.Address);
            order.Latitude = s[0];
            order.Longitude = s[1];
            //寫入資料庫
            db.Update(order);
            db.SaveChanges();
            //Console.WriteLine("A");
        }

        public string Search(int DistrictInput, string addressInput) {
            //todo search by endtime & price ?

            //設定搜尋範圍
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
            //若使用者有登入，則先過濾掉使用者自己新增的團購
            IQueryable<Order> firstPass = db.Orders;
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) != null) {
                firstPass = db.Orders.Where(o => o.HostMemberId != HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value);
            }
            //過濾掉已結束和搜尋範圍外的結果並回傳json
            var newObject = from o in firstPass.Include(o => o.District).Include(o => o.District.City).Include(o=>o.GoRange)
                            where o.IsActive == true
                            select new {
                                o.OrderId,
                                o.ProductId,
                                o.DistrictId,
                                o.District.DistrictName,
                                o.District.City.CityName,
                                o.Address,
                                o.EndTime,
                                o.CanGo,
                                o.GoRange.RangeInMeters,
                                o.AvailableCount,
                                o.Latitude,
                                o.Longitude,
                                o.HostMemberId,
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
        public IActionResult Join(OrderBuyRecord r) {
            //未登入只能搜尋團購，不能使用(加入)，若按下加入後會跳轉登入頁
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) {
                return RedirectToAction("Login", "Member", new { from = "Buy/Index" });
            }
            //若外送地址未填寫(null)，則填入空字串
            r.MemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            if (r.ComeAddress == null) r.ComeAddress = "";
            //todo need server side validation
            //取得被加入的團購物件，可購買數量 - 被購買數量
            //加入購買紀錄物件，將修改寫入資料庫
            var order = db.Orders.Where(o => o.OrderId == r.OrderId).First();
            order.AvailableCount -= r.Count;
            db.Add(r);
            db.SaveChanges();
            //if server side validation is not passed, return view with user's filter option
            //if successed, redirect to history
            //傳到新加入團購的聊天室頁面
            return RedirectToAction("Index", "ChatMessageRecords", new { grouptype = 1, groupid = r.OrderId });
        }
    }
}
