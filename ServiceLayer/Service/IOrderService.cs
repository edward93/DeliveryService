using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IOrderService : IEntityService
    {
        Task<Order> CreateOrderAsync(Order order);
    }
}