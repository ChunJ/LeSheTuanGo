using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string memberid, string message,string orderid)
        {
            await Clients.All.SendAsync("ReceiveMessage", memberid, message,orderid);
        }

    }
}
