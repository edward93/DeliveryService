using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Helpers.DataTableHelper;
using DeliveryService.Helpers.DataTableHelper.Models;
using DeliveryService.ViewModels.Business;
using DeliveryService.ViewModels.Orders;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessOrderController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IDriverService> _driverService;
        private DataTable<BusinessOrder> _ordersDataTable;

        public BusinessOrderController(IConfig config,
            IDbContext context,
            IOrderService orderService,
            IPersonService personService,
            IBusinessService businessService,
            IDriverService driverService) : base(config, context)
        {
            _driverService = new Lazy<IDriverService>(() => driverService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _orderService = new Lazy<IOrderService>(() => orderService);

        }

        public ActionResult BusinessOrders()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddNewOrder(MakeOrderViewModel model)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid) throw new Exception(ModelState.ToString());
                    var contPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    var currBusiness = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                    var pickUpLocation = model.GetPickUpLocation(currBusiness);
                    var dropOffLocation = model.GetDropOffLocation(currBusiness);

                    var order = model.GetOrder(currBusiness);

                    order.DropOffLocation = dropOffLocation;
                    order.PickUpLocation = pickUpLocation;

                    // Create order
                    await _orderService.Value.CreateOrderAsync(order);

                    // TODO: Find nearest driver
                    // TODO: Send this information to business via SignalR

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Order was successfully submited.");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while creating new order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<ActionResult> AcceptDriver(int orderId, int driverId)
        {
            var serviceResult = new ServiceResult();
            using (var trasnaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null) throw new Exception($"Cannot find driver by driver id: {driverId}");

                    if (!driver.Approved) throw new Exception($"This driver is not approved by administration and is not allowed to proceed.");

                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception($"Couldn't find an order with id: {orderId}.");

                    // Calculate order initial price
                    order.OrderPrice = await _orderService.Value.CalculateOrderPriceAsync(order, driver);

                    // Update order
                    await _orderService.Value.UpdateOrderAsync(order, order.Business.ContactPerson);

                    await _orderService.Value.AcceptDriverForOrderAsync(orderId, driverId);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver was accepted for this order by the business.");
                    trasnaction.Commit();
                }
                catch (Exception ex)
                {
                    trasnaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while accepting driver for the order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<ActionResult> CancelDriver(int orderId, int driverId)
        {
            var serviceResult = new ServiceResult();
            using (var trasnaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);

                    if (driver == null) throw new Exception($"Cannot find driver by driver id: {driverId}");

                    if (!driver.Approved) throw new Exception($"This driver is not approved by administration and is not allowed to proceed.");

                    var order = await _orderService.Value.GetByIdAsync<Order>(orderId);

                    if (order == null) throw new Exception($"Couldn't find an order with id: {orderId}.");

                    await _orderService.Value.CancelDriverForOrderAsync(orderId, driverId);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver was canceled for this order by the business.");
                    trasnaction.Commit();
                }
                catch (Exception ex)
                {
                    trasnaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while canceling driver for the order.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
                }
            }

            return Json(serviceResult);
        }

        public async Task<ActionResult> GetBusinessOrdesList(int draw, int start, int length)
        {
            var orders = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Select(o => new BusinessOrder(o)).ToList();

            var param = new DataParam
            {
                Search = Request.QueryString["search[value]"],
                SortColumn = Request.QueryString["order[0][column]"] == null ? -1 : int.Parse(Request.QueryString["order[0][column]"]),
                SortDirection = Request.QueryString["order[0][dir]"] ?? "asc",
                Start = start,
                Draw = draw,
                Length = length
            };

            _ordersDataTable = new DataTable<BusinessOrder>(orders, param);

            return Json(_ordersDataTable.AjaxGetJsonData(), JsonRequestBehavior.AllowGet);
        }
    }
}