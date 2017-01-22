using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DeliveryService.ViewModels.Orders;
using Infrastructure.Config;
using Infrastructure.Helpers;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessOrderController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        public BusinessOrderController(IConfig config, IDbContext context, IOrderService orderService) : base(config, context)
        {
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
            try
            {

            }
            catch (Exception ex)
            {
        
            }

            return Json(serviceResult);
        }
    }
}