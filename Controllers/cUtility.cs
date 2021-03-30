using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers {
    public static class cUtility {
        //key for session
        public static readonly string Current_User_Id = "Current_User_Id";

        //input address and output coordinate[lat, lng] 
        public static decimal[] addressToCoordinate(string address) {
            //google api key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ
            //目前固定放，之後可加備援的額外key，try/catch
            WebRequest request = WebRequest.Create($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ");
            request.Method = "GET";
            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream())) {
                var result = streamReader.ReadToEnd();
                var location = (Newtonsoft.Json.Linq.JObject.Parse(result))["results"][0]["geometry"]["location"];
                //目前回傳的座標，小數位數(小數點後7位)比資料庫要求的(小數點後6位)多，要測試是否資料庫可以自動處理；
                //ex. 手動在資料庫輸入超過6位、或不足6位，資料庫會自動調整四捨五入、或補0
                return new decimal[2] { 
                    Decimal.Parse(location["lat"].ToString()), 
                    Decimal.Parse(location["lng"].ToString())
                };
            }   
        }

        //input spots' lat/lng and output distance in meter(s)
        //lat/lng=緯度/經度
        public static short distanceBetweenTwoSpots(decimal latA, decimal lngA, decimal latB, decimal lngB) {
            GeoCoordinate spotA = new GeoCoordinate((double)latA, (double)lngA);
            GeoCoordinate spotB = new GeoCoordinate((double)latB, (double)lngB);

            //follow db type: short
            short distance = (short)spotA.GetDistanceTo(spotB);
            return distance;
        }
    }
}
