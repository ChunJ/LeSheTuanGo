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
                return RedirectToAction("Login", "Member", new { from = "ChatMessageRecords/Index" });
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
                        s = JsonConvert.SerializeObject(i.Select(n => new { n.OrderId, n.Product.ProductName, n.HostMemberId }).ToList());
                    }
                    else if (selfother == 2)
                    {
                        s = JsonConvert.SerializeObject(j.Select(n => new { n.Order.OrderId, n.Order.Product.ProductName, n.Order.HostMemberId }).ToList());
                    }
                    else
                    {
                        var ls1 = (i.Select(n => new { n.OrderId, n.Product.ProductName, n.HostMemberId }).ToList()).Union((j.Select(n => new { n.Order.OrderId, n.Order.Product.ProductName, n.Order.HostMemberId }).ToList()));
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
                        s = JsonConvert.SerializeObject(q1.Select(n => new { n.GarbageServiceId, EndTime = n.EndTime.ToShortDateString(), n.HostMemberId }).ToList());
                    }
                    else if (selfother == 2)
                    {
                        s = JsonConvert.SerializeObject(q2.Select(n => new { n.GarbageServiceOffer.GarbageServiceId, EndTime = n.GarbageServiceOffer.EndTime.ToShortDateString(), n.GarbageServiceOffer.HostMemberId }).ToList());
                    }
                    else
                    {
                        var ls1 = (q1.Select(n => new { n.GarbageServiceId, EndTime = n.EndTime.ToShortDateString(), n.HostMemberId }).ToList()).Union((q2.Select(n => new { n.GarbageServiceOffer.GarbageServiceId, EndTime = n.GarbageServiceOffer.EndTime.ToShortDateString(), n.GarbageServiceOffer.HostMemberId }).ToList()));
                        s = JsonConvert.SerializeObject(ls1);
                    }
                    break;
            }
            return s;
        }

        //取得團明細
        public string chatGetOrderDetail(int orderId, int grouptype, int hostid)
        {
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            if (grouptype == 1)
            {
                //var ls = new List<dynamic>();
                if (hostid == memberID)
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
                        n.OrderDescription,
                        n.MaxCount,
                        n.AvailableCount,
                        self = true,
                    });
                    var i = order.ToList();
                    return JsonConvert.SerializeObject(order.ToList());

                }
                else
                {
                    var order = _context.OrderBuyRecords.Where(n => n.OrderId == orderId && n.MemberId == memberID).Select(n => new
                    {
                        n.OrderId,
                        n.Order.Address,
                        n.Order.Product.ProductName,
                        n.Order.District.City.CityName,
                        n.Order.District.DistrictName,
                        n.Order.CanGo,
                        n.Order.GoRange.RangeInMeters,
                        n.Order.OrderDescription,
                        n.Order.MaxCount,
                        n.Order.AvailableCount,
                        n.Count,
                        self = false,
                    });
                    var i = order.ToList();
                    return JsonConvert.SerializeObject(order.ToList());
                }
            }
            else
            {
                if (hostid == memberID)
                {
                    var garbage = _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == orderId).Select(n => new
                    {
                        n.GarbageServiceId,
                        n.Address,
                        n.District.City.CityName,
                        n.District.DistrictName,
                        n.CanGo,
                        n.GoRange.RangeInMeters,
                        n.L3maxCount,
                        n.L3available,
                        n.L5maxCount,
                        n.L5available,
                        n.L14maxCount,
                        n.L14available,
                        n.L25maxCount,
                        n.L25available,
                        n.L33maxCount,
                        n.L33available,
                        n.L75maxCount,
                        n.L75available,
                        n.L120maxCount,
                        n.L120available,
                        self = true,
                    });
                    var i = garbage.ToList();
                    return JsonConvert.SerializeObject(garbage.ToList());

                }
                else
                {
                    var garbage = _context.GarbageServiceUseRecords.Where(n => n.GarbageServiceOffer.GarbageServiceId == orderId && n.MemberId == memberID).Select(n => new
                    {
                        n.GarbageServiceOffer.GarbageServiceId,
                        n.GarbageServiceOffer.Address,
                        n.GarbageServiceOffer.District.City.CityName,
                        n.GarbageServiceOffer.District.DistrictName,
                        n.GarbageServiceOffer.CanGo,
                        n.GarbageServiceOffer.GoRange.RangeInMeters,
                        n.GarbageServiceOffer.L3maxCount,
                        n.GarbageServiceOffer.L3available,
                        n.GarbageServiceOffer.L5maxCount,
                        n.GarbageServiceOffer.L5available,
                        n.GarbageServiceOffer.L14maxCount,
                        n.GarbageServiceOffer.L14available,
                        n.GarbageServiceOffer.L25maxCount,
                        n.GarbageServiceOffer.L25available,
                        n.GarbageServiceOffer.L33maxCount,
                        n.GarbageServiceOffer.L33available,
                        n.GarbageServiceOffer.L75maxCount,
                        n.GarbageServiceOffer.L75available,
                        n.GarbageServiceOffer.L120maxCount,
                        n.GarbageServiceOffer.L120available,
                        n.L3count,
                        n.L5count,
                        n.L14count,
                        n.L25count,
                        n.L33count,
                        n.L75count,
                        n.L120count,
                        n.NeedCome,
                        ComeDistrictName = n.ComeDistrict.DistrictName,
                        ComeCityName = n.ComeDistrict.City.CityName,
                        n.ComeAddress,
                        self = false,
                    });
                    var i = garbage.ToList();
                    return JsonConvert.SerializeObject(garbage.ToList());
                }

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
                username = n.SentMember.FirstName + n.SentMember.LastName,
                n.SentMember.ProfileImagePath,
            }).ToList();
            return JsonConvert.SerializeObject(chatMessages);
        }

        public string getJointMember(int orderId, int grouptype)
        {
            if (grouptype == 1)
            {
                var memberList = _context.OrderBuyRecords.Where(n => n.OrderId == orderId).Select(n => new
                {
                    n.MemberId,
                    n.Member.ProfileImagePath,
                    username = n.Member.FirstName + n.Member.LastName,
                }).Distinct();
                var dancyou = _context.Orders.Where(n => n.OrderId == orderId).Select(n => new
                {
                    MemberId = n.HostMemberId,
                    ProfileImagePath = n.HostMember.ProfileImagePath,
                    username = n.HostMember.FirstName + n.HostMember.LastName,
                });
                return JsonConvert.SerializeObject(memberList.Union(dancyou));
            }
            else
            {
                var memberList = _context.GarbageServiceUseRecords.Where(n => n.GarbageServiceOfferId == orderId).Select(n => new
                {
                    n.MemberId,
                    n.Member.ProfileImagePath,
                    username = n.Member.FirstName + n.Member.LastName,
                }).Distinct();
                var dancyou = _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == orderId).Select(n => new
                {
                    MemberId = n.HostMemberId,
                    ProfileImagePath = n.HostMember.ProfileImagePath,
                    username = n.HostMember.FirstName + n.HostMember.LastName,
                });
                return JsonConvert.SerializeObject(memberList.Union(dancyou));
            }
        }

        //寫入聊天紀錄
        [HttpPost]
        public async Task<string> Create(string message, int orderid, int memberid, byte grouptype)
        {
            var memberphoto = _context.Members.Where(n => n.MemberId == memberid).Select(n => n.ProfileImagePath).FirstOrDefault();
            ChatMessageRecord chatMessageRecord = new ChatMessageRecord
            {
                GroupType = grouptype,
                GroupId = orderid,
                SentTime = DateTime.Now,
                SentMemberId = memberid,
                Message = message
            };
            _context.Add(chatMessageRecord);
            await _context.SaveChangesAsync();
            return JsonConvert.SerializeObject(new Tuple<DateTime, string>(chatMessageRecord.SentTime, memberphoto));
        }

        //編輯明細頁面
        public async Task<string[]> editOrder(int grouptype, int orderid, int memberid, bool self)
        {
            string[] ls = new string[3];
            memberID = HttpContext.Session.GetInt32(cUtility.Current_User_Id).Value;
            ls[1] = JsonConvert.SerializeObject(await _context.RangeRefs.Select(n => n).ToListAsync());
            ls[2] = JsonConvert.SerializeObject(await _context.CityRefs.Select(n => n).ToListAsync());
            if (grouptype == 1)
            {
                if (self)
                {
                    var order = await _context.Orders.Where(n => n.OrderId == orderid && n.HostMemberId == memberid).ToListAsync();
                    var ss = JsonConvert.SerializeObject(order);
                    ls[0] = ss;
                    return ls;
                }
                else
                {
                    var orderBuy = await _context.OrderBuyRecords.Where(n => n.OrderId == orderid && n.MemberId == memberid).ToListAsync();
                    var ss = JsonConvert.SerializeObject(orderBuy);
                    ls[0] = ss;
                    return ls;
                }

            }
            else
            {
                if (self)
                {
                    var garbageServiceOffer = await _context.GarbageServiceOffers.Where(n => n.GarbageServiceId == orderid && n.HostMemberId == memberid).Select(n => new
                    {
                        n.GarbageServiceId,
                        n.Address,
                        n.CanGo,
                        n.District.CityId,
                        n.DistrictId,
                        n.EndTime,
                        n.GoRangeId,
                        n.HostMemberId,
                        n.IsActive,
                        n.L3maxCount,
                        n.L5maxCount,
                        n.L14maxCount,
                        n.L25maxCount,
                        n.L33maxCount,
                        n.L75maxCount,
                        n.L120maxCount,
                        n.StartTime,
                        n.ServiceTypeId,
                    }).ToListAsync();
                    var ss = JsonConvert.SerializeObject(garbageServiceOffer);
                    ls[0] = ss;
                    return ls;
                }
                else
                {
                    var garbageServiceOffer = await _context.GarbageServiceUseRecords.Where(n => n.GarbageServiceOfferId == orderid && n.MemberId == memberid).Select(n => new
                    {
                        n.GarbageServiceOffer.GarbageServiceId,
                        n.ComeAddress,
                        n.NeedCome,
                        n.ComeDistrict.CityId,
                        n.ComeDistrictId,
                        n.L3count,
                        n.L5count,
                        n.L14count,
                        n.L25count,
                        n.L33count,
                        n.L75count,
                        n.L120count,
                    }).ToListAsync();
                    var ss = JsonConvert.SerializeObject(garbageServiceOffer);
                    ls[0] = ss;
                    return ls;
                }
                //return ls;
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Bind("GarbageServiceId,ServiceTypeId,HostMemberId,DistrictId,Address,StartTime,EndTime,IsActive,Latitude,Longitude,CanGo,GoRangeId,L3maxCount,L5maxCount,L14maxCount,L25maxCount,L33maxCount,L75maxCount,L120maxCount")]


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
