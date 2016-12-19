using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Config;
using ServiceLayer.Repository;
using ServiceLayer.Service;

namespace DeliveryService.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICardService _cardService;

        public HomeController(IConfig config,
            ICardService service) : base(config)
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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}