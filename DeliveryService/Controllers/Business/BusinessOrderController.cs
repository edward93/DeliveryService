using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.ViewModels.Orders;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessOrderController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessService> _businessService;
        public BusinessOrderController(IConfig config, 
            IDbContext context, 
            IOrderService orderService, 
            IPersonService personService, 
            IBusinessService businessService) : base(config, context)
        {
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        public async Task<ActionResult> BusinessOrders()
        {
            var orders = await _orderService.Value.GetAllEntitiesAsync<Order>();
            return View(orders);
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
    }
}