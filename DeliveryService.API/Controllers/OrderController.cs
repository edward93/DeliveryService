using System;
using System.Threading.Tasks;
using System.Web.Http;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using Infrastructure.Config;
using Infrastructure.Helpers;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IBusinessService> _businessService;
        public OrderController(IConfig config, IDbContext context,
            IOrderService orderService,
            IDriverService driverService,
            IBusinessService businessService) : base(config, context)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        public async Task<IHttpActionResult> AcceptOrder(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null) throw new Exception($"No driver with id: {driverId} was found.");

                    if (!driver.Approved)
                        throw new Exception("This drver is not approved by administration and is not alowed to proceed.");

                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order.VehicleType != driver.VehicleType)
                        throw new Exception("The requested vehicle type doesn't match with the driver's vehicle.");

                    //var business = await _businessService.Value.GetByIdAsync<Business>(order.BusinessId);

                    await _orderService.Value.AcceptOrderAsync(order, driver);

                    await _driverService.Value.ChangeDriverStatusAsync(driverId, DriverStatus.Busy);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info,
                        $"The order (Id: {orderId}) was accepted by driver (Id: {driverId})");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while accepting an order");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
                }
            }
            
            return Json(serviceResult);
        }

        public async Task<IHttpActionResult> RejectOrder(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> OnTheWayToPickUp(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> ArrivedAtThePickUpLocation(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> OrderPickedUp(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> Delivered(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> NotDelivered(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> BookReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> CancelReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> ReturnConfirmed(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }
    }
}