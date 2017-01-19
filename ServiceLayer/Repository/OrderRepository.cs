using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

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

        public async Task AcceptOrderAsync(Order order, Driver driver)
        {
            order.AssignedDriverId = driver.Id;
            order.OrderStatus = OrderStatus.Accepted;
            order.UpdatedDt = DateTime.UtcNow;
            order.UpdatedBy = driver.Id;

            DbContext.Orders.AddOrUpdate(order);
            await DbContext.SaveChangesAsync();
        }
    }
}