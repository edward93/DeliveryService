using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

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
        Task RejectOrderAsync(Order order, Driver driver);
        Task OnTheWayToPickUpAsync(Driver driver, Order order);
        Task ArrivedAtPickUpLocationAsync(Driver driver, Order order);
        Task OrderPickedUpAsync(Driver driver, Order order);
        Task OrderDeliveredAsync(Driver driver, Order order);
        Task OrderNotDeliveredAsync(Driver driver, Order order, string reason);
        Task<IEnumerable<Order>> GetBusinessOrdersAsync(int businessId, OrderStatus? status);
    }
}