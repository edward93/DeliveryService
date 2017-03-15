using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public interface IOrderRepository : IEntityRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task AcceptOrderAsync(Order order, Driver driver);
        Task AcceptDriverForOrderAsync(Driver driver, Order order);
        Task CancelDriverForOrderAsync(Order order);
        Task RejectOrderAsync(Order order, Driver driver);
        Task OnTheWayToPickUpAsync(Order order, Driver driver);
        Task ChangeOrderStatus(Order order, OrderStatus newStatus, Person person);
        Task ArrivedAtPickUpLocationAsync(Order order, Driver driver);
        Task OrderPickedUpAsync(Order order, Driver driver);
        Task OrderDelivieredAsync(Order order, Driver driver);
        Task OrderNotDeliveredAsync(Order order, Driver driver, string reason);
        Task <IEnumerable<Order>> GetBusinessOrdersAsync(int businessId, OrderStatus? status);
        Task<IEnumerable<Order>> GetBusinessActiveOrdersAsync(int businessId);
        Task<IEnumerable<Order>> GetOrdersThatShouldBeRejectedOnBehalfOfRider(int timeInSeconds);
    }
}