using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.Hubs
{
    public class ChatHub : Hub
    {
        //public async Task SendMessage(string memberid, string message,string orderid)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", memberid, message,orderid);
        //}

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public async Task SendMessageToGroup(string groupName,string memberid, string username, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName,memberid, username, message);
        }
        public async Task SendNotificationToGroup(string groupName, string memberid, string username, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", groupName, memberid, username, message);
        }
    }
}
