using GeoCoordinatePortable;
using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers
{
    public class XamarinController : Controller
    {

        private readonly MidtermContext db;
        public XamarinController(MidtermContext context)
        {
            db = context;
        }
        public JsonResult Searchdb()
        {
            var q = db.Members.Select(n => n);
            return Json(q.ToList());
        }
        public string Login(string email , string password)
        {
            string js = "";
            var qMember = db.Members.Where(n => n.Email == email).FirstOrDefault();
            if (qMember == null)
            {
                js = JsonConvert.SerializeObject("Fail");
                return js;
            }
            string sha256Password = sha256(password, qMember.PasswordSalt);
            if (sha256Password != qMember.Password)
            {
                js = JsonConvert.SerializeObject("Fail");
                return js;
            }
            //var qDistrict = db.DistrictRefs.Where(n => n.DistrictId == qMember.DistrictId).First();
            //var qCity = db.CityRefs.Where(n => n.CityId == qDistrict.CityId).First();
            //qMember.Address = qCity.CityName + qDistrict.DistrictName + qMember.Address;
            js = JsonConvert.SerializeObject(qMember);
            return js;
        }
        public string getSpot()
        {
            var q = from n in db.GarbageTruckSpots
                    //where n.RouteId == 1
                    select n;
            var js = JsonConvert.SerializeObject(q.ToList());
            return js;
        }
        private string sha256(string inputPwd, string salt)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(inputPwd + salt);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            return result;//輸出結果
        }
        private string getLength(string Address , int length)
        {
            var latlong = cUtility.addressToLatlong(Address);
            GeoCoordinate userLocation = new GeoCoordinate((double)latlong[0], (double)latlong[1]);
            //var tempUser = db.Members.First();
            //GeoCoordinate userLocation = new GeoCoordinate((double)tempUser.Latitude, (double)tempUser.Longitude);
            var newObject = from o in db.Orders.Include(o => o.District).Include(o => o.District.City)
                            select new
                            {
                                o.OrderId,
                                o.ProductId,
                                o.DistrictId,
                                o.District.DistrictName,
                                o.District.City.CityName,
                                o.Address,
                                o.EndTime,
                                o.CanGo,
                                o.AvailableCount,
                                o.Latitude,
                                o.Longitude,
                                //put user location in every data, not worth putting it elsewhere when the data count is small.
                                userLat = latlong[0],
                                userLong = latlong[1],
                                Distance = userLocation.GetDistanceTo(new GeoCoordinate((double)o.Latitude, (double)o.Longitude)),
                            };
            var offerList = newObject.AsEnumerable().Where(o => o.Distance <= length).ToList();
            return JsonConvert.SerializeObject(offerList);

        }

    }
}
