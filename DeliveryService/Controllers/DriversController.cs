using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Entities;
using Infrastructure.Config;
using ServiceLayer.Repository;
using ServiceLayer.Service;
namespace DeliveryService.Controllers
{
    public class DriversController : BaseController
    {

        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IDriverUploadService> _driverUploadService;

        public DriversController(IConfig config,
            IDriverService driverService, IDriverUploadService uploadService) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => driverService);
            _driverUploadService = new Lazy<IDriverUploadService>(() => uploadService);
        }
        public async Task<ActionResult> Index()
        {
            var driversList = await _driverService.Value.GetAllEntitiesAsync<Driver>();
            return View(driversList);
        }

        [HttpGet]
        public async Task<ActionResult> GetDriverDocuments(int driverId)
        {
            var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driverId);
            return Json(driverDocuments);
        }

        [HttpGet]
        public  ActionResult GetDriversList()
        {
             return null;
        }
    }
}