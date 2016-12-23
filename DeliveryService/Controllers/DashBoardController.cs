using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Config;

namespace DeliveryService.Controllers
{
    public class DashBoardController : BaseController
    {
        // GET: DashBoard
        public DashBoardController(IConfig config) : base(config)
        {

        }

        public ActionResult Index()
        {
            return View();

        }
    }
}