using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DAL.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace DeliveryService.API.Hubs
{
    [Authorize(Roles = Roles.Member)]
    public class AddRiderHub : Hub
    {
        public void Hello()
        {
            var user = Context.User;
            var userId = user.Identity.GetUserId();
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            var user = Context.User;
            var userId = user.Identity.GetUserId();

            CustomMessage v = new CustomMessage();
            v.Message = message;
            v.UserName = name;
            Clients.All.addNewMessageToPage(v);
            Clients.User(user.Identity.GetUserId()).addNewMessageToPage(v);
        }

        public void GetOrder()
        {

        }

        public class CustomMessage
        {
            public string UserName;
            public string Message;
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            await base.OnReconnected();
        }
    }
}