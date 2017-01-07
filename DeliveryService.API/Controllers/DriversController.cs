﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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

        public DriverController(IDriverService service, IConfig config,
            IPersonService personService, IDriverUploadService driverUploadService) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => service);
            _personService = new Lazy<IPersonService>(() => personService);
            _driverUploadService = new Lazy<IDriverUploadService>(() => driverUploadService);
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
                result.Messages.AddMessage(MessageType.Info, Config.Messages["DriverCreationSuccess"]);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.AddMessage(MessageType.Error, "Error while creating driver");
                result.Messages.AddMessage(MessageType.Error, ex.ToString());
            }
            return Json(result);
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
                            RejectionComment = document.RejectionComment
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
        public async Task<IHttpActionResult> UpdateDriver(Driver driver)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var updatedDriver = await _driverService.Value.UpdateDriverAsync(driver);

                result.Success = true;
                result.Data = updatedDriver;
                result.Messages.AddMessage(MessageType.Info, "The Driver was successfuly created");

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.AddMessage(MessageType.Error, "Error while creating driver");
                result.Messages.AddMessage(MessageType.Error, ex.ToString());
            }
            return Json(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}
