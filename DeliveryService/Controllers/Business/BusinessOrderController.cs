using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Helpers.DataTableHelper;
using DeliveryService.Helpers.DataTableHelper.Models;
using DeliveryService.Models.ViewModels;
using DeliveryService.ViewModels.Business;
using DeliveryService.ViewModels.Drivers;
using DeliveryService.ViewModels.Orders;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using ServiceLayer.ApplicationService;
using ServiceLayer.Service;
using WebGrease.Css.Extensions;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessOrderController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IOrderHistoryService> _orderHistoryService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IRiderService> _driverService;
        private readonly Lazy<IBusinessPenaltyService> _businessPenaltyService;
        private readonly Lazy<IDriverLocationService> _driverLocationService;
        private readonly Lazy<IDriverApplicationService> _driverApplicationService;
        private DataTable<BusinessOrder> _ordersDataTable;
        private readonly string _signalRConnection;

        public BusinessOrderController(IConfig config,
            IDbContext context,
            IOrderService orderService,
            IPersonService personService,
            IBusinessService businessService,
            IRiderService driverService,
            IDriverLocationService driverLocationService, 
            IBusinessPenaltyService businessPenaltyService, 
            IOrderHistoryService orderHistoryService, 
            IDriverApplicationService driverApplicationService) : base(config, context)
        {
            _driverApplicationService = new Lazy<IDriverApplicationService>(() => driverApplicationService);
            _orderHistoryService = new Lazy<IOrderHistoryService>(() => orderHistoryService);
            _businessPenaltyService = new Lazy<IBusinessPenaltyService>(() => businessPenaltyService);
            _signalRConnection = $"{Config.SignalRServerUrl}:{Config.SignalRServerPort}/";
            _driverLocationService = new Lazy<IDriverLocationService>(() => driverLocationService);
            _driverService = new Lazy<IRiderService>(() => driverService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _orderService = new Lazy<IOrderService>(() => orderService);

        }

        public async Task<ActionResult> BusinessOrders()
        {
            var currBusiness =
                await _businessService.Value.GetBusinessByPersonId(
                    (await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId())).Id);

            ViewBag.BusinessId = currBusiness.Id;
            return View();
        }

        public async Task<ActionResult> RetryOrder(int orderId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);
                    if (order == null) throw new Exception($"No order found with Id:{orderId}");

                    if (order.OrderStatus != OrderStatus.Pending || order.OrderStatus != OrderStatus.RejectedByDriver)
                        throw new Exception(
                            $"You cannot retry orders with other status than {EnumHelpers<OrderStatus>.GetDisplayValue(OrderStatus.Pending)}");

                    var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                    // Get nearest driver
                    var nearestDriver = await _driverApplicationService.Value.GetNearestDriverAsync(order);
                    if (nearestDriver != null)
                    {
                        using (var hubConnection = new HubConnection(_signalRConnection)
                        {
                            TraceLevel = TraceLevels.All,
                            TraceWriter = Console.Out
                        })
                        {
                            var hubProxy = hubConnection.CreateHubProxy("AddRiderHub");
                            hubConnection.Headers.Add("BusinessId", currBusiness.Id.ToString());

                            await hubConnection.Start();
                            var driverDetails = new DriverDetails(order, nearestDriver);
                            var result = await hubProxy.Invoke<ServiceResult>("NotifyBusiness", driverDetails);

                            if (!result.Success)
                                throw new Exception($"Error while notifying business about nearest driver.");
                        }
                    }
                    else
                    {
                        serviceResult.Success = false;
                        serviceResult.Messages.AddMessage(MessageType.Warning, "No driver was found for your order. Please try again a bit later.");
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while finding nearest driver.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }
        [HttpPost]
        public async Task<ActionResult> AddNewOrder(MakeOrderViewModel model)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid) throw new Exception(ModelState.GetModelStateErrorsAsString());
                    var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                    var pickUpLocation = model.GetPickUpLocation(currBusiness);
                    var dropOffLocation = model.GetDropOffLocation(currBusiness);

                    var order = model.GetOrder(currBusiness);

                    order.DropOffLocation = dropOffLocation;
                    order.PickUpLocation = pickUpLocation;

                    // Create order
                    await _orderService.Value.CreateOrderAsync(order);

                    // Get nearest driver
                    var nearestDriver = await _driverApplicationService.Value.GetNearestDriverAsync(order);
                    if (nearestDriver != null)
                    {
                        using (var hubConnection = new HubConnection(_signalRConnection)
                        {
                            TraceLevel = TraceLevels.All,
                            TraceWriter = Console.Out
                        })
                        {
                            var hubProxy = hubConnection.CreateHubProxy("AddRiderHub");
                            hubConnection.Headers.Add("BusinessId", currBusiness.Id.ToString());

                            await hubConnection.Start();
                            var driverDetails = new DriverDetails(order, nearestDriver);
                            var result = await hubProxy.Invoke<ServiceResult>("NotifyBusiness", driverDetails);

                            if (!result.Success)
                                throw new Exception($"Error while notifying business about nearest driver.");
                        }
                    }
                    else
                    {
                        serviceResult.Messages.AddMessage(MessageType.Warning, "No driver was found for your order. Please try again a bit later.");
                    }

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Order was successfully submited.");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while creating new order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<ActionResult> AcceptDriver(AcceptRejectDriverViewModel model)
        {
            var serviceResult = new ServiceResult();
            using (var trasnaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid) throw new Exception(ModelState.GetModelStateErrorsAsString());

                    var driver = await _driverService.Value.GetByIdAsync<Driver>(model.DriverId);

                    if (driver == null) throw new Exception($"Cannot find driver by driver id: {model.DriverId}");

                    if (!driver.Approved) throw new Exception($"This driver is not approved by administration and is not allowed to proceed.");

                    if (driver.Status != RiderStatus.Online) throw new Exception($"You can assign only online drivers to the order.");

                    var order = await _orderService.Value.GetByIdAsync<Order>(model.OrderId);

                    if (order == null) throw new Exception($"Couldn't find an order with id: {model.OrderId}.");

                    if (order.OrderStatus != OrderStatus.Pending)
                        throw new Exception($"You cannot accept rider for this order.");

                    // Calculate order initial price
                    order.OrderPrice = await _orderService.Value.CalculateOrderPriceAsync(order, driver);

                    // Update order
                    await _orderService.Value.UpdateOrderAsync(order, order.Business.ContactPerson);

                    // Accept Driver
                    await _orderService.Value.AcceptDriverForOrderAsync(model.OrderId, model.DriverId);

                    // Notify driver that he/she recieved an order.
                    // TODO: move HubConnection into DI
                    using (var hubConnection = new HubConnection(_signalRConnection)
                    {
                        TraceLevel = TraceLevels.All,
                        TraceWriter = Console.Out
                    })
                    {
                        var hubProxy = hubConnection.CreateHubProxy("AddRiderHub");

                        await hubConnection.Start();
                        var orderDetails = new OrderDetails(order);
                        orderDetails.Duration = model.Duration;
                        orderDetails.DurationMins = model.Duration/60;

                        var sigResult = await hubProxy.Invoke<ServiceResult>("NotifyDriverAboutOrder", orderDetails, model.DriverId);

                        if (!sigResult.Success) throw new Exception(sigResult.DisplayMessage());
                    }



                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver was accepted for this order by the business.");
                    trasnaction.Commit();
                }
                catch (Exception ex)
                {
                    trasnaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while accepting driver for the order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderDetails(int orderId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var orderPreview = new OrderPreviewModel(await _orderService.Value.GetByIdAsync<Order>(orderId));

                serviceResult.Success = true;
                serviceResult.Data = orderPreview;
                serviceResult.Messages.AddMessage(MessageType.Info, "Order details was getted successfully");
            }
            catch (Exception e)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<ActionResult> CancelDriver(AcceptRejectDriverViewModel model)
        {
            var serviceResult = new ServiceResult();
            using (var trasnaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(model.DriverId);

                    if (driver == null) throw new Exception($"Cannot find driver by driver id: {model.DriverId}");

                    if (!driver.Approved) throw new Exception($"This driver is not approved by administration and is not allowed to proceed.");

                    var order = await _orderService.Value.GetByIdAsync<Order>(model.OrderId);

                    if (order == null) throw new Exception($"Couldn't find an order with id: {model.OrderId}.");

                    if (order.OrderStatus != OrderStatus.Pending)
                        throw new Exception($"You cannot accept rider for this order.");

                    await _orderService.Value.CancelDriverForOrderAsync(model.OrderId, model.DriverId);

                    await _businessPenaltyService.Value.PenelizeBusinessForRejectionAsync(driver, order);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver was canceled for this order by the business.");
                    trasnaction.Commit();
                }
                catch (Exception ex)
                {
                    trasnaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while canceling driver for the order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        public async Task<ActionResult> GetBusinessOrdesList(int draw, int start, int length)
        {
            var currentBusiness =
                await
                    _businessService.Value.GetBusinessByPersonId(
                        (await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId())).Id);

            var orders = (await _orderService.Value.GetAllEntitiesAsync<Order>(c => c.BusinessId == currentBusiness.Id, c => c.OrderByDescending(x => x.CreatedDt)))
                .Select(o => new BusinessOrder(o)).ToList();

            var param = new DataParam
            {
                Search = Request.QueryString["search[value]"],
                SortColumn = Request.QueryString["order[0][column]"] == null ? -1 : int.Parse(Request.QueryString["order[0][column]"]),
                SortDirection = Request.QueryString["order[0][dir]"] ?? "desc",
                Start = start,
                Draw = draw,
                Length = length
            };

            _ordersDataTable = new DataTable<BusinessOrder>(orders, param);

            return Json(_ordersDataTable.AjaxGetJsonData(), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = Roles.Business)]
        [HttpPost]
        public async Task<JsonResult> GetDriverDetailsForOrder(int driverId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var driver = (await _driverService.Value.GetByIdAsync<Driver>(driverId));
                if (driver == null) throw new Exception($"Driver was not found {driverId}.");

                var driverDetailsForOrder = new DriverDetailsForOrder
                {
                    FullName = $"{driver.Person.FirstName} {driver.Person.LastName}",
                    VehicleType = driver.VehicleType.ToString(),
                    RatingAverageScore = driver.Rating.AverageScore,
                    VehicleRegNumber = driver.VehicleRegistrationNumber
                };

                serviceResult.Success = true;
                serviceResult.Data = driverDetailsForOrder;
                serviceResult.Messages.AddMessage(MessageType.Info, "Driver Data for Order details was getted successfully");
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Internal Error");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult);
        }
    }
}