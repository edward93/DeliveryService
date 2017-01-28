using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DAL.Constants;
using DeliveryService.API.ViewModel.Models;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using DAL.Enums;

namespace DeliveryService.API.Hubs
{


    public class AddRiderHub : Hub
    {
        private static readonly ConnectionMapping<int> Connections = new ConnectionMapping<int>();
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

            foreach (var connectionId in Connections.GetConnections(driverHub.DriverId))
            {
                Clients.Client(connectionId).addNewMessageToPage(v);
            }
        }

        public class CustomMessage
        {
            public string UserName;
            public string Message;
        }

        public ServiceResult AppendOrderToDriver(OrderDetails orderDetails, int driverId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var connectionId = Connections.GetConnections(driverId).FirstOrDefault();
                if (connectionId != null)
                {
                    Clients.Client(connectionId).AppendOrderToDriver(orderDetails, driverId);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Order was sucessfully sent to driver");
                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Warning, "Driver was not found in hub");
                }
            }
            catch (Exception e)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Somethig went wrong");
                serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
                serviceResult.Messages.AddMessage(MessageType.Error, e.ToString());
            }
            return serviceResult;
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

                Connections.Add(driverHub.DriverId, Context.ConnectionId);
            }
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

            Connections.Remove(driverHub.DriverId, Context.ConnectionId);

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

            if (!Connections.GetConnections(driverHub.DriverId).Contains(Context.ConnectionId))
            {
                Connections.Add(driverHub.DriverId, Context.ConnectionId);
            }
            await base.OnReconnected();
        }
    }
}