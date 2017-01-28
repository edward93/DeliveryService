using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Entities;
using Microsoft.AspNet.SignalR;

namespace DeliveryService.Hubs
{
    public class AddRiderHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public void NotifyBusiness(Order order, Driver nearDriver)
        {
            // TODO: implement this method
            throw new NotImplementedException();
        }
    }
}