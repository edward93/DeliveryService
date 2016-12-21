using System.Web;
using System.Web.Mvc;
using Infrastructure.Config;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DeliveryService.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly IConfig Config;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public BaseController(IConfig config)
        {
            Config = config;
        }
        //[InjectionConstructor]
        //public BaseController(IPersonService personService)
        //{
        //    _personService = personService;
        //}

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var person = _personService.GetByUserId(userId);
        //        if (Session["User"] == null)
        //            Session["User"] = person;
        //    }
        //    base.OnActionExecuting(filterContext);

        //}

        //public PersonDto CurrentUser
        //{
        //    get
        //    {
        //        if (!Request.IsAuthenticated) return null;
        //        var userId = User.Identity.GetUserId();
        //        var person = _personService.GetByUserId(userId);
        //        if (Session["User"] == null)
        //            Session["User"] = person;
        //        return person;
        //    }
        //}

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