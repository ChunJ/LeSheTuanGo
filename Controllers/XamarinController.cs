using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult Searchdb()
        {
            var q = db.Members.Select(n => n);
            return Json(q.ToList());
        }
    }
}
