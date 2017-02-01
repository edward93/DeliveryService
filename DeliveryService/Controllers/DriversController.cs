using System;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Models;
using DeliveryService.Models.ViewModels;
using DeliveryService.ViewModels.Drivers;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{

    [Authorize(Roles = Roles.Admin)]
    public class DriversController : BaseController
    {

        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IDriverUploadService> _driverUploadService;
        private readonly Lazy<IPersonService> _personService;

        public DriversController(IConfig config,
            IDriverService driverService,
            IDriverUploadService uploadService,
            IDbContext context,
            IPersonService personService) : base(config, context)
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

        [System.Web.Mvc.HttpPost]
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

        

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> DeleteDriver(int driverId)
        {
            var result = await _driverService.Value.DeleteDriver(driverId);
            return Json(result);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> ApproveDriverDocument(int documentId)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    if (person != null)
                    {
                        var document = await _driverUploadService.Value.GetByIdAsync<DriverUpload>(documentId);
                        if (document.DocumentStatus == DocumentStatus.Approved ||
                            document.DocumentStatus == DocumentStatus.Rejected)
                            throw new Exception(
                                $"This document cannot be approved. Either this document is already approved or it has been rejected.");
                        await _driverUploadService.Value.ApproveDriverDocumentAsync(documentId, person.Id);
                        serviceResult.Success = true;
                        serviceResult.Messages.AddMessage(MessageType.Info, "The document was approved");
                        
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        serviceResult.Success = false;
                        serviceResult.Messages.AddMessage(MessageType.Error, $"Could not find current logged in person by user id {User.Identity.GetUserId()}");
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while approving the document (Id: {documentId})");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> RejectDriverDocument(DocumentRejectionViewModel model)
        {
            var serviceResult = new ServiceResult();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        scope.Dispose();
                        transaction.Rollback();
                        serviceResult.Success = false;
                        serviceResult.Messages.AddMessage(MessageType.Error, "Validation Errors");
                        serviceResult.Messages.AddMessage(MessageType.Error, ModelState.ToString());
                    }
                    else
                    {
                        var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                        if (person != null)
                        {
                            var document = await _driverUploadService.Value.GetByIdAsync<DriverUpload>(model.DocumentId);
                            if (document.DocumentStatus == DocumentStatus.Rejected)
                                throw new Exception("This document is already rejected.");
                            await _driverUploadService.Value.RejectDriverDocumentAsync(model.DocumentId, person.Id,
                                model.RejectionComment);

                            await _driverService.Value.RejectDriverAsync(document.DriverId, person.Id);

                            serviceResult.Success = true;
                            serviceResult.Messages.AddMessage(MessageType.Info, "The document was rejected");

                            scope.Complete();
                            transaction.Commit();
                        }
                        else
                        {
                            scope.Dispose();
                            transaction.Rollback();
                            serviceResult.Success = false;
                            serviceResult.Messages.AddMessage(MessageType.Error, "Internal Server Error");
                        }
                    }

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error,
                        $"Error while rejecting the document (Id: {model.DocumentId})");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }

            }
            return

            Json(serviceResult);
        }
    }
}