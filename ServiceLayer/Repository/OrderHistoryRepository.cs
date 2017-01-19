using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

namespace ServiceLayer.Repository
{
    public class OrderHistoryRepository : EntityRepository, IOrderHistoryRepository
    {
        public OrderHistoryRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<OrderHistory> CreateNewRecordAsync(OrderHistory record)
        {
            DbContext.OrderHistories.AddOrUpdate(record);
            await DbContext.SaveChangesAsync();
            return record;
        }

        public async Task<IEnumerable<OrderHistory>> GetRejectedOrdersByDriverForCurrentDayAsync(int driverId)
        {
            var orders =
                await DbContext.OrderHistories.Where(
                    c =>
                        c.DriverId == driverId && c.Action == ActionType.DriverRejectedOrder && c.IsDeleted == false &&
                        c.CreatedDt >= DateTime.UtcNow.AddHours(-24)).ToListAsync();

            return orders;
        }
    }
}