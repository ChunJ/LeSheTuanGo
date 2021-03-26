using Microsoft.AspNetCore.Mvc;
using LeSheTuanGo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace LeSheTuanGo.Controllers
{
    public class MemberController : Controller
    {
        private readonly MidtermContext db;
        public MemberController(MidtermContext context)
        {
            db = context;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(MemberViewModel memberData)
        {
            if(string.IsNullOrEmpty(memberData.Address)|| memberData.DistrictId == 0|| string.IsNullOrEmpty(memberData.Email)|| string.IsNullOrEmpty(memberData.Password))
            {
                return RedirectToAction("Create");
            }
            //密碼加密
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[10];
            rng.GetBytes(buff);
            string salt = Convert.ToBase64String(buff);
            memberData.PasswordSalt = salt;
            memberData.Password = sha256(memberData.Password , salt);
            //密碼加密完成
            db.Members.Add(memberData.member);
            db.SaveChanges();
            return RedirectToAction("Login");
        }
        private string sha256(string input ,string salt)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(input +salt);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            return result;//輸出結果
        }

        public string checkRepeatEmail(string createEmail)
        {

            var Email = from n in db.Members
                        where n.Email == createEmail
                        select n;
            var returnEmail = Email.FirstOrDefault();
            //var returnEmail = db.Members.Where(m => m.Email == createEmail).FirstOrDefault();
            string flag = "";
            if (returnEmail == null)
            {
                flag = "1";
                flag = JsonConvert.SerializeObject(flag);
                return flag;
            }
            else
            {
                flag = "0";
                flag = JsonConvert.SerializeObject(flag);
                return flag;
            }
        }

    }
}
