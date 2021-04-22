using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers {

    //備援KEY-2
    //key=AIzaSyCIV2Z75uPGf9zEAnlAzKS_8bJLdxTvS-k
    //把KEY-1全部取代

    //目前KEY-1
    //key=AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ

    public static class cUtility {
        //key for session
        public static readonly string Current_Ip = "Current_Ip";
        public static readonly string Current_User_Id = "Current_User_Id";
        public static readonly string Current_User_Name = "Current_User_Name";
        public static readonly string Current_User_Validate = "Current_User_Validate"; //之後會驗證是否通過Email驗證
        public static readonly string Current_User_Profile_Image = "Current_User_Profile_Image";
        public static readonly string MerchantTradeNo = "MerchantTradeNo";
        public static readonly string MerchantTradeDate = "MerchantTradeDate";
        public static readonly string balance="balance";
        public static readonly string TradeDesc = "TradeDesc";
        public static readonly string ItemName = "ItemName";
        public static readonly string CheckMacValue = "CheckMacValue";
        public static readonly int[] Moderator_Member_Id = new int[] { 1001 };
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
