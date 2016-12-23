using System.Net.Http;
using System.Web.Http;
using Infrastructure.Config;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DeliveryService.API.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IConfig Config;
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

        public BaseApiController(IConfig config)
        {
            Config = config;
        }
    }
}