using System.Web.Http;
using Infrastructure.Config;

namespace DeliveryService.API.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IConfig Config;

        public BaseApiController(IConfig config)
        {
            Config = config;
        }
    }
}