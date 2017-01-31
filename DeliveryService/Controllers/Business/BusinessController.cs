using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Helpers.DataTableHelper;
using DeliveryService.Helpers.DataTableHelper.Models;
using DeliveryService.ViewModels.Business;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;
using Newtonsoft.Json;

namespace DeliveryService.Controllers
{

    [Authorize(Roles = Roles.Admin)]
    public class BusinessController : BaseController
    {
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IPersonService> _personService;
        private DataTable<BusinessListItem> _ordersDataTable;
        // GET: Business
        public BusinessController(IConfig config, IBusinessService businessService, IPersonService personService, IDbContext context) : base(config, context)
        {
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetBusinessList(int draw, int start, int length)
        {
            var businesses = (await _businessService.Value.GetAllEntitiesAsync<DAL.Entities.Business>()).Select(o => new BusinessListItem(o)).ToList();

            var param = new DataParam
            {
                Search = Request.QueryString["search[value]"],
                SortColumn = Request.QueryString["order[0][column]"] == null ? -1 : int.Parse(Request.QueryString["order[0][column]"]),
                SortDirection = Request.QueryString["order[0][dir]"] ?? "asc",
                Start = start,
                Draw = draw,
                Length = length
            };

            _ordersDataTable = new DataTable<BusinessListItem>(businesses, param);

            return Json(_ordersDataTable.AjaxGetJsonData(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ContentResult> GetBusiness(int businessId)
        {
            var business = await _businessService.Value.GetByIdAsync<DAL.Entities.Business>(businessId);
            var businessAddress = business.Addresses.Count > 0 ? business.Addresses.ToList()[0] : new Address();
            var businessView = new RegisterBusinessModel()
            {
                BusinessId = business.Id,
                PhoneNumber = business.PhoneNumber,
                ContactPersonFirstName = business.ContactPerson.FirstName,
                ContactPersonLastName = business.ContactPerson.LastName,
                ContactPersonPhoneNumber = business.ContactPersonPhoneNumber,
                BusinessName = business.BusinessName,
                DateOfBirth = business.ContactPerson.DateOfBirth,
                BusinessEmail = business.BusinessEmail,
                AddressLine1 = businessAddress.AddressLine1,
                Addressline2 = businessAddress.AddressLine2,
                Country = businessAddress.Country,
                City = businessAddress.City,
                State = businessAddress.State,
                ZipCode = businessAddress.ZipCode
            };

            return Content(JsonConvert.SerializeObject(businessView,
               Formatting.None,
               new JsonSerializerSettings()
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }));
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBusiness(RegisterBusinessModel registerBusiness)
        {
            var serviceResult = new ServiceResult();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var business = await _businessService.Value.GetByIdAsync<DAL.Entities.Business>(registerBusiness.BusinessId);
                    var adminUser = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());
                    var businessAddress = registerBusiness.GetAddress();
                    var rating = business.Rating;

                    await _businessService.Value.CreateBusiness(new DAL.Entities.Business()
                    {
                        Id = business.ContactPerson.Id,
                        Addresses = new List<Address> { businessAddress },
                        BusinessEmail = registerBusiness.BusinessEmail,
                        PhoneNumber = registerBusiness.PhoneNumber,
                        BusinessName = registerBusiness.BusinessName,
                        ContactPersonPhoneNumber = registerBusiness.ContactPersonPhoneNumber,
                        CreatedDt = DateTime.UtcNow,
                        UpdatedDt = DateTime.UtcNow,
                        Approved = false,
                        CreatedBy = adminUser.Id,
                        UpdatedBy = adminUser.Id,
                        RatingId = business.RatingId,
                        Rating = business.Rating
                    });

                    scope.Complete();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while updating business");
                    serviceResult.Messages.AddMessage(MessageType.Error, e.ToString());
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<JsonResult> RegisterBusiness(RegisterBusinessModel registerBusiness)
        {
            var serviceResult = new ServiceResult();
           // using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
           // using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        throw new Exception(ModelState.ToString());
                    }

                    var user = new User { UserName = registerBusiness.BusinessEmail, Email = registerBusiness.BusinessEmail };

                    var result = await UserManager.CreateAsync(user, registerBusiness.Password);

                    if (!result.Succeeded)
                    {
                        throw new Exception("Error while creating User!");
                    }

                    // Get the newly created user
                    var currentUser = await UserManager.FindByEmailAsync(user.Email);
                    var adminUser = await _personService.Value.GetPersonByUserIdAsync(User.Identity.GetUserId());

                    var contactPerson = await _personService.Value.CreatePersonAsync(registerBusiness.GetPerson(currentUser, adminUser));
                    var roleResult = await UserManager.AddToRoleAsync(currentUser.Id, Roles.Business);

                    if (roleResult.Succeeded)
                    {
                        UserManager.AddClaim(currentUser.Id, new Claim(ClaimTypes.Role, Roles.Business));

                        var businessAddress = registerBusiness.GetAddress();

                        await _businessService.Value.CreateBusiness(new DAL.Entities.Business()
                        {
                            Id = contactPerson.Id,
                            Addresses = new List<Address> { businessAddress },
                            BusinessEmail = registerBusiness.BusinessEmail,
                            PhoneNumber = registerBusiness.PhoneNumber,
                            BusinessName = registerBusiness.BusinessName,
                            ContactPersonPhoneNumber = registerBusiness.ContactPersonPhoneNumber,
                            CreatedDt = DateTime.UtcNow,
                            UpdatedDt = DateTime.UtcNow,
                            Approved = false,
                            CreatedBy = adminUser.Id,
                            UpdatedBy = adminUser.Id,
                            Rating = new Rating
                            {
                                CreatedBy = contactPerson.Id,
                                UpdatedBy = contactPerson.Id,
                                AverageScore = 5,
                                CreatedDt = DateTime.Now,
                                UpdatedDt = DateTime.Now
                            }
                        });

                        serviceResult.Messages.AddMessage(MessageType.Info, "The Business was successfuly created");
                        serviceResult.Success = true;

                      //  scope.Complete();
                        //transaction.Commit();
                    }
                    else
                    {
                        throw new Exception($"Error while adding current user (UserId:{currentUser.Id}) to role ({Roles.Business}).");
                    }

                }
                catch (Exception exception)
                {
                   // scope.Dispose();
                    //transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while registering business");
                    serviceResult.Messages.AddMessage(MessageType.Error, exception.ToString());
                }
            }

            return Json(serviceResult);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteBusiness(int businessId)
        {
            var result = await _businessService.Value.RemoveEntityAsync<DAL.Entities.Business>(businessId);
            return Json(result);
        }

    }
}