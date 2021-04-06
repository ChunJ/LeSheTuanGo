using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Controllers
{
    public class BackgroudController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
