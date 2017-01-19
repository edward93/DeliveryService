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
    public class BusinesRequestsController : BaseController
    {
        public BusinesRequestsController(IConfig config, IDbContext context) : base(config, context)
        {

        }

        public ActionResult BusinesRequests()
        {
            var drivers = new List<DAL.Entities.Driver>();
            return View(drivers);
        }
    }
}