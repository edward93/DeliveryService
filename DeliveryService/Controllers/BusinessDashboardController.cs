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
            ViewBag.EarningsToday = 0.01;
            ViewBag.EarningsMonth = 0.01;
            ViewBag.OrdersToday = 1;
            ViewBag.OrdersMonth = 1;

            return View();
        }
    }
}