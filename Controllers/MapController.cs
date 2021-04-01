﻿using GeoCoordinatePortable;
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

        public IActionResult search()
        {
            ViewData["DistrictId"] = new SelectList(iv_context.DistrictRefs, "DistrictId", "DistrictName");
            ViewData["CityId"] = new SelectList(iv_context.CityRefs, "CityId", "CityName");
            return View();
        }
        
        public string spotLessThan100M()
        {
            //set 100m for now
            var memberDistrictId = iv_context.Members.Where(m => m.MemberId == MemberId).First().DistrictId;
            var memberLat = iv_context.Members.Where(m => m.MemberId == MemberId).First().Latitude;
            var memberLng = iv_context.Members.Where(m => m.MemberId == MemberId).First().Longitude;

            var result = iv_context.GarbageTruckSpots/*.Where(s => s.DistrictId == memberDistrictId)*/.ToList();

            var q = from i in result select new { i.Address, i.ArrivalTime, i.Latitude, i.Longitude, distance=cUtility.distanceBetweenTwoSpots(memberLat,memberLng,i.Latitude,i.Longitude) };
            var qList = q.Where(q=>q.distance<=300 && q.distance>=0).OrderBy(q => q.distance)./*Take(10).*/ToList();

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
            return View();
        }

        public IActionResult Index()
        {
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
