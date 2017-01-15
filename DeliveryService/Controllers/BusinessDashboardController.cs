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
    public class BusinessDashboardController : BaseController
    {
        // GET: BusinessDashboard
        public BusinessDashboardController(IConfig config, IDbContext context) : base(config, context)
        {

        }

        public ActionResult DashBoard()
        {
            return View();
        }
    }
}