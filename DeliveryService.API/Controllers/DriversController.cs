using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DAL.Entities;
using DAL.Enums;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriverController : BaseApiController
    {
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IPersonService> _personService;

        public DriverController(IDriverService service, IConfig config,
            IPersonService personService) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => service);
            _personService = new Lazy<IPersonService>(() => personService);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Test()
        {
            return Json("The unitiy for API seems fine");
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddDriver(Driver driver)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var currentPerson = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                driver.Approved = false;
                driver.Status = DriverStatus.Offline;
                driver.Person = currentPerson;
                driver.Id = currentPerson.Id;

                var createdDriver = await _driverService.Value.CreateDriverAsync(driver);

                result.Success = true;
                result.Data = createdDriver;
                result.Messages.Add(MessageType.Info, "The Driver was successfuly created");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add(MessageType.Error, "Error while creating driver");
                result.Messages.Add(MessageType.Error, ex.ToString());
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateDriver(Driver driver)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var updatedDriver = await _driverService.Value.UpdateDriverAsync(driver);

                result.Success = true;
                result.Data = updatedDriver;
                result.Messages.Add(MessageType.Info, "The Driver was successfuly created");

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add(MessageType.Error, "Error while creating driver");
                result.Messages.Add(MessageType.Error, ex.ToString());
            }
            return Json(result);
        }
    }
}
