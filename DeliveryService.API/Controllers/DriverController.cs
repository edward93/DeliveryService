using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.API.ViewModel.Models;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriverController : BaseApiController
    {
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IDriverUploadService> _driverUploadService;
        private readonly Lazy<IDriverLocationService> _driverLocationService;

        public DriverController(IDriverService service, IConfig config, IDbContext context,
            IPersonService personService, 
            IDriverUploadService driverUploadService,
            IDriverLocationService driverLocationService) : base(config, context)
        {
            _driverService = new Lazy<IDriverService>(() => service);
            _personService = new Lazy<IPersonService>(() => personService);
            _driverUploadService = new Lazy<IDriverUploadService>(() => driverUploadService);
            _driverLocationService = new Lazy<IDriverLocationService>(() => driverLocationService);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddDriver(Driver driver)
        {
            ServiceResult result = new ServiceResult();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
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
                    result.Messages.AddMessage(MessageType.Info, Config.Messages["DriverCreationSuccess"]);

                    scope.Complete();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    result.Success = false;
                    result.Messages.AddMessage(MessageType.Error, "Error while creating driver");
                    result.Messages.AddMessage(MessageType.Error, ex.ToString());
                }
            }
            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDriver(DriverDetails driverDetails)
        {
            ServiceResult serviceResult = new ServiceResult();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var driver = await _driverService.Value.GetDriverByPersonAsync(User.Identity.GetUserId());
                    if (driver.Addresses.Count > 0 && driverDetails.Addresses.Count > 0)
                    {
                        var driverOldAddress = driver.Addresses.ToList()[0];
                        var currentAddress = driverDetails.Addresses[0];

                        var addresses = new List<Address>();
                        driverOldAddress.AddressLine1 = currentAddress.AddressLine1;
                        driverOldAddress.AddressLine2 = currentAddress.AddressLine2;
                        driverOldAddress.City = currentAddress.City;
                        driverOldAddress.Country = currentAddress.Country;
                        driverOldAddress.ZipCode = currentAddress.ZipCode;
                        addresses.Add(driverOldAddress);
                        driver.Addresses = addresses;
                    }

                    var person = driver.Person;
                    person.FirstName = driverDetails.FirstName;
                    person.LastName = driverDetails.LastName;
                    person.Phone = driverDetails.Phone;
                    person.Email = driverDetails.Email;
                    person.Sex = driverDetails.Sex;
                    person.DateOfBirth = driverDetails.DateOfBirth;
                    person.UpdatedDt = DateTime.Now;
                    driver.Person = person;
                    driver.UpdatedDt = DateTime.Now;
                    await _driverService.Value.CreateDriverAsync(driver);

                    serviceResult.Data = null;
                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver Data was updated successfully");

                    scope.Complete();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Something went wrong");
                    serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
                }
            }
            return

            Json(serviceResult);
        }

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> GetDriverDetails()
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var driver = await _driverService.Value.GetDriverByPersonAsync(User.Identity.GetUserId());
                if (driver != null)
                {
                    var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driver.Id);
                    var driverDocList = new List<DriverDocumentModel>();
                    foreach (var document in driverDocuments)
                    {
                        driverDocList.Add(new DriverDocumentModel()
                        {
                            DocumentType = document.UploadType,
                            FileName = document.FileName,
                            Description = document.Description,
                            DocumentStatus = document.DocumentStatus,
                            ExpireDate = document.ExpireDate,
                            RejectionComment = document.RejectionComment,
                            DocumentId = document.Id
                        });
                    }
                    var driverDetails = new DriverDetails()
                    {
                        FirstName = driver.Person.FirstName,
                        LastName = driver.Person.LastName,
                        Email = driver.Person.Email,
                        DateOfBirth = driver.Person.DateOfBirth,
                        Phone = driver.Person.Phone,
                        Sex = driver.Person.Sex,
                        Addresses = driver.Addresses.ToList(),
                        DriverDocuments = driverDocList
                    };

                    result.Success = true;
                    result.Data = driverDetails;
                }
                else
                {
                    result.Success = false;
                    result.Messages.AddMessage(MessageType.Error, "Driver was not found");
                }
            }
            catch (Exception exception)
            {
                result.Success = false;
                result.Messages.AddMessage(MessageType.Error, "Error while creating driver");
                result.Messages.AddMessage(MessageType.Error, exception.ToString());
            }

            return Json(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> ChangeDriverStatus(int driverId, DriverStatus newStatus)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (newStatus == DriverStatus.Busy) throw new Exception("Driver cannot change it's status to Busy");

                    await _driverService.Value.ChangeDriverStatusAsync(driverId, newStatus);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, $"Driver Status was changed to {newStatus}.");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, $"Error while changing driver's status to {newStatus}");

                    /* TODO: This should be changed to ex.Message to display only messages 
                       TODO: and ex.Tostring for logging to display more detailed information about error in a log file */
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
                    throw;
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetDriverLocation(int driverId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var location = await _driverLocationService.Value.GetDriverLocationByDriverIdAsync(driverId);

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "Location successfully retrieved.");
                serviceResult.Data = location;
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while retrieving driver's location.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateDriverLocation(DriverLocation model)
        {
            var serviceResult = new ServiceResult();
            try
            {
                if (!ModelState.IsValid) throw new Exception(ModelState.ToString());

                await _driverLocationService.Value.UpdateDriverLocation(model);

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "The driver location was updated.");
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while updating driver's location.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.ToString());
            }

            return Json(serviceResult, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

    }
}
