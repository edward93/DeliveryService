using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class OrderRepository : EntityRepository, IOrderRepository
    {
        public OrderRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            DbContext.Orders.AddOrUpdate(order);
            await DbContext.SaveChangesAsync();
            return order;
        }
    }
}