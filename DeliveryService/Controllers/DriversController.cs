﻿using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Entities;
using DAL.Enums;
using DAL.UnitOfWork;
using DeliveryService.Models;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Repository;
using ServiceLayer.Service;
namespace DeliveryService.Controllers
{
    public class DriversController : BaseController
    {

        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IDriverUploadService> _driverUploadService;
        private readonly Lazy<IPersonService> _personService;

        public DriversController(IConfig config,
            IDriverService driverService, 
            IDriverUploadService uploadService,
            IPersonService personService) : base(config)
        {
            _driverService = new Lazy<IDriverService>(() => driverService);
            _driverUploadService = new Lazy<IDriverUploadService>(() => uploadService);
            _personService = new Lazy<IPersonService>(() => personService);
        }
        public async Task<ActionResult> Index()
        {
            var driversList = await _driverService.Value.GetAllEntitiesAsync<Driver>();
            return View(driversList);
        }

        [HttpPost]
        public async Task<ContentResult> GetDriverDocuments(int driverId)
        {
            var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driverId);
            var list = JsonConvert.SerializeObject(driverDocuments,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return Content(list);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteDriver(int driverId)
        {
            var result = await _driverService.Value.DeleteDriver(driverId);
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetDriversList()
        {
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> ApproveDriverDocument(int documentId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                await _driverUploadService.Value.ApproveDriverDocumentAsync(documentId, person.Id);
                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "The document was approved");
            }
            catch (Exception)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, $"Error while approving the document (Id: {documentId})");
            }
            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<JsonResult> RejectDriverDocument(DocumentRejectionViewModel model)
        {
            var serviceResult = new ServiceResult();
            try
            {
                if (!ModelState.IsValid)
                {
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Validation Errors");
                    serviceResult.Messages.AddMessage(MessageType.Error, ModelState.ToString());
                }
                else
                {
                    var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    await _driverUploadService.Value.RejectDriverDocumentAsync(model.DocumentId, person.Id, model.RejectionComment);
                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "The document was rejected");
                }
                
            }
            catch (Exception)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, $"Error while rejecting the document (Id: {model.DocumentId})");
            }
            return Json(serviceResult);
        }
    }
}