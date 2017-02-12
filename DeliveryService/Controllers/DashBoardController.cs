using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Context;
using DAL.Entities;
using DeliveryService.Models;
using Infrastructure.Config;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{

    [Authorize(Roles = "Admin")]
    public class DashBoardController : BaseController
    {
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IBusinessService> _businessService;
        // GET: DashBoard
        public DashBoardController(IConfig config, IDriverService driverService, IBusinessService businessService, IDbContext context) : base(config, context)
        {
            _driverService = new Lazy<IDriverService>(() => driverService);
            _businessService = new Lazy<IBusinessService>(() => businessService);
        }

        public async Task<ActionResult> Index()
        {
            return View(await GetSuperAdminDashboardModel());
        }

        private async Task<AdminDashBoardModel> GetSuperAdminDashboardModel()
        {
            var model = new AdminDashBoardModel
            {
                AllMembersCount = 0,
                TodayMembersCount = 0,
                AllDriversCount = (await _driverService.Value.GetAllEntitiesAsync<Driver>()).Count(),
                TodayDriversCount = 0,
                ActivePartnersCount =
                    (await _businessService.Value.GetAllEntitiesAsync<DAL.Entities.Business>()).Count(),
                InactivePartnersCount = 0,
                TodayAmount = 0,
                AllAmount = 0,
                TodayEarned = 0,
                AllEarned = 0,
                TodayAddRiderFee = 0,
                AllAddRiderFee = 0
            };


            return model;
        }
    }
}