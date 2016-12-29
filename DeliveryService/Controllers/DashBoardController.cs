using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Entities;
using DeliveryService.Models;
using Infrastructure.Config;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    public class DashBoardController : BaseController
    {
        private readonly Lazy<IDriverService> _driverService;
        // GET: DashBoard
        public DashBoardController(IConfig config, IDriverService driverService) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        public async Task<ActionResult> Index()
        {
            return View(await GetSuperAdminDashboardModel());
        }

        private async Task<AdminDashBoardModel> GetSuperAdminDashboardModel()
        {
            var model = new AdminDashBoardModel();

            var driversList = await _driverService.Value.GetAllEntitiesAsync<Driver>();
            model.AllMembersCount = 0;
            model.TodayMembersCount = 0;
            model.AllDriversCount = driversList.ToList().Count;
            model.TodayDriversCount = 0;
            model.ActivePartnersCount = 0;
            model.InactivePartnersCount = 0;
            model.TodayAmount = 0;
            model.AllAmount = 0;
            model.TodayEarned = 0;
            model.AllEarned = 0;
            model.TodayCablrFee = 0;
            model.AllCablrFee = 0;

            return model;
        }
    }
}