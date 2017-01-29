using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Models.ViewModels;
using Infrastructure.Helpers;
using Microsoft.AspNet.SignalR;
using ServiceLayer.Service;
using SignalRSelfHost.App_Start;

namespace SignalRSelfHost.AddRiderHub
{
    public class AddRiderHub : Hub
    {
        private static readonly ConnectionMapping<int> Connections = new ConnectionMapping<int>();
        private readonly Lazy<IOrderService> _orderService;
        private System.Threading.Timer timer;
        public AddRiderHub(IOrderService orderService)
        {
            Console.WriteLine("Initializing object.");
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        public void Send(string param1, string param2)
        {
            Console.WriteLine("Sending something. Debugging is not working.");
        }
        public ServiceResult NotifyDriverAboutOrder(OrderDetails orderDetails, int driverId)
        {
            orderDetails.OrderStatus = OrderStatus.AcceptedByDriver;
            var serviceResult = new ServiceResult();
            try
            {
                var connectionId = Connections.GetConnections(driverId).FirstOrDefault();
                if (connectionId != null)
                {
                    Clients.Client(connectionId).AppendOrderToDriver(orderDetails);

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
                    Name = "",
                    DriverId = int.Parse(Context.Headers["DriverId"])
                };

                Connections.Add(driverHub.DriverId, Context.ConnectionId);


                timer = new System.Threading.Timer(async e => NotifyDriverAboutOrder(await GetOrder(), driverHub.DriverId),
                   null,
                   TimeSpan.Zero,
                   TimeSpan.FromSeconds(50));


            }
            return base.OnConnected();
        }

        private async Task<OrderDetails> GetOrder()
        {
            var result = (await _orderService.Value.GetAllEntitiesAsync<Order>()).
                Select(o => new OrderDetails(o)).FirstOrDefault();
            return result;
        }



        public override async Task OnDisconnected(bool stopCalled)
        {
            var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            var driverHub = new DriverHubModel
            {
                Name = "",
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
                Name = "",
                DriverId = int.Parse(Context.Headers["DriverId"])
            };

            if (!Connections.GetConnections(driverHub.DriverId).Contains(Context.ConnectionId))
            {
                Connections.Add(driverHub.DriverId, Context.ConnectionId);
            }
            await base.OnReconnected();
        }

        public void NotifyBusiness(Order order, Driver driver)
        {
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<AddRiderHub>();
            Clients.All.notifyBusiness("hello", "myFriend");
        }
    }
}