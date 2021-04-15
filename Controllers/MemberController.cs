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
using System.Net.Mail;

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

        public IActionResult Login(string from = "Home/Index")
        {
            ViewData["from"] = from;
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email,string Password,string from)
        {
            string check = checkLogin(Email, Password);
            check = JsonConvert.DeserializeObject(check).ToString();
            if (check == "not User" || check == "incorrect")
            {
                return RedirectToAction("Login");
            }
            string[] path = from.Split('/');
            return RedirectToAction(path[1], path[0]);
        }

        public IActionResult Create()
        {
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View();
        }//todo 0407 css排版
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
                    iv_host.WebRootPath + @"\images\ProfileImages\" + photoName,
                    FileMode.Create))
                {
                    memberData.image.CopyTo(photo);
                }
                memberData.ProfileImagePath = "/images/ProfileImages/" + photoName;
            }
            else
            {
                string imageDefalt = "guestProfile.jpg";
                memberData.ProfileImagePath = "/images/ProfileImages/" + imageDefalt;
            }
            #endregion
            #region 地址轉經緯
            decimal[] addressToLatlong = new decimal[2];
            var qDistrict = db.DistrictRefs.Where(n => n.DistrictId == memberData.DistrictId).FirstOrDefault();
            var qCity = db.CityRefs.Where(n => n.CityId == qDistrict.CityId).FirstOrDefault();
            addressToLatlong = cUtility.addressToLatlong(qCity.CityName + qDistrict.DistrictName + memberData.Address);
            memberData.Latitude = addressToLatlong[0];
            memberData.Longitude = addressToLatlong[1];
            #endregion
            db.Members.Add(memberData.member);
            db.SaveChanges();
            sendEmail(memberData.Email , memberData.MemberId ,"openMember");
            return RedirectToAction("Login");
        }

        public IActionResult Detail()
        {
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var qMember = db.Members.Where(n => n.MemberId == userId).FirstOrDefault();
            var qDistrict = db.DistrictRefs.Where(n => n.DistrictId == qMember.DistrictId).FirstOrDefault();
            var qCity = db.CityRefs.Where(n => n.CityId == qDistrict.CityId).FirstOrDefault();
            qMember.Address = qCity.CityName + qDistrict.DistrictName + qMember.Address;
            MemberViewModel vm = new MemberViewModel(qMember);
            if (qMember.Validate)
                vm.Validate = "已驗證";
            else
                vm.Validate = "未驗證";

            return View(vm);
        }

        public IActionResult Charge()
        {
            return View();
        }

        public IActionResult EditPassword(int? memberId)
        {
            if (memberId == null)
                return RedirectToAction("Detail");
            var EditPassword = db.Members.Where(n => n.MemberId == memberId).FirstOrDefault();
            MemberViewModel vm = new MemberViewModel(EditPassword);
            return View(vm);
        }
        [HttpPost]
        public IActionResult EditPassword(MemberViewModel memberEdit)
        {
            var selected = db.Members.Where(n => n.MemberId == memberEdit.MemberId).FirstOrDefault();
            if (selected != null)
            {
                selected.Password = sha256(memberEdit.Password, selected.PasswordSalt);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult EditAddress(int? memberId)
        {
            if (memberId == null)
                return RedirectToAction("Detail");
            var EditPassword = db.Members.Where(n => n.MemberId == memberId).FirstOrDefault();
            MemberViewModel vm = new MemberViewModel(EditPassword);
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View(vm);
        }
        [HttpPost]
        public IActionResult EditAddress(MemberViewModel memberEdit)
        {
            var selected = db.Members.Where(n => n.MemberId == memberEdit.MemberId).FirstOrDefault();
            if (selected != null)
            {
                selected.Address = memberEdit.Address;
                selected.DistrictId = memberEdit.DistrictId;

                decimal[] addressToLatlong = new decimal[2];
                addressToLatlong = cUtility.addressToLatlong(selected.Address);
                selected.Latitude = addressToLatlong[0];
                selected.Longitude = addressToLatlong[1];

                db.SaveChanges();
                return RedirectToAction("Detail");
            }

            return View();
        }

        public IActionResult EditImage(int? memberId)
        {
            if (memberId == null)
                return RedirectToAction("Detail");
            var EditPassword = db.Members.Where(n => n.MemberId == memberId).FirstOrDefault();
            MemberViewModel vm = new MemberViewModel(EditPassword);
            return View(vm);
        }
        [HttpPost]
        public IActionResult EditImage(MemberViewModel memberEdit)
        {
            var selected = db.Members.Where(n => n.MemberId == memberEdit.MemberId).FirstOrDefault();
            if (selected != null)
            {
                #region 照片
                if (memberEdit.image != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    using (var photo = new FileStream(
                        iv_host.WebRootPath + @"\images\ProfileImages\" + photoName,
                        FileMode.Create))
                    {
                        memberEdit.image.CopyTo(photo);
                    }
                    memberEdit.ProfileImagePath = "/images/ProfileImages/" + photoName;
                }
                else
                {
                    return RedirectToAction("Detail");
                }
                #endregion

                selected.ProfileImagePath = memberEdit.ProfileImagePath;
                db.SaveChanges();
                HttpContext.Session.SetInt32(cUtility.Current_User_Id, Convert.ToInt32(selected.MemberId));
                HttpContext.Session.SetString(cUtility.Current_User_Name, selected.FirstName + " " + selected.LastName);
                HttpContext.Session.SetString(cUtility.Current_User_Profile_Image, selected.ProfileImagePath);
            }
            return RedirectToAction("Detail");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult forgetPassword()
        {
             return View(); 
        }
        [HttpPost]
        public IActionResult forgetPassword(string Email)
        {
            var memberData = db.Members.Where(n => n.Email == Email).FirstOrDefault();
            if(memberData != null)
            {
                sendEmail(memberData.Email, memberData.MemberId , "resetPassword");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }//ok 差中間跳轉頁面

        public IActionResult resetPassword(string memberId)
        {
            var member = db.Members.Where(n => n.MemberId.ToString() == memberId).FirstOrDefault();
            return View(member);
        }
        [HttpPost]
        public IActionResult resetPassword(Member member)
        {
            if(member != null)
            {
                var q = db.Members.Where(n => n.MemberId == member.MemberId).FirstOrDefault();
                if (q == null)
                    return RedirectToAction("resetPassword");
                q.Password = sha256(member.Password, q.PasswordSalt);
                db.SaveChanges();
            }
            return RedirectToAction("Login");
        }//ok  差中間跳轉頁面

        public IActionResult openMember(string memberId)
        {
            var q = db.Members.Where(n => n.MemberId.ToString() == memberId).FirstOrDefault();
            if (!q.Validate)
            {
                q.Validate = true;
                db.SaveChanges();
            }
            else
                return RedirectToAction("Index", "Home");
            
            return RedirectToAction("Login", "Member");
        }//ok  差中間跳轉頁面

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
            var selected = db.Members.Where(n => n.Email == inputEmail).FirstOrDefault();
            if (selected == null)
            {
                returnMessage = "not User";
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
            string salt = selected.PasswordSalt;
            string saltedPwd = sha256(inputPassword, salt);
            if (saltedPwd == selected.Password)
            {
                if (selected.Validate)
                {
                    HttpContext.Session.SetInt32(cUtility.Current_User_Id, Convert.ToInt32(selected.MemberId));
                    HttpContext.Session.SetString(cUtility.Current_User_Name, selected.FirstName + " " + selected.LastName);
                    HttpContext.Session.SetString(cUtility.Current_User_Profile_Image, selected.ProfileImagePath);
                    returnMessage = selected.MemberId.ToString();
                    returnMessage = JsonConvert.SerializeObject(returnMessage);
                    return returnMessage;
                }
                else
                {
                    returnMessage = "not Validate";
                    returnMessage = JsonConvert.SerializeObject(returnMessage);
                    return returnMessage;
                }
            }
            else
            {
                returnMessage = "incorrect";
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
        }
        public string samePassword(string inputPassword)
        {
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var q = db.Members.Where(n => n.MemberId == userId).FirstOrDefault();
            string returnMessage = "";
            if (q == null)
            {
                returnMessage = "fail";
                returnMessage = JsonConvert.SerializeObject(returnMessage);
                return returnMessage;
            }
            if (q.Password == sha256(inputPassword, q.PasswordSalt))
                returnMessage = "correct";
            else
                returnMessage = "incorrcet";
            returnMessage = JsonConvert.SerializeObject(returnMessage);
            return returnMessage;
        }
        public string reSendEmail(string Email)
        {
            string message = "";
            var q = db.Members.Where(n => n.Email == Email).FirstOrDefault();
            if (q == null)
            {
                message = "not find";
                message = JsonConvert.SerializeObject(message);
                return message;
            }
            sendEmail(q.Email, q.MemberId, "openMember");
            message = "success";
            message = JsonConvert.SerializeObject(message);
            return message;

        }
        public void sendEmail(string inputEmail , int inputId , string controllerName)
        {
            string bodyEmail = "";
            if (controllerName == "openMember")
                bodyEmail = "http://192.168.36.145:8080/Member/openMember?memberId=" + inputId;
            else if (controllerName == "resetPassword")
                bodyEmail = "http://192.168.36.145:8080/Member/resetPassword?memberId=" + inputId;
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            MySmtp.Credentials = new System.Net.NetworkCredential("msit129GarbageCar@gmail.com", "@msit129GarbageCar@");

            MySmtp.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(inputEmail, "樂圾團GO");
            mail.To.Add(inputEmail);
            mail.Priority = MailPriority.Normal;
            mail.Subject = "樂圾團GO驗證信";
            if(controllerName == "openMember")
                mail.Body = "點選網址，啟動會員 :\r\n" + bodyEmail; //todo 可能可以在裡面加a標籤 看情形
            if(controllerName == "resetPassword")
                mail.Body = "點選網址，重新設定密碼 :\r\n" + bodyEmail; //todo 可能可以在裡面加a標籤 看情形

            MySmtp.Send(mail);
        }
        public IActionResult InsertRecord()
        {
            Payment payment = new Payment();
            payment.MemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            payment.MerchantTradeDate = HttpContext.Session.GetString(cUtility.MerchantTradeDate);
            payment.MerchantTradeNo=HttpContext.Session.GetString(cUtility.MerchantTradeNo);
            payment.TradeDesc = "";
            payment.ItemName = HttpContext.Session.GetString(cUtility.ItemName);
            payment.CheckMacValue = HttpContext.Session.GetString(cUtility.CheckMacValue);
            db.Payments.Add(payment);
            db.SaveChanges();
            return RedirectToAction("Charge");
        }
    }
}
