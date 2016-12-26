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
        public Task<IHttpActionResult> AddFileForDriver()
        {
            ServiceResult result = new ServiceResult();

            return null;
        }

        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> PostFile()
        {
            var userId = User.Identity.GetUserId();

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            UploadType documentType;
            if (HttpContext.Current.Request.Form["documentType"] != null)
                documentType =
                   (UploadType)Convert.ToInt32(HttpContext.Current.Request.Form["documentType"]);
            else
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");

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
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    var document = PrepareFile(file, documentType);


                    File.Move(file.LocalFileName, document.Item1);
                    Driver driver = await _driverService.Value.GetDriverByPersonAsync(userId);

                    var driverUpload = new DriverUpload
                    {
                        Driver = driver,
                        IsApproved = false,
                        ExpireDate = DateTime.UtcNow,
                        FilePath = document.Item2,
                        UploadType = documentType,
                        CreatedBy = driver.Person.Id,
                        UpdatedBy = driver.Person.Id,
                        UpdatedDt = DateTime.UtcNow,
                        CreatedDt = DateTime.UtcNow
                    };

                    await _driverUploadService.Value.CreateDriverUpload(driverUpload);

                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private Tuple<string, string> PrepareFile(MultipartFileData file, UploadType documentType)
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

            string filePath = "~/Uploads/DriverDocuments/";
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

            fileName = GetTimestamp(DateTime.UtcNow) + fileName;
            var filepath = Path.Combine(HttpContext.Current.Server.MapPath(filePath), fileName);
            var path = HttpContext.Current.Server.MapPath(filePath);
            Directory.CreateDirectory(path);
            return Tuple.Create(path + "/" + fileName, fileName);
        }

        private string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }


    }
}
