using System.Web.Mvc;
using Infrastructure.Config;

namespace DeliveryService.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfig Config;

        public BaseController(IConfig config)
        {
            Config = config;
        }
    }
}