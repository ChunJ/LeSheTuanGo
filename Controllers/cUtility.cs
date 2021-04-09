using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers {
    public static class cUtility {
        //key for session
        public static readonly string Current_User_Id = "Current_User_Id";
        public static readonly string Current_User_Name = "Current_User_Name";
        public static readonly string Current_User_Validate = "Current_User_Validate"; //之後會驗證是否通過Email驗證
        public static readonly string Current_User_Profile_Image = "Current_User_Profile_Image";

        //input address and output coordinate[lat, lng] 
        public static decimal[] addressToLatlong(string address) {
            //google api key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ
            //目前固定放，之後可加備援的額外key，try/catch
            WebRequest request = WebRequest.Create($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ");
            request.Method = "GET";
            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream())) {
                var result = streamReader.ReadToEnd();
                var location = (Newtonsoft.Json.Linq.JObject.Parse(result))["results"][0]["geometry"]["location"];
                return new decimal[2] { 
                    Decimal.Parse(location["lat"].ToString()), 
                    Decimal.Parse(location["lng"].ToString())
                };
            }   
        }

        //input spots' lat/lng and output distance in meter(s) lat/lng=緯度/經度
        public static short distanceBetweenTwoSpots(decimal latA, decimal lngA, decimal latB, decimal lngB) {
            GeoCoordinate spotA = new GeoCoordinate((double)latA, (double)lngA);
            GeoCoordinate spotB = new GeoCoordinate((double)latB, (double)lngB);

            //follow db type: short
            short distance = (short)spotA.GetDistanceTo(spotB);
            return distance;
        }

        public static void newNotofication(List<int> memberID,string notificationContent,int groupType,int groupID)
        {
            Notification notification = new Notification();

            foreach (int id in memberID)
            {

            }
        }
    }
}
