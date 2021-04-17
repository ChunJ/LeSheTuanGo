using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly MidtermContext db;
        public ShoppingController(MidtermContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult change(string MerchantID, string MerchantTradeNo, string MerchantTradeDate, string PaymentType, string TotalAmount, string TradeDesc, string ItemName, string ReturnURL, string ChoosePayment, string StoreID, string ClientBackURL, string CreditInstallment, string InstallmentAmount, string Redeem, string EncryptType, string CheckMacValue)
        {
            List<PayModel> pays = new List<PayModel>();
            PayModel pay = new PayModel();
            pay.ItemName = ItemName;
            pay.TotalAmount = TotalAmount;           
            pay.MerchantTradeNo = "DX" + DateTime.Now.ToString("yyyyMMddhhmmss") + "bc73";
            pay.MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            //pay.TradeDesc = "";
            pay.SuccessUrl = $"https://{HttpContext.Session.GetString(cUtility.Current_Ip)}:81/Member/Charge";
            pay.ReturnURL = $"https://{HttpContext.Session.GetString(cUtility.Current_Ip)}:81/Member/InsertRecord";
            //pay.SuccessUrl = "https://localhost:5001/Member/Charge";
            //pay.ReturnURL = "https://localhost:5001/Member/InsertRecord";
            pay.HashKey = "5294y06JbISpM5x9";
            pay.HashIV = "v77hoKGq4kWxNNIS";
            var input = $"HashKey=5294y06JbISpM5x9&ChoosePayment=Credit&ClientBackURL={pay.ReturnURL}&CreditInstallment=&EncryptType=1&InstallmentAmount=&ItemName={pay.ItemName}&MerchantID=2000132&MerchantTradeDate={pay.MerchantTradeDate}&MerchantTradeNo={pay.MerchantTradeNo}&PaymentType=aio&Redeem=&ReturnURL={pay.SuccessUrl}&StoreID=&TotalAmount={pay.TotalAmount}&TradeDesc=建立信用卡測試訂單&HashIV=v77hoKGq4kWxNNIS";
            Console.WriteLine(input);
            string encoded = System.Web.HttpUtility.UrlEncode(input).ToLower();
            byte[] messageBytes = Encoding.Default.GetBytes(encoded);
            SHA256 sHA256 = new SHA256CryptoServiceProvider();
            byte[] vs = sHA256.ComputeHash(messageBytes);
            string result = BitConverter.ToString(vs).ToUpper();
            result = result.Replace("-", "");
            pay.CheckMacValue = result;
            HttpContext.Session.SetString(cUtility.ItemName, pay.ItemName);
            HttpContext.Session.SetString(cUtility.MerchantTradeNo, pay.MerchantTradeNo);
            HttpContext.Session.SetString(cUtility.MerchantTradeDate, pay.MerchantTradeDate);
            HttpContext.Session.SetString(cUtility.CheckMacValue, pay.CheckMacValue);
            HttpContext.Session.SetInt32(cUtility.balance, int.Parse(pay.TotalAmount));
            Console.WriteLine(result);
            pays.Add(pay);
            return Json(new { JsonResult = pays });
        }
    }
}
