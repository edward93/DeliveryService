using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using Infrastructure.Config;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessDashboardController : BaseController
    {
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IRiderService> _driverService;
        // GET: BusinessDashboard
        public BusinessDashboardController(IConfig config, IDbContext context, 
            IOrderService orderService, 
            IPersonService personService, 
            IBusinessService businessServicec, 
            IRiderService driverService) : base(config, context)
        {
            _driverService = new Lazy<IRiderService>(() => driverService);
            _personService = new Lazy<IPersonService>(() => personService);
            _businessService = new Lazy<IBusinessService>(() => businessServicec);
            _orderService = new Lazy<IOrderService>(() => orderService);
        }

        public async Task<ActionResult> DashBoard()
        {
            var currentBusiness =
                await
                    _businessService.Value.GetBusinessByPersonId(
                        (await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId())).Id);

            ViewBag.OrdersToday = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Where(o => o.CreatedDt.Date == DateTime.Today && o.BusinessId == currentBusiness.Id).ToList().Count;
            ViewBag.OnlineDriversCount = await _driverService.Value.GetOnlineDriversCountAsync();
            ViewBag.OrdersMonth = (await _orderService.Value.GetAllEntitiesAsync<Order>()).Where(o => o.CreatedDt.Month == DateTime.UtcNow.Month && o.BusinessId == currentBusiness.Id).ToList().Count;

            return View();
        }
    }
}