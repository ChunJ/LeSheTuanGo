using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
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
