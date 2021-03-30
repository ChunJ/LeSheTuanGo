
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeSheTuanGo.Models;
using System;
using System.Collections.Generic;
//using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Http;
using LeSheTuanGo.ViewModels;
using GeoCoordinatePortable;

namespace LeSheTuanGo.Controllers
{

    public class BuyController : Controller
    {
        private readonly MidtermContext _context;


        int MemberID = 2;
       readonly GeoCoordinate Geo = null;
        public BuyController(MidtermContext context)
        {
            var q = from n in (new MidtermContext()).Members
                    where n.MemberId == MemberID
                    select n;
            double userLatitude = (double)q.ToList()[0].Latitude;
            double userLongitude = (double)q.ToList()[0].Longitude;
            Geo = new GeoCoordinate(userLatitude, userLongitude);
            _context = context;
            
        }


        public IActionResult Index()
        {
            var q = _context.Orders.Include(g => g.GoRange).Include(g=>g.Product).Where(g=>g.AvailableCount !=0).ToList();
            var q2 = from n in (new MidtermContext()).Products
                     select n;
            
            ViewData["GoRange"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeters");
            ViewData["Category"] = new SelectList(_context.CategoryRefs, "CategoryId", "CategoryName");
            ViewBag.ProductName = q2.ToList();
            List<JoinGroupViewModel> list = new List<JoinGroupViewModel>();
            foreach (var item in q)
            {
            list.Add(new JoinGroupViewModel(item));
            }

            return View(list);
        }
        [HttpPost]
        public IActionResult Index(int distance)
        {
            var q = from n in (new MidtermContext()).Orders
                    where Geo.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude))<distance 
                  select n;

            return View(q);
        }
        public string Refresh(string re)
        {
            var q = from n in (new MidtermContext()).RangeRefs
                    where n.RangeId.ToString()==re
                    select n;
           var 距離= q.ToList().First().RangeInMeters;
            var q2 = from n in (new MidtermContext()).Orders.AsEnumerable()
                     where Geo.GetDistanceTo(new GeoCoordinate((double)n.Latitude, (double)n.Longitude))/1000 <= (double)距離
                     select n;
            var A = q2.ToList();
            string ls = JsonConvert.SerializeObject(A);

            return ls;
        }
 
        public string InsertProductName(string Pid)
        {
            var q = from n in (new MidtermContext()).Products
                    where n.ProductId ==int.Parse(Pid)
                    select n.ProductName;
            var A = q.ToList();
            string ls = JsonConvert.SerializeObject(A);

            return ls;
        }
        public IActionResult JoinGroup(int id)
        {
           
            var order = (new MidtermContext()).Orders
                .Include(g => g.GoRange)
                .Where(m => m.OrderId == id);
            List<JoinGroupViewModel> list = new List<JoinGroupViewModel>();
            list.Add(new JoinGroupViewModel(order.FirstOrDefault()));

            //var orderHistory = _context.OrderBuyRecords
            //    .Include(g => g.Member)
            //    .Include(g => g.Order)
            //    .Where(m => m.OrderId == id);

            var countMax = from n in _context.Orders
                           where n.OrderId==id
                           select n;
            var member = _context.Members.Include(n => n.District).Where(n => n.MemberId == MemberID);
            ViewData["City"] = new SelectList(_context.CityRefs, "CityId", "CityName");
            ViewBag.DistrictId = member.FirstOrDefault().DistrictId;
            ViewBag.CityId = member.FirstOrDefault().District.CityId;
            ViewBag.total = countMax.FirstOrDefault().AvailableCount;
            return View(list);
        }
        [HttpPost]
        public IActionResult JoinGroup(int id, int count,bool NeedCome,int ComeDistrictID,string ComeAddress)
        {
            //todo 地址更改
            //MidtermContext mid = new MidtermContext();
            var q = from n in _context.Orders
                    where n.OrderId == id
                    select n;
            var newOrder = q.ToList()[0];
            if (newOrder.AvailableCount - (byte)count > 0)
            {
                newOrder.AvailableCount = (byte)(newOrder.AvailableCount - (byte)count);  
                OrderBuyRecord ob = new OrderBuyRecord();
                ob.OrderId = id;
                ob.Count = (byte)count;
                ob.MemberId = MemberID;
                ob.NeedCome = NeedCome;
                ob.ComeDistrictId = (short)ComeDistrictID;
                ob.ComeAddress = ComeAddress;
                _context.Add(ob);
                _context.SaveChanges();
            }

            else
            {
                var order =_context.Orders
                .Include(g => g.GoRange)
                .Where(m => m.OrderId == id);
                List<JoinGroupViewModel> list = new List<JoinGroupViewModel>();

                list.Add(new JoinGroupViewModel(order.FirstOrDefault()));

                return View(list);
            }
            return RedirectToAction("Index");
            //todo 修改max總數
        }
        public IActionResult HistoryList()
        {
               var q = from n in (new MidtermContext()).OrderBuyRecords
                    where n.MemberId == MemberID
                    select n;
            return View(q);
        }
        public IActionResult Edit(int id)
        {
            var q = from n in (new MidtermContext()).OrderBuyRecords
                    where n.OrderBuyRecordId == id
                    select n;

            return View(q);
        }
    }
}
