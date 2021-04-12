using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using LeSheTuanGo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
//JObject
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//streamreader
using System.IO;
using System.Linq;
//webrequest
using System.Net;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers
{
    public class MapController : Controller
    {
        int MemberId = 1;

        private readonly MidtermContext iv_context;
        public MapController(MidtermContext midtermContext)
        {
            iv_context = midtermContext;
        }

        public string AdjFavorite(int mem, int spot, byte type)
        {
            if (type==0) //+spot
            {
                var AdjSpot = new GarbageSpotLike { MemberId = mem, GarbageTruckSpotId = spot, MinutesBeforeNotify = 0, NotifyMe = false };
                iv_context.Add(AdjSpot);
                iv_context.SaveChanges();
                return "您已成功新增：";
            }
            else //-spot; type=1
            {
                var AdjSpot = iv_context.GarbageSpotLikes.Where(s => s.MemberId == mem && s.GarbageTruckSpotId == spot).First();
                iv_context.Remove(AdjSpot);
                iv_context.SaveChanges();
                return "您已成功移除：";
            }
        }

        public string ifSpotExist(int mem, int spot)
        {
            return "判斷spotId是否已在memberId收藏中";
        }
        
        public string spotCollectedNum(int mem)
        {
            var spotCollected = iv_context.GarbageSpotLikes.Where(s => s.MemberId == mem).OrderBy(s => s.GarbageTruckSpotId).Select(s => s.GarbageTruckSpotId ).ToList();
            string spotJsonString = JsonConvert.SerializeObject(spotCollected);
            return spotJsonString;
        }

        public string spotCollectedtest(int mem)
        {
            var spotCollected = iv_context.GarbageSpotLikes.Where(s => s.MemberId == mem).OrderBy(s => s.GarbageTruckSpotId).Select(s => s.GarbageTruckSpotId).ToList();


            var memberLat = iv_context.Members.Where(m => m.MemberId == mem).First().Latitude;
            var memberLng = iv_context.Members.Where(m => m.MemberId == mem).First().Longitude;


            List<dynamic> qList = new List<dynamic>();
            foreach (int i in spotCollected)
            {
                var spotinfo = iv_context.GarbageTruckSpots.Where(s => s.GarbageTruckSpotId == i).Select(s => new { s.Address, s.ArrivalTime, s.Latitude, s.Longitude, distance = cUtility.distanceBetweenTwoSpots(memberLat, memberLng, s.Latitude, s.Longitude), addLat = memberLat, addLng = memberLng, s.GarbageTruckSpotId }).ToList();
                qList.Add(spotinfo[0]);
            }

            qList.Add(spotCollected);

            //var q = from i in result select new { i.Address, i.ArrivalTime, i.Latitude, i.Longitude, distance = cUtility.distanceBetweenTwoSpots(addressLat, addressLng, i.Latitude, i.Longitude), addLat = addressLat, addLng = addressLng, i.GarbageTruckSpotId };
            //var qList = q.Where(q => q.distance <= 300 && q.distance >= 0).OrderBy(q => q.distance).ToList();

            string listJsonString = JsonConvert.SerializeObject(qList);

            return listJsonString;
            //return "";
        }

        public string spotCollected(int mem)
        {
            var spotCollected = iv_context.GarbageSpotLikes.Where(s => s.MemberId == mem).OrderBy(s=>s.GarbageTruckSpotId).Select(s => s.GarbageTruckSpotId).ToList();


            var memberLat = iv_context.Members.Where(m => m.MemberId == mem).First().Latitude;
            var memberLng = iv_context.Members.Where(m => m.MemberId == mem).First().Longitude;


            List<dynamic> qList=new List<dynamic>();
            foreach (int i in spotCollected)
            {
                var spotinfo = iv_context.GarbageTruckSpots.Where(s => s.GarbageTruckSpotId == i).Select(s => new { s.Address, s.ArrivalTime, s.Latitude, s.Longitude, distance = cUtility.distanceBetweenTwoSpots(memberLat, memberLng, s.Latitude, s.Longitude), addLat = memberLat, addLng = memberLng, s.GarbageTruckSpotId }).ToList();
                qList.Add(spotinfo[0]);
            }

            //var q = from i in result select new { i.Address, i.ArrivalTime, i.Latitude, i.Longitude, distance = cUtility.distanceBetweenTwoSpots(addressLat, addressLng, i.Latitude, i.Longitude), addLat = addressLat, addLng = addressLng, i.GarbageTruckSpotId };
            //var qList = q.Where(q => q.distance <= 300 && q.distance >= 0).OrderBy(q => q.distance).ToList();

            string listJsonString = JsonConvert.SerializeObject(qList);

            return listJsonString;
            //return "";
        }

        public IActionResult collection()
        {
            return View();
        }

        public IActionResult favorite()
        {
            return View();
        }
        public IActionResult Index() {
            ViewData["CityId"] = new SelectList(iv_context.CityRefs, "CityId", "CityName");
            return View();
        }
        public IActionResult searchOld()
        {
            ViewData["CityId"] = new SelectList(iv_context.CityRefs, "CityId", "CityName");
            return View();
        }
        
        public string spotLessThan300M()
        {
            //set 300m for now
            var memberDistrictId = iv_context.Members.Where(m => m.MemberId == MemberId).First().DistrictId;
            var memberLat = iv_context.Members.Where(m => m.MemberId == MemberId).First().Latitude;
            var memberLng = iv_context.Members.Where(m => m.MemberId == MemberId).First().Longitude;

            var result = iv_context.GarbageTruckSpots/*.Where(s => s.DistrictId == memberDistrictId)*/.ToList();

            var q = from i in result select new { i.Address, i.ArrivalTime, i.Latitude, i.Longitude, distance=cUtility.distanceBetweenTwoSpots(memberLat,memberLng,i.Latitude,i.Longitude) };
            var qList = q.Where(q=>q.distance<=300 && q.distance>=0).OrderBy(q => q.distance)./*Take(10).*/ToList();

            string listJsonString = JsonConvert.SerializeObject(qList);

            return listJsonString;
        }

        public string spotLessThan300M2(string address, decimal lat = 0, decimal lng = 0, int memId = 0, string type = "")
        {
            decimal addressLat;
            decimal addressLng;

            if (type == "mem")
            {
                addressLat = iv_context.Members.Where(m => m.MemberId == memId).First().Latitude;
                addressLng = iv_context.Members.Where(m => m.MemberId == memId).First().Longitude;

            }
            else if (type == "nav")
            {
                addressLat = lat;
                addressLng = lng;
            }
            else  //type=="add"
            {
                var addressLoc = cUtility.addressToLatlong(address);

                addressLat = addressLoc[0];
                addressLng = addressLoc[1];
            }
            

            var result = iv_context.GarbageTruckSpots.ToList();

            var q = from i in result select new { i.Address, i.ArrivalTime, i.Latitude, i.Longitude, distance = cUtility.distanceBetweenTwoSpots(addressLat, addressLng, i.Latitude, i.Longitude), addLat=addressLat, addLng=addressLng, i.GarbageTruckSpotId };
            var qList = q.Where(q => q.distance <= 300 && q.distance >= 0).OrderBy(q => q.distance).ToList();

            string listJsonString = JsonConvert.SerializeObject(qList);

            return listJsonString;
        }

        public IActionResult searchByMember()
        {
            var memberDistrictId = iv_context.Members.Where(m => m.MemberId == MemberId).First().DistrictId;

            var result = iv_context.GarbageTruckSpots.Where(s => s.DistrictId == memberDistrictId).Take(3).ToList();

            ViewBag.result = result;

            ViewBag.resultLat = result[0].Latitude;
            ViewBag.resultLng = result[0].Longitude;
            ViewBag.resultNam = result[0].Address;

            List<GarbageTruckSpotViewModel> garbageTruckSpotViewModels = new List<GarbageTruckSpotViewModel>();

            foreach (GarbageTruckSpot r in result)
            {
                GarbageTruckSpotViewModel garbageTruckSpotViewModel = new GarbageTruckSpotViewModel(r);
                garbageTruckSpotViewModels.Add(garbageTruckSpotViewModel);
            }

            //return View(garbageTruckSpotViewModels);
            ViewData["CityId"] = new SelectList(iv_context.CityRefs, "CityId", "CityName");
            return View();
        }
        
        //input address and output coordinate[lat, lng] 
        public List<decimal> addressToCoordinate(string address)
        {
            List<decimal> coordinate = new List<decimal>(2);

            //google api key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ
            //目前固定放，之後可加字典檔、備援的額外key，try/catch
            WebRequest request = WebRequest.Create($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ");
            request.Method = "GET";
            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var jsonobj = JObject.Parse(result);
                coordinate.Add(Decimal.Parse(jsonobj["results"][0]["geometry"]["location"]["lat"].ToString()));
                coordinate.Add(Decimal.Parse(jsonobj["results"][0]["geometry"]["location"]["lng"].ToString()));
            }
            
            //目前回傳的座標，小數位數(小數點後7位)比資料庫要求的(小數點後6位)多，要測試是否資料庫可以自動處理；
            //ex. 手動在資料庫輸入超過6位、或不足6位，資料庫會自動調整四捨五入、或補0
            return coordinate;
        }

        //input spots' lat/lng and output distance in meter(s)
        //lat/lng=緯度/經度
        public short distanceBetweenTwoSpots(decimal latA, decimal lngA, decimal latB, decimal lngB)
        {
            GeoCoordinate spotA = new GeoCoordinate((double)latA, (double)lngA);
            GeoCoordinate spotB = new GeoCoordinate((double)latB, (double)lngB);
            
            //follow db type: short
            short distance = (short)spotA.GetDistanceTo(spotB);
            return distance;
        }
    }
}
