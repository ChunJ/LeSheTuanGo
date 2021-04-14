using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;


namespace LeSheTuanGo.Controllers
{
    public class BackgroundController : Controller
    {
        private readonly MidtermContext _context;
        private IWebHostEnvironment iv_host;
        public BackgroundController(MidtermContext context, IWebHostEnvironment iv)
        {
            _context = context;
            iv_host = iv;

        }
        public IActionResult Index()
        {
            ViewData["Category"] = new SelectList(_context.CategoryRefs, "CategoryId", "CategoryName");
            return View();
        }
        public IActionResult BIPanel1() {
            return View();
        }
        #region 新增商品項目
        public string InsertCategory(string s)
        {
            var 商品類型重複=_context.CategoryRefs.Where(n => n.CategoryName == s).ToList();

            if (商品類型重複.Count != 0)
            {
                return "商品類型重複";
            }
            else
            {
                CategoryRef category = new CategoryRef();
                category.CategoryName = s;
                _context.CategoryRefs.Add(category);
                _context.SaveChanges();
                var 商品選項 = _context.CategoryRefs.Select(n => new { n.CategoryId, n.CategoryName }).ToList();
                var 下拉是選單 = JsonConvert.SerializeObject(商品選項);
                return 下拉是選單;
            }
        }
        #endregion
        #region 新增商品
        public JsonResult InsertProduct(IFormCollection data, IFormFile photo)
        {
            Byte caId = (byte)int.Parse(data["id"]);
            string imgPath = null;
            if (data["id"]!=""&data["name"] != "" & data["des"] != "" & photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                using (var photopath = new FileStream(
                    iv_host.WebRootPath + @"\images\ProductImages\" + photoName,
                    FileMode.Create))
                {
                    photo.CopyTo(photopath);
                }
                imgPath = "/images/ProductImages/" + photoName;
                Product product = new Product();
                product.CategoryId = caId;
                product.ProductName = data["name"];
                product.ProductDescription = data["des"];
                product.ProductImagePath = imgPath;
                _context.Add(product);
                _context.SaveChanges();
                return Json(new { result = true, msg = "上傳成功" });
            }
            else
            {
                return Json(new { result = false, msg = "上傳失敗" });
            }
        }
        #endregion
    }
}
