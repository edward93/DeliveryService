using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Helpers;
using DeliveryService.ViewModels.Business;
using Infrastructure.Config;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers.Business
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessProfileController : BaseController
    {
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IBusinessUploadService> _businessUploadService;
        private FileUpload _fileUpload;
        // GET: BusinessProfile
        public BusinessProfileController(IConfig config, IDbContext context, IBusinessService businessService,
            IPersonService personService, IBusinessUploadService businessUploadService) : base(config, context)
        {
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
            _businessUploadService = new Lazy<IBusinessUploadService>(() => businessUploadService);
            _fileUpload = new FileUpload();
        }

        public async Task<ActionResult> BusinessProfile()
        {
            var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
            var business = await _businessService.Value.GetBusinessByPersonId(person.Id);
            var address = business.Addresses.Count == 0 ? new Address() : business.Addresses.ToList()[0];
            var businessUploads = (await _businessUploadService.Value.GetBusinessUploadsByBusinessIdAsync(business.Id)).
                    FirstOrDefault(b => b.UploadType == BusinessUploadType.BusinessProfile);

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
                Addressline2 = address.AddressLine2,
                BusinessLogo = businessUploads != null ? businessUploads.FileName : "",
                BusinessLogoId = businessUploads?.Id ?? 0
            };

            return View(previewBusiness);
        }

        //[HttpPost]
        //public async Task<JsonResult> GetBusinessProfileImage()
        //{
        //    var serviceResult = new ServiceResult();
        //    try
        //    {
        //        User.Identity.GetUserId();
        //        var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
        //        var business = await _businessService.Value.GetBusinessByPersonId(person.Id);
        //        var businessUploads = (await _businessUploadService.Value.GetBusinessUploadsByBusinessIdAsync(business.Id)).
        //            FirstOrDefault(b => b.UploadType == BusinessUploadType.BusinessProfile);

        //        var businessProfile = new BusinessProfile
        //        {
        //            FirstName = person.FirstName,
        //            LastName = person.LastName,
        //            Email = person.Email,
        //            MediaPath = businessUploads != null && System.IO.File.Exists(Request.PhysicalApplicationPath +
        //            "\\Documents\\userImages\\thumbs\\" + businessUploads.FileName + ".80x80.jpg") ? "/Documents/userImages/thumbs/" + businessUploads.FileName + ".80x80.jpg" : "/Documents/defaultImages/chmo.jpg"
        //        };

        //        serviceResult.Success = true;
        //        serviceResult.Data = businessProfile;
        //        serviceResult.Messages.AddMessage(MessageType.Info, "Business Profile Data was getted successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResult.Success = false;
        //        serviceResult.Messages.AddMessage(MessageType.Error, "Unhadled Error");
        //        serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
        //    }
        //    return Json(serviceResult);
        //}

        [HttpPost]
        public async Task<ActionResult> UploadBusinessDocument(BusinessUploadType uploadType)
        {
            var serviceResult = new ServiceResult();
            using (var transactoin = Context.Database.BeginTransaction())
            {
                try
                {
                    var expireDate = DateTime.UtcNow.AddYears(1);
                    var desc = System.Web.HttpContext.Current.Request.Form["Description"] ?? string.Empty;
                    var file = Request.Files[0];
                    if (file == null) throw new Exception("No file was found.");

                    var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());

                    var business = await _businessService.Value.GetBusinessByPersonId(person.Id);

                    var fileDetails = PrepareFile(file, uploadType);

                    var existingUpload =
                          await
                              _businessUploadService.Value.GetBusinessUploadByBusinessIdAndUploadTypeAsync(business.Id,
                                  uploadType);

                    if (existingUpload != null)
                    {
                        await _businessUploadService.Value.RemoveEntityAsync<BusinessUpload>(existingUpload.Id);
                    }

                    // Save to db
                    var businessDocument = await _businessUploadService.Value.CreateBusinessUploadAsync(new BusinessUpload
                    {
                        Business = business,
                        Description = desc,
                        DocumentStatus = DocumentStatus.WaitingForApproval,
                        ExpireDate = expireDate,
                        FileName = fileDetails.FileName,
                        UploadType = uploadType,
                        CreatedBy = business.ContactPerson.Id,
                        UpdatedBy = business.ContactPerson.Id,
                        UpdatedDt = DateTime.UtcNow,
                        CreatedDt = DateTime.UtcNow
                    });

                    var extension = Path.GetExtension(fileDetails.FilePath);
                    extension = extension?.Substring(1);

                    // Save file
                    file.SaveAs(fileDetails.FilePath);

                    // Save as thumbnail
                    var thumbnail = new WebImage(fileDetails.FilePath).Resize(200, 200);
                    thumbnail.Save(fileDetails.ThumbPath, extension);

                    transactoin.Commit();
                    serviceResult.Success = true;
                    serviceResult.Data = new
                    {
                        ThumbPath = fileDetails.ThumbPath,
                        FilePath = fileDetails.FilePath,
                        FileName = fileDetails.FileName,
                        DocumentId = businessDocument.Id,
                        BusinessId = businessDocument.BusinessId,
                        DocumentType = businessDocument.UploadType
                    };
                    serviceResult.Messages.AddMessage(MessageType.Info, "File was successfully uploaded.");
                }
                catch (Exception ex)
                {
                    transactoin.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while uploading file.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        private FileDetails PrepareFile(HttpPostedFileBase file, BusinessUploadType type)
        {
            // Name of the file
            var fileName = $"{Path.GetFileName(file.FileName)}_{DateTime.UtcNow.Timestamp()}{Path.GetExtension(file.FileName)}";
            // Relative path
            string filePath = $"{Config.UploadsFolderPath}/BusinessDocuments/{type}/";
            // Relative thumb path
            string thumbFilePath = $"{Config.UploadsFolderPath}/BusinessDocuments/{type}/thumbs/";

            // Absolute path
            var path = Server.MapPath(filePath);
            // Absolute thumb path
            var thumbPath = Server.MapPath(thumbFilePath);

            // Create dirs
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(thumbPath);

            if (!Utilities.HasImageExtension(fileName)) return null;

            return new FileDetails
            {
                FileName = fileName,
                FilePath = Path.Combine(path, fileName),
                ThumbName = fileName,
                ThumbPath = Path.Combine(thumbPath, fileName)
            };
        }


        public async Task<JsonResult> GetFileList()
        {
            try
            {
                var person = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                var business = await _businessService.Value.GetBusinessByPersonId(person.Id);
                var businessDocs = (await _businessUploadService.Value.GetBusinessUploadsByBusinessIdAsync(business.Id)).ToList();

                var result = new List<BusinessDocumentModel>();
                foreach (var businesDoc in businessDocs)
                {
                    result.Add(new BusinessDocumentModel
                    {
                        UploadType = businesDoc.UploadType,
                        ExpireDate = businesDoc.ExpireDate,
                        FileName = businesDoc.FileName,
                        DocumentId = businesDoc.Id
                    });
                }
                foreach (var file in result)
                {
                    file.IsFileExist = System.IO.File.Exists(
                        Path.Combine(
                            Server.MapPath($"{Config.UploadsFolderPath}/BusinessDocuments/{Enum.GetName(typeof(BusinessUploadType), file.UploadType)}/"),
                            file.FileName));
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(string controlId, string file, int id)
        {
            var serviceResult = new ServiceResult();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (id != 0)
                    {
                        var docToRemove = await _businessService.Value.GetByIdAsync<BusinessUpload>(id);
                        await _businessUploadService.Value.RemoveEntityAsync<BusinessUpload>(id);
                        var filePath =
                            Path.Combine(
                                Server.MapPath(
                                    $"{Config.UploadsFolderPath}/BusinessDocuments/{Enum.GetName(typeof(BusinessUploadType), docToRemove.UploadType)}"),
                                docToRemove.FileName);
                        var thumbPath =
                            Path.Combine(
                                Server.MapPath(
                                    $"{Config.UploadsFolderPath}/BusinessDocuments/{Enum.GetName(typeof(BusinessUploadType), docToRemove.UploadType)}/thumbs"),
                                docToRemove.FileName);

                        // Remove file 
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        // Remove thumbnail 
                        if (System.IO.File.Exists(thumbPath))
                        {
                            System.IO.File.Delete(thumbPath);
                        }
                    }

                    transaction.Commit();
                    serviceResult.Success = true;
                    serviceResult.Messages.AddMessage(MessageType.Info, "File was successfully removed.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while removing file.");
                    serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<ActionResult> GetBusinessShortInfo(string userId)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var contPerson = await _personService.Value.GetPersonByUserIdAsync(userId);

                if (contPerson == null) throw new Exception($"No person was found with user id {userId}.");
                var business = await _businessService.Value.GetBusinessByPersonId(contPerson.Id);

                if (business == null) throw new Exception($"No business was found for given contact person {contPerson.FirstName} {contPerson.LastName}");

                var businessUploads =
                    await _businessUploadService.Value.GetBusinessUploadByBusinessIdAndUploadTypeAsync(business.Id, BusinessUploadType.Logo);

                var viewModel = new BusinessViewModel
                {
                    BusinessId = business.Id,
                    BusinessContactPersonFullName = $"{contPerson.FirstName} {contPerson.LastName}",
                    BusinessName = business.BusinessName,
                    UserName = business.ContactPerson.User.UserName
                };

                if (businessUploads != null)
                {
                    viewModel.BusinessImageUrl = businessUploads.FileName;
                }

                serviceResult.Success = true;
                serviceResult.Messages.AddMessage(MessageType.Info, "Business Information is successfully retrieved.");
                serviceResult.Data = viewModel;
            }
            catch (Exception ex)
            {
                serviceResult.Success = false;
                serviceResult.Messages.AddMessage(MessageType.Error, "Error while retrieving business information.");
                serviceResult.Messages.AddMessage(MessageType.Error, ex.Message);
            }

            return Json(serviceResult);
        }
    }
}