using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DAL.Context;
using Infrastructure.Config;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DeliveryService.API.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IConfig Config;
        protected readonly IDbContext Context;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            protected set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; protected set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        public BaseApiController(IConfig config, IDbContext context)
        {
            Config = config;
            Context = context;
        }


        public string GetModelStateErrorsAsString(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            return modelState.Values.Aggregate(string.Empty,
                (current, state) => current + string.Join(Environment.NewLine, state.Errors.Select(c => c.ErrorMessage).ToList()));
        }
    }
}