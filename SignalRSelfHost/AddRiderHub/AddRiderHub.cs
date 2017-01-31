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
        //private Timer timer;
        public AddRiderHub(IOrderService orderService)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

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
            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    Connections.Add(driverId, Context.ConnectionId);

                    //timer = new System.Threading.Timer(async e => NotifyDriverAboutOrder(await GetOrder(), driverHub.DriverId),
                    //null,
                    //TimeSpan.Zero,
                    //TimeSpan.FromSeconds(50));
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    Connections.Add(businessId, Context.ConnectionId);
                } else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    Connections.Add(businessId, Context.ConnectionId);
                }
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
            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    Connections.Remove(driverId, Context.ConnectionId);
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    Connections.Remove(businessId, Context.ConnectionId);
                }
                else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    Connections.Remove(businessId, Context.ConnectionId);
                }
            }

            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            if (Context.Headers != null)
            {

                if (Context.Headers["DriverId"] != null)
                {
                    int driverId;

                    int.TryParse(Context.Headers["DriverId"], out driverId);

                    if (!Connections.GetConnections(driverId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(driverId, Context.ConnectionId);
                    }
                }
                else if (Context.Headers["BusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.Headers["BusinessId"], out businessId);

                    if (!Connections.GetConnections(businessId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(businessId, Context.ConnectionId);
                    }
                }
                else if (Context.QueryString["ClientBusinessId"] != null)
                {
                    int businessId;

                    int.TryParse(Context.QueryString["ClientBusinessId"], out businessId);

                    if (!Connections.GetConnections(businessId).Contains(Context.ConnectionId))
                    {
                        Connections.Add(businessId, Context.ConnectionId);
                    }
                }
            }


            await base.OnReconnected();
        }

        public ServiceResult NotifyBusiness(DriverDetails dirverDetails)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var connectionId = Connections.GetConnections(-dirverDetails.BusinessId).FirstOrDefault();

                if(connectionId == null) throw new Exception($"No client with {dirverDetails.BusinessId} business id was found.");

                Clients.Client(connectionId).NotifyBusinessAboutDriver(dirverDetails);

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "Business was successfully notified");
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while notifying business about driver.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
            }

            return serviceResult;
        }
    }
}