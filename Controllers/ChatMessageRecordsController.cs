﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeSheTuanGo.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace LeSheTuanGo.Controllers
{
    public class ChatMessageRecordsController : Controller
    {
        private readonly MidtermContext _context;

        public ChatMessageRecordsController(MidtermContext context)
        {
            _context = context;
        }
        int memberID = 0;
        // GET: ChatMessageRecords
        public async Task<IActionResult> Index()
        {
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            //var midtermContext = _context.ChatMessageRecords.Include(c => c.SentMember);
            var orderQuery = _context.Orders.Where(n => n.HostMemberId == memberID).Select(n=>n.OrderId) ;
            var orderBuyRQuery =  _context.OrderBuyRecords.Where(n => n.MemberId == memberID).Select(n => n.OrderId);
            List<int> ls1 = await orderQuery.ToListAsync();
            ViewBag.order =(await orderQuery.ToListAsync()).Union(await orderBuyRQuery.ToListAsync());
            ViewBag.memberid = memberID;
            //ViewBag.orderBuy = await orderBuyRQuery.ToListAsync();
            //return View(await midtermContext.ToListAsync());
            return View();
        }

        public string chatGetOrderDetail(int orderId)
        {
            var order = _context.Orders.Where(n => n.OrderId == orderId);
            //var orderBuy = _context.OrderBuyRecords.Where(n => n.OrderId == orderId);
            //return order.ToList().Count < 1 ? JsonConvert.SerializeObject(order.ToList()) : JsonConvert.SerializeObject(orderBuy.ToList());
            var i = order.ToList();
            var s = JsonConvert.SerializeObject(order.ToList());
            return JsonConvert.SerializeObject(order.ToList());
        }

        public string chatGetChat(int orderId)
        {
            var chatMessages = _context.ChatMessageRecords.Where(n=>n.GroupId==orderId).ToList();
            return JsonConvert.SerializeObject(chatMessages);
        }
        // GET: ChatMessageRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessageRecord = await _context.ChatMessageRecords
                .Include(c => c.SentMember)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (chatMessageRecord == null)
            {
                return NotFound();
            }

            return View(chatMessageRecord);
        }

        //// GET: ChatMessageRecords/Create
        //public IActionResult Create()
        //{
        //    ViewData["SentMemberId"] = new SelectList(_context.Members, "MemberId", "Address");
        //    return View();
        //}

        // POST: ChatMessageRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public void Create(string message,int orderid,int memberid)
        {
            ChatMessageRecord chatMessageRecord = new ChatMessageRecord {
                GroupType = 1,
                GroupId = orderid,
                SentTime = DateTime.Now,
                SentMemberId=memberid,
                Message=message
            };
            _context.Add(chatMessageRecord);
            _context.SaveChanges();
        }

        // GET: ChatMessageRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessageRecord = await _context.ChatMessageRecords.FindAsync(id);
            if (chatMessageRecord == null)
            {
                return NotFound();
            }
            ViewData["SentMemberId"] = new SelectList(_context.Members, "MemberId", "Address", chatMessageRecord.SentMemberId);
            return View(chatMessageRecord);
        }

        // POST: ChatMessageRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,GroupType,GroupId,SentTime,SentMemberId,Message")] ChatMessageRecord chatMessageRecord)
        {
            if (id != chatMessageRecord.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatMessageRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatMessageRecordExists(chatMessageRecord.MessageId))
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
            ViewData["SentMemberId"] = new SelectList(_context.Members, "MemberId", "Address", chatMessageRecord.SentMemberId);
            return View(chatMessageRecord);
        }

        // GET: ChatMessageRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessageRecord = await _context.ChatMessageRecords
                .Include(c => c.SentMember)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (chatMessageRecord == null)
            {
                return NotFound();
            }

            return View(chatMessageRecord);
        }

        // POST: ChatMessageRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chatMessageRecord = await _context.ChatMessageRecords.FindAsync(id);
            _context.ChatMessageRecords.Remove(chatMessageRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatMessageRecordExists(int id)
        {
            return _context.ChatMessageRecords.Any(e => e.MessageId == id);
        }
    }
}
