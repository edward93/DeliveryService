using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using Infrastructure.Config;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    [Authorize(Roles = Roles.Business)]
    public class BusinesOrdersController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        public BusinesOrdersController(IConfig config, IDbContext context, IOrderService orderService) : base(config, context)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        public async Task<ActionResult> BusinesOrders()
        {
            var orders = await _orderService.Value.GetAllEntitiesAsync<Order>();
            return View(orders);
        }
    }
}