using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers
{
    public class BackgroundController : Controller
    {
        private readonly MidtermContext _context;
        public BackgroundController(MidtermContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            ViewData["Category"] = new SelectList(_context.CategoryRefs, "CategoryId", "CategoryName");
            return View();
        }
        public ActionResult InsertCategory(string s)
        {
            var 商品類型重複=_context.CategoryRefs.Where(n => n.CategoryName == s).ToList();

            if (商品類型重複.Count != 0)
            {
                return Content("商品類型重複");
            }
            else
            {
                CategoryRef category = new CategoryRef();
                category.CategoryName = s;
                _context.CategoryRefs.Add(category);
                _context.SaveChanges();
                return Content("儲存成功");
            }

        }
    }
}
