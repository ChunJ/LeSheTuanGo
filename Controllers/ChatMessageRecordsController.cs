using System;
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

        //聊天室主頁
        public async Task<IActionResult> Index(int grouptype = 0, int groupid = 0)
        {
            if (HttpContext.Session.GetInt32(cUtility.Current_User_Id) == null)
            {
                return RedirectToAction("Login", "Member");
            }

            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            var memberName = await _context.Members.Where(n => n.MemberId == memberID).Select(n => n.FirstName + n.LastName).ToListAsync();
            var orderQuery = _context.Orders.Where(n => n.HostMemberId == memberID).Select(n => n.OrderId);
            var orderBuyRQuery = _context.OrderBuyRecords.Where(n => n.MemberId == memberID).Select(n => n.OrderId);
            ViewBag.order = (await orderQuery.ToListAsync()).Union(await orderBuyRQuery.ToListAsync());
            ViewBag.username = memberName[0];
            ViewBag.memberid = memberID;
            ViewBag.grouptype = grouptype;
            ViewBag.groupid = groupid;
            return View();
        }

        public string getOrderList(int groupType, int selfother, int onoff)
        {
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            string s = "";
            bool of = onoff == 1 ? true : false;
            switch (groupType)
            {
                case 1: //團購團
                    var i = _context.Orders.Where(n => n.HostMemberId == memberID);
                    var j = _context.OrderBuyRecords.Where(n => n.MemberId == memberID);
                    //案件狀態是否有效
                    if (onoff != 0)
                    {
                        i = i.Where(n => n.IsActive == of);
                        j = j.Where(n => n.Order.IsActive == of);
                    }
                    //自己團or別人團
                    if (selfother == 1)
                    {
                        s = JsonConvert.SerializeObject(i.Select(n => new { n.OrderId, n.Product.ProductName,n.HostMemberId }).ToList());
                    }
                    else if (selfother == 2)
                    {
                        s = JsonConvert.SerializeObject(j.Select(n => new { n.Order.OrderId, n.Order.Product.ProductName,n.Order.HostMemberId }).ToList());
                    }
                    else
                    {
                        var ls1 = (i.Select(n => new { n.OrderId, n.Product.ProductName,n.HostMemberId }).ToList()).Union((j.Select(n => new { n.Order.OrderId, n.Order.Product.ProductName,n.Order.HostMemberId }).ToList()));
                        s = JsonConvert.SerializeObject(ls1);
                    }
                    break;

                case 2: //垃圾團
                    var q1 = _context.GarbageServiceOffers.Where(n => n.HostMemberId == memberID);
                    var q2 = _context.GarbageServiceUseRecords.Where(n => n.MemberId == memberID);
                    //案件狀態是否有效
                    if (onoff != 0)
                    {
                        q1 = q1.Where(n => n.IsActive == of);
                        q2 = q2.Where(n => n.GarbageServiceOffer.IsActive == of);
                    }
                    //自己團or別人團
                    if (selfother == 1)
                    {
                        s = JsonConvert.SerializeObject(q1.Select(n => new { n.GarbageServiceId, EndTime = n.EndTime.ToShortDateString(),n.HostMemberId }).ToList());
                    }
                    else if (selfother == 2)
                    {
                        s = JsonConvert.SerializeObject(q2.Select(n => new { n.GarbageServiceOffer.GarbageServiceId, EndTime = n.GarbageServiceOffer.EndTime.ToShortDateString(),n.GarbageServiceOffer.HostMemberId }).ToList());
                    }
                    else
                    {
                        var ls1 = (q1.Select(n => new { n.GarbageServiceId, EndTime = n.EndTime.ToShortDateString(),n.HostMemberId }).ToList()).Union((q2.Select(n => new { n.GarbageServiceOffer.GarbageServiceId, EndTime = n.GarbageServiceOffer.EndTime.ToShortDateString(),n.GarbageServiceOffer.HostMemberId }).ToList()));
                        s = JsonConvert.SerializeObject(ls1);
                    }
                    break;
            }
            return s;
        }

        //取得團明細
        public string chatGetOrderDetail(int orderId, int grouptype,int hostid)
        {
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            if (grouptype == 1)
            {
                var order = _context.Orders.Where(n => n.OrderId == orderId).Select(n => new
                {
                    n.OrderId,
                    n.Address,
                    n.Product.ProductName,
                    n.District.City.CityName,
                    n.District.DistrictName,
                    n.CanGo,
                    n.GoRange.RangeInMeters,
                });
                var i = order.ToList();
                return JsonConvert.SerializeObject(order.ToList());
            }
            else
            {
                var garbage = _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == orderId);
                var i = garbage.ToList();
                return JsonConvert.SerializeObject(garbage.ToList());
            }

        }

        //取得聊天紀錄
        public string chatGetChat(int orderId, int grouptype)
        {
            var chatMessages = _context.ChatMessageRecords.Where(n => n.GroupId == orderId && n.GroupType == grouptype).Select(n => new
            {
                n.Message,
                n.SentMemberId,
                SentTime = n.SentTime.ToString("yyyy/MM/dd, HH:mm"),
                username = n.SentMember.FirstName + n.SentMember.LastName
            }).ToList();
            return JsonConvert.SerializeObject(chatMessages);
        }

        //寫入聊天紀錄
        [HttpPost]
        public void Create(string message, int orderid, int memberid, byte grouptype)
        {
            ChatMessageRecord chatMessageRecord = new ChatMessageRecord
            {
                GroupType = grouptype,
                GroupId = orderid,
                SentTime = DateTime.Now,
                SentMemberId = memberid,
                Message = message
            };
            _context.Add(chatMessageRecord);
            _context.SaveChanges();

        }

        #region 無用僅佔存的code

        // GET: ChatMessageRecords/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var chatMessageRecord = await _context.ChatMessageRecords
        //        .Include(c => c.SentMember)
        //        .FirstOrDefaultAsync(m => m.MessageId == id);
        //    if (chatMessageRecord == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(chatMessageRecord);
        //}
        // GET: ChatMessageRecords/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var chatMessageRecord = await _context.ChatMessageRecords.FindAsync(id);
        //    if (chatMessageRecord == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["SentMemberId"] = new SelectList(_context.Members, "MemberId", "Address", chatMessageRecord.SentMemberId);
        //    return View(chatMessageRecord);
        //}

        // POST: ChatMessageRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("MessageId,GroupType,GroupId,SentTime,SentMemberId,Message")] ChatMessageRecord chatMessageRecord)
        //{
        //    if (id != chatMessageRecord.MessageId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(chatMessageRecord);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ChatMessageRecordExists(chatMessageRecord.MessageId))
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
        //    ViewData["SentMemberId"] = new SelectList(_context.Members, "MemberId", "Address", chatMessageRecord.SentMemberId);
        //    return View(chatMessageRecord);
        //}

        //// GET: ChatMessageRecords/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var chatMessageRecord = await _context.ChatMessageRecords
        //        .Include(c => c.SentMember)
        //        .FirstOrDefaultAsync(m => m.MessageId == id);
        //    if (chatMessageRecord == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(chatMessageRecord);
        //}

        //// POST: ChatMessageRecords/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var chatMessageRecord = await _context.ChatMessageRecords.FindAsync(id);
        //    _context.ChatMessageRecords.Remove(chatMessageRecord);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion
        private bool ChatMessageRecordExists(int id)
        {
            return _context.ChatMessageRecords.Any(e => e.MessageId == id);
        }
    }
}
