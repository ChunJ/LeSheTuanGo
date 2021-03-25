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

namespace LeSheTuanGo.Controllers
{
    public class GarbageServiceOffersController : Controller
    {
        private readonly MidtermContext _context;

        public GarbageServiceOffersController(MidtermContext context)
        {
            _context = context;
        }
        int memberID = 3;
        // GET: GarbageServiceOffers
        public async Task<IActionResult> Index()
        {
            var midtermContext = await _context.GarbageServiceOffers.Where(g=>g.HostMemberId==memberID).Include(g => g.District).Include(g => g.GoRange).Include(g => g.HostMember).Include(g => g.ServiceType).ToListAsync();
            List<GarbageServiceOffersViewModel> ls = new List<GarbageServiceOffersViewModel>();
            foreach (var i in midtermContext)
            {
                GarbageServiceOffersViewModel g = new GarbageServiceOffersViewModel(i);
                ls.Add(g);
            }
            return View(ls);
        }

        // GET: GarbageServiceOffers/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var garbageServiceOffer = await _context.GarbageServiceOffers
        //        .Include(g => g.District)
        //        .Include(g => g.GoRange)
        //        .Include(g => g.HostMember)
        //        .Include(g => g.ServiceType)
        //        .FirstOrDefaultAsync(m => m.GarbageServiceId == id);
        //    if (garbageServiceOffer == null)
        //    {
        //        return NotFound();
        //    }
        //    GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(garbageServiceOffer);
          
        //    return View(gsovm);
            
        //}

        // GET: GarbageServiceOffers/Create
        public IActionResult Create()
        {
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName");
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeters");
            //ViewData["HostMemberId"] = new SelectList(_context.Members, "MemberId", "MemberID");
            //ViewData["HostMemberId"] = memberID;
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName");
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName");
            return View();
        }

        // POST: GarbageServiceOffers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Bind("GarbageServiceId,ServiceTypeId,HostMemberId,DistrictId,Address,StartTime,EndTime,IsActive,Latitude,Longitude,CanGo,GoRangeId,L3maxCount,L5maxCount,L14maxCount,L25maxCount,L33maxCount,L75maxCount,L120maxCount")] GarbageServiceOffer garbageServiceOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GarbageServiceOffersViewModel g)
        {
            if (g.Address!=null)
            {
                g.StartTime = DateTime.Now;
                g.HostMemberId = memberID;
                _context.Add(g.gso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName", g.DistrictId);
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeaters", g.GoRangeId);
            //ViewData["HostMemberId"] = 1;
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName", g.District);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", g.ServiceTypeId);
            //GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(g);
            return View(g);
        }
        public string getDistrict(string cityId)
        {
            var cityref = _context.DistrictRefs.Where(n => n.CityId == int.Parse(cityId)).Select(n=>new { n.DistrictId,n.DistrictName}).ToList();
            var s = JsonConvert.SerializeObject(cityref);
            return s;
        }
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
            //ViewData["HostMemberId"] = memberID;
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", garbageServiceOffer.ServiceTypeId);

            GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(garbageServiceOffer);
            return View(gsovm);
        }

        // POST: GarbageServiceOffers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            //ViewData["HostMemberId"] = new SelectList(_context.Members, "MemberId", "Address", garbageServiceOffer.HostMemberId);
            //ViewData["HostMemberId"] = memberID;
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
            //var garbageServiceOffer = await _context.GarbageServiceOffers
            //    .Include(g => g.District)
            //    .Include(g => g.GoRange)
            //    .Include(g => g.HostMember)
            //    .Include(g => g.ServiceType)
            //    .FirstOrDefaultAsync(m => m.GarbageServiceId == id);
            if (garbageServiceOffer == null)
            {
                return NotFound();
            }
            //GarbageServiceOffersViewModel gsovm = new GarbageServiceOffersViewModel(garbageServiceOffer);
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
