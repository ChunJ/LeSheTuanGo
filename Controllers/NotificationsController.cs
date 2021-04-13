﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeSheTuanGo.Models;
using Newtonsoft.Json;

namespace LeSheTuanGo.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MidtermContext _context;

        public NotificationsController(MidtermContext context)
        {
            _context = context;
        }
        #region 廢棄
        //// GET: Notifications
        //public async Task<IActionResult> Index()
        //{
        //    var midtermContext = _context.Notifications.Include(n => n.Content).Include(n => n.Member);
        //    return View(await midtermContext.ToListAsync());
        //}

        //// GET: Notifications/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications
        //        .Include(n => n.Content)
        //        .Include(n => n.Member)
        //        .FirstOrDefaultAsync(m => m.NotifyId == id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(notification);
        //}

        //// GET: Notifications/Create
        //public IActionResult Create()
        //{
        //    ViewData["ContentId"] = new SelectList(_context.NotifyContents, "ContentId", "ContentText");
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Address");
        //    return View();
        //}

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        //// GET: Notifications/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications.FindAsync(id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ContentId"] = new SelectList(_context.NotifyContents, "ContentId", "ContentText", notification.ContentId);
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Address", notification.MemberId);
        //    return View(notification);
        //}

        //// POST: Notifications/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("NotifyId,MemberId,ContentId,SentTime,SourceType,SourceId,Checked")] Notification notification)
        //{
        //    if (id != notification.NotifyId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(notification);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NotificationExists(notification.NotifyId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ContentId"] = new SelectList(_context.NotifyContents, "ContentId", "ContentText", notification.ContentId);
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Address", notification.MemberId);
        //    return View(notification);
        //}

        //// GET: Notifications/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications
        //        .Include(n => n.Content)
        //        .Include(n => n.Member)
        //        .FirstOrDefaultAsync(m => m.NotifyId == id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(notification);
        //}

        //// POST: Notifications/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var notification = await _context.Notifications.FindAsync(id);
        //    _context.Notifications.Remove(notification);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion

        //[HttpPost]
        //public async Task<string> CreateNotification([Bind("NotifyId,MemberId,ContentId,SentTime,SourceType,SourceId,Checked")] Notification notification)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(notification);
        //        await _context.SaveChangesAsync();
        //        return "";
        //    }
        //    return "";
        //}

        
        public async Task<string> CreateNotification(string groupType,string orderId,string senderId,string notifyContent)
        {
            int contentID = 0;
            DateTime d = DateTime.Now;
            List<int> memberid = new List<int>();
            if (notifyContent != "")
            {
                NotifyContent n = new NotifyContent();
                n.ContentText = notifyContent;
                _context.Add(n);
                await _context.SaveChangesAsync();
                contentID = n.ContentId;
            }
            if (contentID != 0)
            {
                switch (int.Parse(groupType))
                {
                    case 1:
                        var orderhost = await _context.Orders.Where(n => n.OrderId == int.Parse(orderId)).Select(n => n.HostMemberId).ToListAsync();
                        var orderjoin = await _context.OrderBuyRecords.Where(n => n.OrderId == int.Parse(orderId)).Select(n => n.MemberId).ToListAsync();
                        memberid = orderhost.Union(orderjoin).ToList();
                        break;
                    case 2:
                        var garbhost = await _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == int.Parse(orderId)).Select(n => n.HostMemberId).ToListAsync();
                        var garbjoin = await _context.GarbageServiceUseRecords.Where(n => n.GarbageServiceOfferId == int.Parse(orderId)).Select(n => n.MemberId).ToListAsync();
                        memberid = garbhost.Union(garbjoin).ToList();
                        break;
                    case 3:
                        break;
                }
                memberid.Remove(int.Parse(senderId));
                foreach(int id in memberid)
                {
                    Notification notification = new Notification
                    {
                        MemberId = id,
                        ContentId = contentID,
                        SourceType = byte.Parse(groupType),
                        SourceId = int.Parse(orderId),
                        SentTime = d,
                        Checked = false
                    };
                    _context.Add(notification);
                }
                await _context.SaveChangesAsync();
            }

            return JsonConvert.SerializeObject(memberid);
        }
        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.NotifyId == id);
        }
    }
}
