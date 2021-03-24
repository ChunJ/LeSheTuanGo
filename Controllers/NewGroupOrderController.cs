using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System.Text;

namespace LeSheTuanGo.Controllers {
    public class NewGroupOrderController : Controller {
        private readonly MidtermContext db;
        public NewGroupOrderController(MidtermContext context) {
            db = context;
        }
        public IActionResult Index() {
            ViewData["Category"] = new SelectList(db.CategoryRefs, "CategoryId", "CategoryName");
            ViewData["City"] = new SelectList(db.CityRefs, "CityId", "CityName");
            return View();
        }
        //non view
        
    }
}
