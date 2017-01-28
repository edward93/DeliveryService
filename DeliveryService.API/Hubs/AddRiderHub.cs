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
        private readonly static ConnectionMapping<int> _connections =
         new ConnectionMapping<int>();
        public void Hello()
        {
            var user = Context.User;
            var userId = user.Identity.GetUserId();
            Clients.All.hello();
        }

        public void Send(string who, string message)
        {
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            string name = ticket.Identity.GetUserName();
            CustomMessage v = new CustomMessage();
            v.Message = message;
            v.UserName = name;

            var driverHub = new DriverHubModel
            {
                Name = ticket.Identity.GetUserName(),
                DriverId = int.Parse(Context.Headers["DriverId"])
            };

            foreach (var connectionId in _connections.GetConnections(driverHub.DriverId))
            {
                Clients.Client(connectionId).addNewMessageToPage(v);
            }
        }

        public class CustomMessage
        {
            public string UserName;
            public string Message;
        }

        
        

        public override Task OnConnected()
        {
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            if (ticket != null)
            {
                var driverHub = new DriverHubModel
                {
                    Name = ticket.Identity.GetUserName(),
                    DriverId = int.Parse(Context.Headers["DriverId"])
                };

                _connections.Add(driverHub.DriverId, Context.ConnectionId);
            }
            //   AssignToSecurityGroup();
            return base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            string name = ticket.Identity.GetUserName();
            var driverHub = new DriverHubModel
            {
                Name = ticket.Identity.GetUserName(),
                DriverId = int.Parse(Context.Headers["DriverId"])
            };

            _connections.Remove(driverHub.DriverId, Context.ConnectionId);

            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            
            var driverHub = new DriverHubModel
            {
                Name = ticket.Identity.GetUserName(),
                DriverId = int.Parse(Context.Headers["DriverId"])
            };

            if (!_connections.GetConnections(driverHub.DriverId).Contains(Context.ConnectionId))
            {
                _connections.Add(driverHub.DriverId, Context.ConnectionId);
            }
            await base.OnReconnected();
        }
    }
}