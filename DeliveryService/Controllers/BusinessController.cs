using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.ViewModels;
using Infrastructure.Config;
using Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    public class BusinessController : BaseController
    {
        private readonly Lazy<IBusinessService> _businessService;
        private readonly Lazy<IPersonService> _personService;
        // GET: Business
        public BusinessController(IConfig config, IBusinessService businessService, IPersonService personService, IDbContext context) : base(config, context)
        {
            _businessService = new Lazy<IBusinessService>(() => businessService);
            _personService = new Lazy<IPersonService>(() => personService);
        }

        public async Task<ActionResult> Index()
        {
            var driversList = await _businessService.Value.GetAllEntitiesAsync<Business>();
            return View(driversList);
        }

        [HttpPost]
        public async Task<JsonResult> RegisterDriver(RegisterBusinessModel registerBusiness)
        {
            var serviceResult = new ServiceResult();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        throw new Exception(ModelState.ToString());
                    }

                    var user = new User { UserName = registerBusiness.BusinessEmail, Email = registerBusiness.BusinessEmail };

                    IdentityResult result = await UserManager.CreateAsync(user, registerBusiness.Password);

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

                        var createBusiness = await _businessService.Value.CreateBusiness(new Business()
                        {
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
                            Id = contactPerson.Id
                        });

                        serviceResult.Messages.AddMessage(MessageType.Info, "The Business was successfuly created");
                        serviceResult.Success = true;

                        scope.Complete();
                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception($"Error while adding current user (UserId:{currentUser.Id}) to role ({Roles.Business}).");
                    }

                }
                catch (Exception exception)
                {
                    scope.Dispose();
                    transaction.Rollback();

                    serviceResult.Success = false;
                    serviceResult.Messages.AddMessage(MessageType.Error, "Error while registering business");
                    serviceResult.Messages.AddMessage(MessageType.Error, exception.ToString());
                }
            }

            return Json(serviceResult);
        }


    }
}