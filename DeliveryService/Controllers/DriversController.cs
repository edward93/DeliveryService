using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Config;
using ServiceLayer.Repository;
using ServiceLayer.Service;
namespace DeliveryService.Controllers
{
    public class DriversController : BaseController
    {

        private readonly Lazy<IDriverService> _driverService;

        public DriversController(IConfig config,
            IDriverService service) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => service);
        }
        public async Task<ActionResult> Index()
        {
            var driversList = await _driverService.Value.GetDrivers();
            return View();
        }

        [HttpGet]
        public  ActionResult GetDriversList()
        {
             return null;
        }
    }
}