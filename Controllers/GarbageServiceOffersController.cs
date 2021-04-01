using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using LeSheTuanGo.Models;
using LeSheTuanGo.ViewModels;
using Microsoft.AspNetCore.Http;

namespace LeSheTuanGo.Controllers
{
    public class GarbageServiceOffersController : Controller
    {
        private readonly MidtermContext _context;

        public GarbageServiceOffersController(MidtermContext context)
        {
            _context = context;
        }

        int memberID = 0;

        //總覽
        // GET: GarbageServiceOffers
        public async Task<IActionResult> Index()
        {
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var midtermContext = await _context.GarbageServiceOffers.Where(g=>g.HostMemberId==memberID).Include(g => g.District).Include(g => g.GoRange).Include(g => g.HostMember).Include(g => g.ServiceType).ToListAsync();
            List<GarbageServiceOffersViewModel> ls = new List<GarbageServiceOffersViewModel>();
            foreach (var i in midtermContext)
            {
                GarbageServiceOffersViewModel g = new GarbageServiceOffersViewModel(i);
                ls.Add(g);
                //A
            }
            return View(ls);
        }

        // GET: GarbageServiceOffers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member");
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName");
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeters");
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName");
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName");
            return View();
        }

        // POST: GarbageServiceOffers/Create
        [HttpPost]
        public async Task<IActionResult> Create(GarbageServiceOffersViewModel g)
        {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member");
            g.HostMemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            if (g.Address!=null)
            {
                g.StartTime = DateTime.Now;
                //get latlong from address
                DistrictRef dist = _context.DistrictRefs.Where(d => d.DistrictId == g.DistrictId)
                .Include(d => d.City).First();
                string address = dist.City.CityName + dist.DistrictName + g.Address;
                var latlong = cUtility.addressToLatlong(address);
                g.Latitude = latlong[0];
                g.Longitude = latlong[1];
                //A
                g.IsActive = true;
                g.gso.L3available = g.L3maxCount;
                g.gso.L5available = g.L5maxCount;
                g.gso.L14available = g.L14maxCount;
                g.gso.L25available = g.L25maxCount;
                g.gso.L33available = g.L33maxCount;
                g.gso.L75available = g.L75maxCount;
                g.gso.L120available = g.L120maxCount;
                _context.Add(g.gso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName", g.DistrictId);
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeaters", g.GoRangeId);
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName", g.District);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", g.ServiceTypeId);
            return View(g);
        }

        #region 縣市區連動用，目前不使用
        public string getDistrict(string cityId)
        {
            var cityref = _context.DistrictRefs.Where(n => n.CityId == int.Parse(cityId)).Select(n=>new { n.DistrictId,n.DistrictName}).ToList();
            var s = JsonConvert.SerializeObject(cityref);
            return s;
        }
        #endregion
        // GET: GarbageServiceOffers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garbageServiceOffer = await _context.GarbageServiceOffers.FindAsync(id);
            if (garbageServiceOffer == null)
            {
                return NotFound();
            }
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName", garbageServiceOffer.DistrictId);
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeters", garbageServiceOffer.GoRangeId);
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName", garbageServiceOffer.District);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", garbageServiceOffer.ServiceTypeId);
            GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(garbageServiceOffer);
            return View(gsovm);
        }

        // POST: GarbageServiceOffers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GarbageServiceId,ServiceTypeId,HostMemberId,DistrictId,Address,StartTime,EndTime,IsActive,Latitude,Longitude,CanGo,GoRangeId,L3maxCount,L5maxCount,L14maxCount,L25maxCount,L33maxCount,L75maxCount,L120maxCount")] GarbageServiceOffer garbageServiceOffer)
        {
            if (id != garbageServiceOffer.GarbageServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garbageServiceOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarbageServiceOfferExists(garbageServiceOffer.GarbageServiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName", garbageServiceOffer.DistrictId);
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeId", garbageServiceOffer.GoRangeId);
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName", garbageServiceOffer.District);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", garbageServiceOffer.ServiceTypeId);
            GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(garbageServiceOffer);
            return View(gsovm);
        }

        // GET: GarbageServiceOffers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var garbageServiceOffer = await _context.GarbageServiceOffers.FindAsync(id);
            _context.GarbageServiceOffers.Remove(garbageServiceOffer);
            await _context.SaveChangesAsync();
            if (garbageServiceOffer == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: GarbageServiceOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garbageServiceOffer = await _context.GarbageServiceOffers.FindAsync(id);
            _context.GarbageServiceOffers.Remove(garbageServiceOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GarbageServiceOfferExists(int id)
        {
            return _context.GarbageServiceOffers.Any(e => e.GarbageServiceId == id);
        }
    }
}
