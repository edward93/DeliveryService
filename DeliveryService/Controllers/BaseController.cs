using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using DeliveryService.App_Start;
using Infrastructure.Config;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DeliveryService.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly IConfig Config;
        protected readonly IDbContext Context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public BaseController(IConfig config, IDbContext context)
        {
            Config = config;
            Context = context;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}