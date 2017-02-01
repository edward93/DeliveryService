using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using Infrastructure.Config;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessDashboardController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        // GET: BusinessDashboard
        public BusinessDashboardController(IConfig config, IDbContext context, IOrderService orderService) : base(config, context)
        {
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        public async Task<ActionResult> DashBoard()
        {

            ViewBag.EarningsToday = 0.00;
            ViewBag.EarningsMonth = 0.00;
            ViewBag.OrdersToday = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Where(o => o.CreatedDt == DateTime.Today).ToList().Count;
            ViewBag.OrdersMonth = 1;

            return View();
        }
    }
}