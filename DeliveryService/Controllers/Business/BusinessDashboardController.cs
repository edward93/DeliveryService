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
            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            ViewBag.OrdersToday = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Where(o => o.CreatedDt == DateTime.Today).ToList().Count;
            ViewBag.OrdersMonth = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Where(o => o.CreatedDt >= firstDayOfMonth && o.CreatedDt <= lastDayOfMonth).ToList().Count;

            return View();
        }
    }
}