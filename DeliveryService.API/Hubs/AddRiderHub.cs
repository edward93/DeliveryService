using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DAL.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DeliveryService.API.Hubs
{
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
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            var userId = ticket.Identity.GetUserId();

            CustomMessage v = new CustomMessage();
            v.Message = message;
            v.UserName = name;
            Clients.All.addNewMessageToPage(v);
            Clients.User(userId).addNewMessageToPage(v);
        }

        private void AssignToSecurityGroup()
        {
            if (Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]) != null)
                Groups.Add(
                    Context.ConnectionId, "authenticated");
            else
                Groups.Add(Context.ConnectionId, "anonymous");
        }

        public void GetOrder()
        {

        }

        public class CustomMessage
        {
            public string UserName;
            public string Message;
        }

        public override Task OnConnected()
        {
            //   AssignToSecurityGroup();
            return base.OnConnected();
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