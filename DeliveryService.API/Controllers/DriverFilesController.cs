using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.API.ApiModels;
using DeliveryService.API.ViewModel.Enums;
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
        public DriverFilesController(IConfig config, IDriverUploadService uploadService, IDriverService driverService) : base(config)
        {
            _driverUploadService = new Lazy<IDriverUploadService>(() => uploadService);
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddFileForDriver()
        {
            var serviceResult = new ServiceResult();
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
                    expireDate = Convert.ToDateTime(HttpContext.Current.Request.Form["ExpireDate"]);
                }
                else
                {
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "This request is not properly formatted");
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

                    await _driverUploadService.Value.CreateDriverUploadAsync(driverUpload);

                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "The file was successfully uploaded");
                }

            }
            catch (Exception e)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, e.Message);
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
