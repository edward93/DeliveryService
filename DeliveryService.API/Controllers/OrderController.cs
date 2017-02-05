using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.API.Hubs;
using DeliveryService.API.ViewModel.Models;
using DeliveryService.Models.ViewModels;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.Ajax.Utilities;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IOrderHistoryService> _orderHistoryService;
        private readonly Lazy<IDriverPenaltyService> _driverPenaltyService;
        private readonly Lazy<IDriverFeeService> _driverFeeService;
        private readonly Lazy<IBusinessPenaltyService> _businessPenaltyService;

        public OrderController(IConfig config, IDbContext context,
            IOrderService orderService,
            IDriverService driverService,
            IBusinessService businessService,
            IOrderHistoryService orderHistoryService,
            IDriverPenaltyService driverPenaltyService, 
            IDriverFeeService driverFeeService, 
            IBusinessPenaltyService businessPenaltyService) : base(config, context)
        {
            _driverFeeService = new Lazy<IDriverFeeService>(() => driverFeeService);
            _businessPenaltyService = new Lazy<IBusinessPenaltyService>(() => businessPenaltyService);
            _driverPenaltyService = new Lazy<IDriverPenaltyService>(() => driverPenaltyService);
            _orderHistoryService = new Lazy<IOrderHistoryService>(() => orderHistoryService);
            _orderService = new Lazy<IOrderService>(() => orderService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> AcceptOrder(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));

                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order.VehicleType != driver.VehicleType)
                        throw new Exception("The requested vehicle type doesn't match with the driver's vehicle.");

                    //var business = await _businessService.Value.GetByIdAsync<Business>(order.BusinessId);

                    await _orderService.Value.AcceptOrderAsync(order, driver);

                    // TODO: Update client app with signalR!
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
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while accepting an order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> RejectOrder(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));

                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception($"No order found with id: {orderId}");

                    if (order.OrderStatus != OrderStatus.DriverAcceptedByBusiness)
                        throw new Exception($"Driver cannot accept order which has {order.OrderStatus} status.");

                    await _orderService.Value.RejectOrderAsync(order, driver);

                    // TODO: Update client app with signalR!
                    await _driverService.Value.ChangeDriverStatusAsync(driverId, DriverStatus.Busy);

                    // Calculate rejection penalty if ther is one
                    var orders = await _orderHistoryService.Value.GetRejectedOrdersByDriverForCurrentDayAsync(driverId);

                    // TODO: test this method
                    if (orders.Count() >= 3)
                    {
                        // Penalize driver for rejecting more then 3 times during last 24 hours.
                        await
                            _driverPenaltyService.Value.PenalizeDriverForRejectingMoreThenThreeTimesAsync(driver, order);
                    }

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info,
                        $"The order (Id: {orderId}) was rejected by driver (Id: {driverId})");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while rejecting an order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        // TODO: Q: Why do we need this action?
        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> OnTheWayToPickUp(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));


                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception($"No order found with id: {orderId}");

                    if (order.OrderStatus != OrderStatus.AcceptedByDriver)
                        throw new Exception(Config.Messages["CannotChangeOrderStatus"]);

                    await _orderService.Value.OnTheWayToPickUpAsync(driver, order);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver is on the way to pick up.");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while changing order status to {OrderStatus.OnTheWayToPickUp}.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> ArrivedAtThePickUpLocation(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));


                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception(string.Format(Config.Messages["OrderIdNotFound"], orderId));

                    if (order.OrderStatus != OrderStatus.OnTheWayToPickUp)
                        throw new Exception(Config.Messages["CannotChangeOrderStatus"]);

                    await _orderService.Value.ArrivedAtPickUpLocationAsync(driver, order);

                    // Calculate driver penalties if there are any
                    var orderRecord =
                        await
                            _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driverId, orderId,
                                ActionType.DriverArrivedAtPickUpLocation);

                    if (orderRecord == null)
                        throw new Exception(
                            $"No record found with driver id:{driverId} orderId: {orderId} and action: {ActionType.DriverArrivedAtPickUpLocation}.");

                    if (orderRecord.ActuallTimeToPickUpLocation != null)
                    {
                        // calculate delay in minutes
                        var driverArrivalDelay = orderRecord.ActuallTimeToPickUpLocation.Value -
                                                 orderRecord.TimeToReachPickUpLocation;

                        if (driverArrivalDelay >= 1)
                        {
                            await
                                _driverPenaltyService.Value.CalculatePenaltyForDelayAsync(driver, order,
                                    driverArrivalDelay);
                        }
                    }

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info,
                        Config.Messages["ArrivedAtPickUpLocatoinSuccess"]);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while changing order status to {OrderStatus.ArrivedAtThePickUpLocation}.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> OrderPickedUp(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));


                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception(string.Format(Config.Messages["OrderIdNotFound"], orderId));

                    if (order.OrderStatus != OrderStatus.ArrivedAtThePickUpLocation)
                        throw new Exception(Config.Messages["CannotChangeOrderStatus"]);

                    await _orderService.Value.OrderPickedUpAsync(driver, order);

                    // Calculate driver fees if there are any
                    var orderRecord =
                        await
                            _orderHistoryService.Value.GetRecordByDriverIdOrderIdAndActionTypeAsync(driverId, orderId,
                                ActionType.DriverArrivedAtPickUpLocation);


                    // Calculate waiting time in minutes
                    var driverWaitingTime = new decimal((orderRecord.UpdatedDt -
                                             DateTime.UtcNow).TotalMinutes);

                    if (driverWaitingTime >= 1)
                    {
                        await
                            _driverFeeService.Value.CalculateFeeForWaitingAsync(driver, order,
                                driverWaitingTime);

                        // Penelize business for making driver to wait
                        await _businessPenaltyService.Value.CalculatePenaltyForDriverWaitingAsync(driver, order, driverWaitingTime);
                    }

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info,
                        Config.Messages["OrderPickedUpSuccess"]);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while changing order status to {OrderStatus.ArrivedAtThePickUpLocation}.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> Delivered(int driverId, int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));


                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception(string.Format(Config.Messages["OrderIdNotFound"], orderId));

                    if (order.OrderStatus != OrderStatus.OrderPickedUp)
                        throw new Exception(Config.Messages["CannotChangeOrderStatus"]);

                    await _orderService.Value.OrderDeliveredAsync(driver, order);

                    // TODO: calculate penalties and fees and create transactions

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info,
                        Config.Messages["DeliveredSuccess"]);

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while changing order status to {OrderStatus.Delivered}.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> NotDelivered(int driverId, int orderId, string reason)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Get driver
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null)
                        throw new Exception(string.Format(Config.Messages["DriverIdNotFound"], driverId));


                    if (!driver.Approved)
                        throw new Exception(Config.Messages["NonApprovedDriver"]);

                    // Get order
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception(string.Format(Config.Messages["OrderIdNotFound"], orderId));

                    if (order.OrderStatus != OrderStatus.OrderPickedUp)
                        throw new Exception(Config.Messages["CannotChangeOrderStatus"]);


                    await _orderService.Value.OrderNotDeliveredAsync(driver, order, reason);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, string.Format(Config.Messages["NotDeliveredSuccess"], reason));

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while changing order status to {OrderStatus.Delivered}.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> BookReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> CancelReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> ReturnConfirmed(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }
    }
}