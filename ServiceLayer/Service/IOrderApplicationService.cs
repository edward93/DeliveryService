using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IOrderApplicationService
    {
        Task RejectOrderAsync(int orderId, int riderId);
        Task<IEnumerable<Order>> GetOrdersThatShouldBeRejectedOnBehalfOfRider(int timeInSeconds);
    }
}