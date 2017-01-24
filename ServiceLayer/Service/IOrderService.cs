using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IOrderService : IEntityService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task AcceptOrderAsync(Order order, Driver driver);
        Task AcceptDriverForOrderAsync(int orderId, int driverId);
        Task<decimal> CalculateOrderPriceAsync(Order order, Driver driver);
        Task UpdateOrderAsync(Order order, Person person);
        Task CancelDriverForOrderAsync(int orderId, int driverId);
    }
}