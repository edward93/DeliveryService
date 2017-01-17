using System;
using System.Threading.Tasks;
using System.Web.Http;
using DAL.Context;
using Infrastructure.Config;

namespace DeliveryService.API.Controllers
{
    public class OrderController : BaseApiController
    {
        public OrderController(IConfig config, IDbContext context) : base(config, context)
        {
        }

        public async Task<IHttpActionResult> AcceptOrder(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> RejectOrder(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> OnTheWayToPickUp(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> ArrivedAtThePickUpLocation(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> OrderPickedUp(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> Delivered(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> NotDelivered(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> BookReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> CancelReturn(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IHttpActionResult> ReturnConfirmed(int driverId, int orderId)
        {
            throw new NotImplementedException();
        }
    }
}