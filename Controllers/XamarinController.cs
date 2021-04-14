﻿using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
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
        public JsonResult Mlogin(string email, string password)
        {
            List<string> ls = new List<string>();
            var qMember = db.Members.Where(n => n.Email == email).FirstOrDefault();
            if (qMember == null)
            {
                ls.Add("fail");
                return Json(ls);
            }
            string sha256Password = sha256(password, qMember.PasswordSalt);
            if (sha256Password != qMember.Password)
            {
                ls.Add("fail");
                return Json(ls);
            }
            return Json(qMember);
        }

    }
}
