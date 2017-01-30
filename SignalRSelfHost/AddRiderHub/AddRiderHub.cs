using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Constants;
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
        private Timer timer;
        public AddRiderHub(IOrderService orderService)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        // TODO: implement authorizatoin later
        //[Authorize(Roles = Roles.Business)]
        public void Send(string param1, string param2)
        {
            Console.WriteLine("Sending something. Debugging is not working.");
        }

        [Authorize(Roles = Roles.Business)]
        public ServiceResult NotifyDriverAboutOrder(OrderDetails orderDetails, int driverId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                orderDetails.OrderStatus = OrderStatus.Pending;

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
            //var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            //if (ticket != null)
            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);
                    var driverHub = new DriverHubModel
                    {
                        Name = "",
                        DriverId = driverId,
                    };

                    Connections.Add(driverHub.DriverId, Context.ConnectionId);
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    var businessHub = new BusinessHubModel
                    {
                        Name = "",
                        BusinessId = businessId
                    };

                    // TODO: Sarkis make this more generic. Saving only ids is incorrect!
                    Connections.Add(businessHub.BusinessId, Context.ConnectionId);
                }
            }


            //timer = new System.Threading.Timer(async e => NotifyDriverAboutOrder(await GetOrder(), driverHub.DriverId),
            //   null,
            //   TimeSpan.Zero,
            //   TimeSpan.FromSeconds(50));


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
            //var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);
            //var driverHub = new DriverHubModel
            //{
            //    Name = "",
            //    DriverId = int.Parse(Context.Headers["DriverId"])
            //};

            //Connections.Remove(driverHub.DriverId, Context.ConnectionId);

            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);
                    var driverHub = new DriverHubModel
                    {
                        Name = "",
                        DriverId = driverId,
                    };

                    Connections.Remove(driverHub.DriverId, Context.ConnectionId);
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    var businessHub = new BusinessHubModel
                    {
                        Name = "",
                        BusinessId = businessId
                    };

                    // TODO: Sarkis make this more generic. Saving only ids is incorrect!
                    Connections.Remove(businessHub.BusinessId, Context.ConnectionId);
                }
            }

            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            //var ticket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(Context.Headers["Authorization"]);

            //var driverHub = new DriverHubModel
            //{
            //    Name = "",
            //    DriverId = int.Parse(Context.Headers["DriverId"])
            //};

            //if (!Connections.GetConnections(driverHub.DriverId).Contains(Context.ConnectionId))
            //{
            //    Connections.Add(driverHub.DriverId, Context.ConnectionId);
            //}

            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);
                    var driverHub = new DriverHubModel
                    {
                        Name = "",
                        DriverId = driverId,
                    };

                    if (!Connections.GetConnections(driverHub.DriverId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(driverHub.DriverId, Context.ConnectionId);
                    }
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    var businessHub = new BusinessHubModel
                    {
                        Name = "",
                        BusinessId = businessId
                    };

                    // TODO: Sarkis make this more generic. Saving only ids is incorrect!
                    if (!Connections.GetConnections(businessHub.BusinessId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(businessHub.BusinessId, Context.ConnectionId);
                    }
                }
            }


            await base.OnReconnected();
        }

        public async Task<ServiceResult> NotifyBusiness(Order order, Driver driver)
        {
            var serviceResult = new ServiceResult();

            // TODO: Sarkis Implement this method correctly
            throw new NotImplementedException();

            return serviceResult;
        }
    }
}