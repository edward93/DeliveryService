using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DeliveryService.API.Hubs
{
    public class AddRiderHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            CustomMessage v = new CustomMessage();
            v.Message = message;
            v.UserName = name;
            Clients.All.addNewMessageToPage(v);
        }
        public class CustomMessage
        {
            public string UserName;
            public string Message;
        }

    }
}