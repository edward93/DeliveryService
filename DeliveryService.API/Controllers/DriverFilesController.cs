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
using ServiceLayer.Service;

namespace DeliveryService.API.Controllers
{
    public class DriverFilesController : BaseApiController
    {
        const string StoragePath = @"T:\WebApiTest";
        private readonly IDriverUploadService _driverUploadService;
        public DriverFilesController(IConfig config, IDriverUploadService service) : base(config)
        {
            _driverUploadService = service;
        }

        [HttpPost]
        public Task<IHttpActionResult> AddFileForDriver()
        {
            ServiceResult result = new ServiceResult();

            return null;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            DriverDocumentTypeEnum documentType;
            if (HttpContext.Current.Request.Form["documentType"] != null)
                documentType =
                   (DriverDocumentTypeEnum)Convert.ToInt32(HttpContext.Current.Request.Form["documentType"]);
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

                    File.Move(file.LocalFileName, PrepareFile(file, documentType));
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private string PrepareFile(MultipartFileData file, DriverDocumentTypeEnum documentType)
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
                case DriverDocumentTypeEnum.Address:
                    filePath += "Address";
                    break;
                case DriverDocumentTypeEnum.Insurance:
                    filePath += "Insurance";
                    break;
                case DriverDocumentTypeEnum.License:
                    filePath += "License";
                    break;
                case DriverDocumentTypeEnum.Passport:
                    filePath += "Passport";
                    break;
            }

            fileName = GetTimestamp(DateTime.UtcNow) + fileName;
            var filepath = Path.Combine(HttpContext.Current.Server.MapPath(filePath), fileName);
            var path = HttpContext.Current.Server.MapPath(filePath);
            Directory.CreateDirectory(path);
            return filepath;
        }

        private string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }


    }
}
