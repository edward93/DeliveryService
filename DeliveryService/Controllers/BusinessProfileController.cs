using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.Constants;
using DAL.Context;
using Infrastructure.Config;

namespace DeliveryService.Controllers
{
    [Authorize(Roles = Roles.Business)]
    public class BusinessProfileController : BaseController
    {
        // GET: BusinessProfile
        public BusinessProfileController(IConfig config, IDbContext context) : base(config, context)
        {
        }

        public ActionResult BusinessProfile()
        {
            return View();
        }
    }
}