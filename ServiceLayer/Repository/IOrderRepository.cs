using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IOrderRepository : IEntityRepository
    {
        Task<Order> CreateOrderAsync(Order order);
    }
}