using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using DAL.Context;
using DAL.Enums;
using Infrastructure.Config;
using Infrastructure.Helpers;
using ServiceLayer.Repository;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICardService _cardService;

        public HomeController(IConfig config,
            IDbContext context,
            ICardService service) : base(config, context)
        {
            _cardService = service;
        }

        public async Task<ActionResult> Index()
        {
            var cardOfDriver = await _cardService.GetCardByDriverIdAsync(1);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = Config.Messages["TestMessage"];

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // TODO: This actions should be moved to admin or into the other controller
        public ActionResult GetCountries()
        {
            return Json(Utilities.ToSelectizeItemsList<Country>());
        }

        // TODO: This actions should be moved to admin or into the other controller
        public ActionResult GetVehicleTypes()
        {
            return Json(Utilities.ToSelectizeItemsList<VehicleType>());
        }
    }
}