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
    public class ServiceOfferController : Controller
    {
        private readonly MidtermContext _context;
        int memberID = 0;
        public ServiceOfferController(MidtermContext context)
        {
            _context = context;
        }
        //總覽
        // GET: GarbageServiceOffers
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member", new { from = "ServiceOffer/Index" });
            int userId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //get user's district
            var user = _context.Members.Where(m => m.MemberId == userId).Include(m => m.District).First();
            ViewData["Address"] = user.Address;
            ViewData["DistrictId"] = user.DistrictId;
            ViewData["CityId"] = user.District.CityId;

            ViewData["District"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName");
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeters");
            ViewData["City"] = new SelectList(_context.CityRefs, "CityId", "CityName");
            //ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName");
            return View();
        }

        // POST: GarbageServiceOffers/Create
        [HttpPost]
        public async Task<IActionResult> Index(GarbageServiceOffersViewModel g)
        {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null) return RedirectToAction("Login", "Member");
            g.HostMemberId = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            if (g.Address != null)
            {
                g.StartTime = DateTime.Now;
                g.ServiceTypeId = 1;
                //get latlong from address
                DistrictRef dist = _context.DistrictRefs.Where(d => d.DistrictId == g.DistrictId)
                .Include(d => d.City).First();
                string address = dist.City.CityName + dist.DistrictName + g.Address;
                var latlong = cUtility.addressToLatlong(address);
                g.Latitude = latlong[0];
                g.Longitude = latlong[1];
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
                return RedirectToAction("Index", "ChatMessageRecords", new { grouptype = 2, groupid = g.gso.GarbageServiceId });
            }
            ViewData["DistrictId"] = new SelectList(_context.DistrictRefs, "DistrictId", "DistrictName", g.DistrictId);
            ViewData["GoRangeId"] = new SelectList(_context.RangeRefs, "RangeId", "RangeInMeaters", g.GoRangeId);
            ViewData["CityId"] = new SelectList(_context.CityRefs, "CityId", "CityName", g.District);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypeRefs, "ServiceTypeId", "ServiceName", g.ServiceTypeId);
            return View(g);
        }

        #region 縣市區連動用，目前不使用
        public string getDistrict(int cityId)
        {
            var cityref = _context.DistrictRefs.Where(n => n.CityId == cityId).Select(n => new { n.DistrictId, n.DistrictName }).ToList();
            var s = JsonConvert.SerializeObject(cityref);
            return s;
        }
        #endregion

        
        //編輯開團內容
        public void EditGarbageOffer(int garbageServiceID, GarbageServiceOffer garbageServiceOffer)
        {
            var g = _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == garbageServiceID).Select(n => new
            {
                n.L3available,
                n.L3maxCount,
                n.L5available,
                n.L5maxCount,
                n.L14available,
                n.L14maxCount,
                n.L25available,
                n.L25maxCount,
                n.L33available,
                n.L33maxCount,
                n.L75available,
                n.L75maxCount,
                n.L120available,
                n.L120maxCount,
            }).ToList();

            if (garbageServiceID == garbageServiceOffer.GarbageServiceId)
            {
                decimal[] s = cUtility.addressToLatlong(garbageServiceOffer.Address);
                garbageServiceOffer.Latitude = s[0];
                garbageServiceOffer.Longitude = s[1];
                garbageServiceOffer.L3available = (byte)(g[0].L3available + (garbageServiceOffer.L3maxCount - g[0].L3maxCount));
                garbageServiceOffer.L5available = (byte)(g[0].L5available + (garbageServiceOffer.L5maxCount - g[0].L5maxCount));
                garbageServiceOffer.L14available = (byte)(g[0].L14available + (garbageServiceOffer.L14maxCount - g[0].L14maxCount));
                garbageServiceOffer.L25available = (byte)(g[0].L25available + (garbageServiceOffer.L25maxCount - g[0].L25maxCount));
                garbageServiceOffer.L33available = (byte)(g[0].L33available + (garbageServiceOffer.L33maxCount - g[0].L33maxCount));
                garbageServiceOffer.L75available = (byte)(g[0].L75available + (garbageServiceOffer.L75maxCount - g[0].L75maxCount));
                garbageServiceOffer.L120available = (byte)(g[0].L120available + (garbageServiceOffer.L120maxCount - g[0].L120maxCount));
                _context.Update(garbageServiceOffer);
                _context.SaveChanges();
            }
        }

        #region 無用程式碼
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
        #endregion
        private bool GarbageServiceOfferExists(int id)
        {
            return _context.GarbageServiceOffers.Any(e => e.GarbageServiceId == id);
        }
    }
}
