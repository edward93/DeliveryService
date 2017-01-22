using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Helpers;
using DeliveryService.ViewModels.Business;
using Infrastructure.Config;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessProfileController : BaseController
    {
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessUploadService> _businessUploadService;
        // GET: BusinessProfile
        public BusinessProfileController(IConfig config, IDbContext context, IBusinessService businessService,
            IPersonService personService, IBusinessUploadService businessUploadService) : base(config, context)
        {
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _businessUploadService = new Lazy<IBusinessUploadService>(() => businessUploadService);
        }

        public async Task<ActionResult> BusinessProfile()
        {
            var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
            var business = await _businessService.Value.GetBusinessByPersonId(person.Id);
            var address = business.Addresses.Count == 0 ? new Address() : business.Addresses.ToList()[0];

            PreviewBusinessModel previewBusiness = new PreviewBusinessModel()
            {
                Phone = business.PhoneNumber,
                BusinessEmail = business.BusinessEmail,
                DateOfBirth = business.ContactPerson.DateOfBirth,
                BusinessId = business.Id,
                BusinessName = business.BusinessName,
                ContactPersonFirstName = business.ContactPerson.FirstName,
                ContactPersonLastName = business.ContactPerson.LastName,
                ContactPersonPhoneNumber = business.ContactPersonPhoneNumber,
                Sex = business.ContactPerson.Sex,
                PhoneNumber = business.PhoneNumber,
                Country = address.Country,
                ZipCode = address.ZipCode,
                AddressLine1 = address.AddressLine1,
                City = address.City,
                State = address.State,
                Addressline2 = address.AddressLine2
            };

            return View(previewBusiness);
        }

        [HttpPost]
        public async Task<JsonResult> Upload()
        {
            try
            {
                string controlId = Request.Params["controlID"];
                var resultList = new List<ViewDataUploadFilesResult>();
                var currentContext = HttpContext;
                FileUpload fileUpload = InitUploader(controlId);
                fileUpload.FilesHelper.UploadAndShowResults(currentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);
                if (files.Files.Length > 0)
                {
                    var file = files.Files[0];
                    var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    var business = await _businessService.Value.GetBusinessByPersonId(person.Id);
                    var documentType = (BusinessUploadType)Enum.Parse(typeof(BusinessUploadType), controlId);
                    var existingUpload =
                          await
                              _businessUploadService.Value.GetBusinessUploadByBusinessIdAndUploadTypeAsync(business.Id,
                                  documentType);

                    if (existingUpload != null)
                    {
                        await _businessUploadService.Value.RemoveEntityAsync<BusinessUpload>(existingUpload.Id);
                    }
                    await _businessUploadService.Value.CreateBusinessUploadAsync(new BusinessUpload
                    {
                        Business = business,
                        Description = "",
                        DocumentStatus = DocumentStatus.WaitingForApproval,
                        ExpireDate = DateTime.Now,
                        FileName = file.Name,
                        UploadType = (BusinessUploadType)Enum.Parse(typeof(BusinessUploadType), controlId),
                        CreatedBy = business.ContactPerson.Id,
                        UpdatedBy = business.ContactPerson.Id,
                        UpdatedDt = DateTime.UtcNow,
                        CreatedDt = DateTime.UtcNow
                    });
                }

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                    return Json("Error ");
                return Json(files);
            }
            catch (Exception)
            {
                return Json("error");
            }
        }

        private FileUpload InitUploader(string controlId)
        {
            FileUploadConfig uploaderConfig = new FileUploadConfig()
            {
                TempPath = "~/BusinessDocs/",
                ServerMapPath = "~/Documents/Business/" + controlId,
                UrlBase = "~/Documents/Business/" + controlId,
                DeleteUrl = "/BusinessProfile/DeleteFile/?file=",
                DeleteType = "GET",
            };

            return new FileUpload(uploaderConfig);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(string controlId, string file, int id)
        {
            try
            {
                FileUpload fileUpload = InitUploader(controlId);
                fileUpload.FilesHelper.DeleteFile(file);

                if (id != 0)
                {
                    /*var resultFile = await _vehicleFileService.GetVehicleFileById(id);
                    await _businessUploadService.DeleteVehicleFile(id);*/
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }
    }
}