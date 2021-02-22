using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data.Hubs
{
    public class SignalServer : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("displayNotification", "");
        }
    }
}
