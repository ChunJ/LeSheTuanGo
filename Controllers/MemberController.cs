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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LeSheTuanGo.Controllers
{
    public class MemberController : Controller
    {
        private readonly MidtermContext db;
        private IWebHostEnvironment iv_host;

        public MemberController(IWebHostEnvironment host,MidtermContext context)
        {
            iv_host = host;
            db = context;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Member member)
        {
            string check = checkLogin(member.Email, member.Password);
            check = JsonConvert.DeserializeObject(check).ToString();
            if (check == "not User" && check == "incorrect")
            {
                return RedirectToAction("Login");
            }
            HttpContext.Session.SetInt32(cUtility.Current_User_Id, Convert.ToInt32(check));

            return RedirectToAction("Index","Home");
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
            #region 密碼加密
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[10];
            rng.GetBytes(buff);
            string salt = Convert.ToBase64String(buff);
            memberData.PasswordSalt = salt;
            memberData.Password = sha256(memberData.Password , salt);
            #endregion
            #region 照片
            if(memberData.image != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                using (var photo = new FileStream(
                    iv_host.WebRootPath + @"\profileImages\" + photoName,
                    FileMode.Create))
                {
                    memberData.image.CopyTo(photo);
                }
                memberData.ProfileImagePath = "/profileImages/" + photoName;
            }
            else
            {
                string imageDefalt = "profilePic.jpg";
                memberData.ProfileImagePath = "/profileImages/" + imageDefalt;
            }                
            #endregion

            db.Members.Add(memberData.member);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        private string sha256(string inputPwd ,string salt)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(inputPwd +salt);//將字串轉為Byte[]
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
                flag = "No";
                flag = JsonConvert.SerializeObject(flag);
                return flag;
            }
            else
            {
                flag = "Yes";
                flag = JsonConvert.SerializeObject(flag);
                return flag;
            }
        }

        public string checkLogin(string inputEmail , string inputPassword)
        {
            string returnMessage = "";
            var qEmail = db.Members.Where(n => n.Email == inputEmail).FirstOrDefault();
            if (qEmail == null)
            {
                returnMessage = "not User";
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
            string salt = qEmail.PasswordSalt;
            string saltedPwd = sha256(inputPassword, salt);
            if (saltedPwd == qEmail.Password)
            {
                returnMessage = qEmail.MemberId.ToString();
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
            else
            {
                returnMessage = "incorrect";
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
        }
    }
}
