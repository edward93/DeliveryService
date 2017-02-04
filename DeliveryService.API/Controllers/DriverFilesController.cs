using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.API.ViewModel.Models;
using Infrastructure.Config;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriverFilesController : BaseApiController
    {
        private readonly Lazy<IDriverUploadService> _driverUploadService;
        private readonly Lazy<IDriverService> _driverService;
        public DriverFilesController(IConfig config, IDbContext context, IDriverUploadService uploadService, IDriverService driverService) : base(config, context)
        {
            _driverUploadService = new Lazy<IDriverUploadService>(() => uploadService);
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> DeleteDriverDocument(int Id)
        {
            var serviceResult = new ServiceResult();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var document = await _driverUploadService.Value.GetByIdAsync<DriverUpload>(Id);
                    if (document == null) throw new Exception($"No document found for given Id {Id}");

                    await _driverUploadService.Value.RemoveEntityAsync<DriverUpload>(Id);
                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Document was deleted successfully");


                    await _driverService.Value.RejectDriverAsync(document.DriverId, document.DriverId);
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver was rejected because of missing document");

                    scope.Complete();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while deleting document!");
                    serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetDriverDocuments()
        {
            var serviceResult = new ServiceResult();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var userId = User.Identity.GetUserId();
                    var driver = await _driverService.Value.GetDriverByPersonAsync(userId);
                    var driverDocuments = await _driverUploadService.Value.GetDriverUploadsByDriverIdAsync(driver.Id);

                    var driverVehicleType = driver.VehicleType;

                    var driverDocumentWithState = new DriverDocumentsWithState();
                    var driverDocList = new List<DriverDocumentModel>();
                    int counter = 0;
                    foreach (var document in driverDocuments)
                    {
                        if (document.DocumentStatus == DocumentStatus.Approved)
                        {
                            counter++;
                        }
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

                    if (driverVehicleType == VehicleType.Van || driverVehicleType == VehicleType.Car ||
                        driverVehicleType == VehicleType.Motorbike)
                    {
                        if (counter == 4)
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
                        if (counter > 2)
                        {
                            await _driverService.Value.ApproveDriverAsync(driver.Id, driver.Id);
                        }
                        else
                        {
                            await _driverService.Value.RejectDriverAsync(driver.Id, driver.Id);
                        }
                    }

                    driverDocumentWithState.Approved = await IsDriverApproved(driver.Id);
                    driverDocumentWithState.DriverDocumentsList = driverDocList;


                    serviceResult.Success = true;
                    serviceResult.Data = driverDocumentWithState;
                    serviceResult.Messages.AddMessage(MessageType.Info, "Driver Documents list was created successfully");

                    scope.Complete();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while getting driver documents!");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        private async Task<bool> IsDriverApproved(int driverId)
        {
            var driver = await _driverService.Value.GetByIdAsync<Driver>(driverId);
            return driver.Approved;
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddFileForDriver()
        {
            var serviceResult = new ServiceResult();

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var userId = User.Identity.GetUserId();
                    if (!Request.Content.IsMimeMultipartContent())
                    {
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                    }
                    var documentType = UploadType.Other;
                    var expireDate = DateTime.UtcNow;
                    var desc = HttpContext.Current.Request.Form["Description"] ?? string.Empty;

                    if (HttpContext.Current.Request.Form["DocumentType"] != null &&
                        HttpContext.Current.Request.Form["ExpireDate"] != null)
                    {
                        documentType =
                            (UploadType)Convert.ToInt32(HttpContext.Current.Request.Form["DocumentType"]);
                    }
                    else
                    {
                        serviceResult.Success = false;
                        serviceResult.Messages.AddMessage(MessageType.Error, "This request is not properly formatted");

                        transaction.Rollback();
                    }

                    string root = HttpContext.Current.Server.MapPath("~/App_Data");
                    var provider = new MultipartFormDataStreamProvider(root);


                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);

                    // This illustrates how to get the file names.
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                        {
                            serviceResult.Success = false;
                            serviceResult.Messages.AddMessage(MessageType.Error,
                                "This request is not properly formatted");

                            transaction.Rollback();
                        }

                        var document = PrepareFile(file, documentType);

                        if (!document.Item3) continue;

                        File.Move(file.LocalFileName, document.Item1);
                        var driver = await _driverService.Value.GetDriverByPersonAsync(userId);

                        var driverUpload = new DriverUpload
                        {
                            Driver = driver,
                            Description = desc,
                            DocumentStatus = DocumentStatus.WaitingForApproval,
                            ExpireDate = expireDate,
                            FileName = document.Item2,
                            UploadType = documentType,
                            CreatedBy = driver.Person.Id,
                            UpdatedBy = driver.Person.Id,
                            UpdatedDt = DateTime.UtcNow,
                            CreatedDt = DateTime.UtcNow
                        };

                        // Check if this driver has previously uploaded any document for the same type
                        // if he did the old document should be removed (marked as deleted)
                        var existingUpload =
                            await
                                _driverUploadService.Value.GetDriverUploadByDriverIdAndUploadTypeAsync(driver.Id,
                                    documentType);

                        if (existingUpload != null)
                        {
                            await _driverUploadService.Value.RemoveEntityAsync<DriverUpload>(existingUpload.Id);
                        }

                        var driverDocument = await _driverUploadService.Value.CreateDriverUploadAsync(driverUpload);

                        transaction.Commit();

                        serviceResult.Data = driverDocument.Id;
                        serviceResult.Success = true;
                        serviceResult.Messages.AddMessage(MessageType.Info, "The file was successfully uploaded");
                    }

                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
                }
            }


            return Json(serviceResult, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        private Tuple<string, string, bool> PrepareFile(MultipartFileData file, UploadType documentType)
        {
            string fileName = file.Headers.ContentDisposition.FileName;
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Trim('"');
            }
            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
            {
                fileName = Path.GetFileName(fileName);
            }

            string filePath = $"{Config.UploadsFolderPath}/DriverDocuments/";
            switch (documentType)
            {
                case UploadType.ProofOfAddress:
                    filePath += "ProofOfAddress";
                    break;
                case UploadType.Insurance:
                    filePath += "Insurance";
                    break;
                case UploadType.License:
                    filePath += "License";
                    break;
                case UploadType.Passport:
                    filePath += "Passport";
                    break;
                case UploadType.Photo:
                    filePath += "Photo";
                    break;
                default:
                    filePath += "Other";
                    break;
            }

            if (!Utilities.HasImageExtension(fileName)) return Tuple.Create("", "", false);

            var extension = Path.GetExtension(fileName);
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow.Timestamp()}{extension}";
            var path = Path.GetFullPath(HttpContext.Current.Server.MapPath("~/") + filePath);
            var filepath = Path.Combine(path, fileName);
            Directory.CreateDirectory(path);
            return Tuple.Create(filepath, fileName, true);
        }
    }
}

