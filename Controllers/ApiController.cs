using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System.Text;

namespace LeSheTuanGo.Controllers {
    public class ApiController : Controller {
        private readonly MidtermContext db;
        public ApiController(MidtermContext context) {
            db = context;
        }
        public string getDistrictByCity(int cityIn) {
            var districtyList = db.DistrictRefs.Where(d => d.CityId == cityIn).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var dis in districtyList) {
                sb.Append(dis.DistrictId + ",");
                sb.Append(dis.DistrictName + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString(); ;
        }
    }
}
