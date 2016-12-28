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
            var userId = User.Identity.GetUserId();
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            UploadType documentType = UploadType.Other;
            if (HttpContext.Current.Request.Form["documentType"] != null)
                documentType =
                    (UploadType)Convert.ToInt32(HttpContext.Current.Request.Form["documentType"]);
            else
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "This request is not properly formatted");
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
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
                    if (document.Item3)
                    {
                        File.Move(file.LocalFileName, document.Item1);
                        Driver driver = await _driverService.Value.GetDriverByPersonAsync(userId);

                        var driverUpload = new DriverUpload
                        {
                            Driver = driver,
                            DocumentStatus = DocumentStatus.WaitingForApproval,
                            ExpireDate = DateTime.UtcNow,
                            FileName = document.Item2,
                            UploadType = documentType,
                            CreatedBy = driver.Person.Id,
                            UpdatedBy = driver.Person.Id,
                            UpdatedDt = DateTime.UtcNow,
                            CreatedDt = DateTime.UtcNow
                        };

                        await _driverUploadService.Value.CreateDriverUpload(driverUpload);
                        serviceResult.Success = true;
                        serviceResult.Messages.AddMessage(MessageType.Info, "The file was successfully uploaded");
                    }
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

            string filePath = "../DeliveryService/Uploads/DriverDocuments/";
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
            if (HasImageExtension(fileName))
            {
                var extension = Path.GetExtension(fileName);
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{GetTimestamp(DateTime.UtcNow)}{extension}";
                var path = Path.GetFullPath(HttpContext.Current.Server.MapPath("~/") + filePath);
                var filepath = Path.Combine(path, fileName);
                Directory.CreateDirectory(path);
                return Tuple.Create(filepath, fileName, true);
            }
            return Tuple.Create("", "", false);
        }

        public bool HasImageExtension(string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") ||
                source.EndsWith(".jpeg") || source.EndsWith(".tif") || source.EndsWith(".bmp"));
        }

        private string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }


    }
}
