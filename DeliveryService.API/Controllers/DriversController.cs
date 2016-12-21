using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DAL.Entities;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriversController : BaseApiController
    {
        private readonly Lazy<IDriverService> _driverService;

        public DriversController(IDriverService service)
        {
            _driverService = new Lazy<IDriverService>(() => service);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Test()
        {

            var driverList = await _driverService.Value.GetDrivers();
            return Json("The unitiy for API seems fine");
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult AddDriver(Driver driver)
        {
            return Ok(_driverService.Value.AddDriver(driver));
        }
    }
}
