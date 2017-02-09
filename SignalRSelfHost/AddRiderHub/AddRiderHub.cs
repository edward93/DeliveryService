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
    // TODO: implement authorizatoin later
    public class AddRiderHub : Hub
    {
        private static readonly ConnectionMapping<int> Connections = new ConnectionMapping<int>();
        private readonly Lazy<IOrderService> _orderService;
        public AddRiderHub(IOrderService orderService)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        
        public override Task OnConnected()
        {
            Console.WriteLine($"{nameof(OnConnected)} has started.");

            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    Connections.Add(driverId, Context.ConnectionId);
                    Console.WriteLine($"Driver {driverId} connected.");


                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    Connections.Add(businessId, Context.ConnectionId);
                    Console.WriteLine($"Business {businessId} connected.");
                }
                else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    Connections.Add(businessId, Context.ConnectionId);
                    Console.WriteLine($"Business {businessId} connected.");
                }
            }

            Console.WriteLine($"{nameof(OnConnected)} finished.");

            return base.OnConnected();
        }


        public override async Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine($"{nameof(OnDisconnected)} has started.");

            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    Connections.Remove(driverId, Context.ConnectionId);
                    Console.WriteLine($"Driver {driverId} disconnected.");
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    Connections.Remove(businessId, Context.ConnectionId);
                    Console.WriteLine($"Business {businessId} disconnected.");
                }
                else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    Connections.Remove(businessId, Context.ConnectionId);
                    Console.WriteLine($"Business {businessId} disconnected.");
                }
            }
            Console.WriteLine($"{nameof(OnDisconnected)} finished.");

            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            Console.WriteLine($"{nameof(OnReconnected)} has started.");

            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    if (!Connections.GetConnections(driverId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(driverId, Context.ConnectionId);
                        Console.WriteLine($"Driver {driverId} reconnected.");
                    }
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    if (!Connections.GetConnections(businessId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(businessId, Context.ConnectionId);
                        Console.WriteLine($"Business {businessId} reconnected.");
                    }
                }
                else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    if (!Connections.GetConnections(businessId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(businessId, Context.ConnectionId);
                        Console.WriteLine($"Business {businessId} reconnected.");
                    }
                }
            }

            Console.WriteLine($"{nameof(OnReconnected)} finished.");

            await base.OnReconnected();
        }

        public ServiceResult NotifyBusiness(DriverDetails dirverDetails)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var connections = Connections.GetConnections(-dirverDetails.BusinessId);
                foreach (var connectionId in connections)
                {
                    if (connectionId == null) throw new Exception($"No client with {dirverDetails.BusinessId} business id was found.");

                    Clients.Client(connectionId).NotifyBusinessAboutDriver(dirverDetails);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Business was successfully notified");
                    Console.WriteLine(serviceResult.DisplayMessage());
                }
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while notifying business about driver.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                Console.WriteLine(serviceResult.DisplayMessage());
            }


            Console.WriteLine($"{nameof(NotifyBusiness)} finished.");
            return serviceResult;
        }

        public ServiceResult NotifyDriverAboutOrder(OrderDetails orderDetails, int driverId)
        {
            Console.WriteLine($"{nameof(NotifyDriverAboutOrder)} has started.");
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
                    Console.WriteLine(serviceResult.DisplayMessage());

                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Warning, "Driver was not found in hub");
                    Console.WriteLine(serviceResult.DisplayMessage());
                }
            }
            catch (Exception e)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Somethig went wrong");
                //serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
                serviceResult.Messages.AddMessage(MessageType.Error, e.ToString());
                Console.WriteLine(serviceResult.DisplayMessage());
            }

            Console.WriteLine($"{nameof(NotifyDriverAboutOrder)} finished.");
            return serviceResult;
        }

    }
}