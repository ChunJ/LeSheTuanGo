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
        //non view
        public string getDistrictBy(int cityId) {
            var districtyList = db.DistrictRefs.Where(d => d.CityId == cityId).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var dis in districtyList) {
                sb.Append(dis.DistrictId + ",");
                sb.Append(dis.DistrictName + ",");
            }
            if (sb.Length != 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        //non view
        public string getProductBy(int CategoryId) {
            var productList = db.Products.Where(d => d.CategoryId == CategoryId).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var dis in productList) {
                sb.Append(dis.ProductId + ",");
                sb.Append(dis.ProductName + ",");
            }
            if (sb.Length != 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

    }
}
