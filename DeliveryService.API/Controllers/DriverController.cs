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
using DeliveryService.API.Hubs;
using DeliveryService.API.ViewModel.Models;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Service;
using DAL.Constants;

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
                    result.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }
            return Json(result);
        }

        [System.Web.Http.Authorize]
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
                    driver.VehicleType = driverDetails.VehicleType;
                    driver.VehicleRegistrationNumber = driverDetails.VehicleRegistrationNumber;
                    await _driverService.Value.CreateDriverAsync(driver);

                    serviceResult.Data = null;
                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver Data was updated successfully");


                    var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driver.Id);
                    foreach (var document in driverDocuments)
                    {
                        if (driverDetails.VehicleType != VehicleType.Van && driverDetails.VehicleType != VehicleType.Car &&
                            driverDetails.VehicleType != VehicleType.Motorbike)
                        {
                            if (document.UploadType == UploadType.Insurance || document.UploadType == UploadType.License)
                                await _driverUploadService.Value.RemoveEntityAsync<DriverUpload>(document.Id);
                        }
                    }

                    int driverDocumentsCount = (await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driver.Id)).Count();

                    if (driverDetails.VehicleType == VehicleType.Van || driverDetails.VehicleType == VehicleType.Car ||
                        driverDetails.VehicleType == VehicleType.Motorbike)
                    {
                        if (driverDocumentsCount >= 4)
                        {
                            await _driverService.Value.ApproveDriverAsync(driver.Id, driver.Id);
                        }
                        else
                        {
                            await _driverService.Value.RejectDriverAsync(driver.Id, driver.Id);
                        }
                    }
                    else
                    {
                        if (driverDocumentsCount >= 2)
                        {
                            await _driverService.Value.ApproveDriverAsync(driver.Id, driver.Id);
                        }
                        else
                        {
                            await _driverService.Value.RejectDriverAsync(driver.Id, driver.Id);
                        }
                    }

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

        [System.Web.Http.Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> GetDriverDetails()
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                if (!User.IsInRole(Roles.Driver))
                {
                    throw new Exception("You have no permission");
                }

                var driver = await _driverService.Value.GetDriverByPersonAsync(User.Identity.GetUserId());
                
                var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driver.Id);
                var driverDocList = new List<DriverDocumentModel>();
                var driverVehicleType = driver.VehicleType;

                foreach (var document in driverDocuments)
                {
                    if (driverVehicleType != VehicleType.Van && driverVehicleType != VehicleType.Car &&
                        driverVehicleType != VehicleType.Motorbike)
                    {
                        if (document.UploadType == UploadType.Passport ||
                            document.UploadType == UploadType.ProofOfAddress
                            || document.UploadType == UploadType.Photo)
                        {
                            driverDocList.Add(new DriverDocumentModel
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
                    }
                    else
                    {
                        driverDocList.Add(new DriverDocumentModel
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
                }

                var driverDetails = new DriverDetails
                {
                    FirstName = driver.Person.FirstName,
                    LastName = driver.Person.LastName,
                    Email = driver.Person.Email,
                    DateOfBirth = driver.Person.DateOfBirth,
                    Phone = driver.Person.Phone,
                    Sex = driver.Person.Sex,
                    Addresses = driver.Addresses.ToList(),
                    DriverDocuments = driverDocList,
                    DriverId = driver.Id,
                    VehicleType = driver.VehicleType,
                    VehicleRegistrationNumber = driver.VehicleRegistrationNumber,
                    Approved = driver.Approved,
                    RatingAverageScore = (double)driver.Rating.AverageScore
                };

                serviceResult.Success = true;
                serviceResult.Data = driverDetails;

            }
            catch (Exception exception)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while getting driver details");
                serviceResult.Messages.AddMessage(MessageType.Error, exception.ToString());
            }

            return Json(serviceResult, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = Roles.Driver)]
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
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                    // throw;
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
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = Roles.Driver)]
        public async Task<IHttpActionResult> UpdateDriverLocation(DriverLocationModel model)
        {

            var serviceResult = new ServiceResult();
            try
            {
                if (!ModelState.IsValid) throw new Exception(GetModelStateErrorsAsString(ModelState));

                var driver = await _driverService.Value.GetByIdAsync<Driver>(model.DriverId);
                if (driver != null)
                {
                    var driverLocation = new DriverLocation
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Id = model.DriverId,
                        Lat = model.Lat,
                        Long = model.Long,
                        CreatedBy = model.DriverId,
                        CreatedDt = DateTime.Now,
                        UpdatedBy = model.DriverId,
                        UpdatedDt = DateTime.Now
                    };

                    await _driverLocationService.Value.UpdateDriverLocation(driverLocation);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "The driver location was updated.");
                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, $"No driver found for given Id {model.DriverId}");
                }

            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while updating driver's location.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

    }
}
