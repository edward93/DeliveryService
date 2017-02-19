using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;
using DeliveryService.Models.ViewModels;

namespace ServiceLayer.Repository
{
    public class DriverRepository : EntityRepository, IDriverRepository
    {
        private readonly Lazy<IOrderRepository> _orderRepository;
        public DriverRepository(IDbContext dbContext,
            IOrderRepository orderRepository) : base(dbContext)
        {
            _orderRepository = new Lazy<IOrderRepository>(() => orderRepository);
        }

        public async Task<Driver> CreateDriverAsync(Driver driver)
        {
            try
            {
                DbContext.Drivers.AddOrUpdate(driver);
                await DbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return driver;
        }

        public async Task<Driver> GetDriverByPersonId(string personId)
        {
            return await DbContext.Drivers.FirstOrDefaultAsync(c => c.Person.UserId == personId);
        }

        public Task ApproveDriver(int driverId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetOnlineDriversCountAsync()
        {
            return
                await
                    DbContext.Drivers
                        .CountAsync(c => c.IsDeleted == false && c.Approved && c.Status == DriverStatus.Online);
        }

        public async Task<IEnumerable<DriverDetailsWithLocation>> GetOnlineDriversAsync()
        {
            var riders = await
                DbContext.Drivers.Where(c => c.Approved && c.IsDeleted == false && c.Status == DriverStatus.Online)
                    .ToListAsync();

            return riders.Select(c => new DriverDetailsWithLocation(c, null, null))
                    .ToList();
        }

        public async Task<IEnumerable<DriverDetailsWithLocation>> GetBusinessDriversAsync(int businessId)
        {
            var orders = await _orderRepository.Value.GetBusinessActiveOrdersAsync(businessId);
            var riders = (from order in orders
                where
                    order.AssignedDriver != null && 
                    order.AssignedDriver.Approved && order.AssignedDriver.IsDeleted == false &&
                    order.AssignedDriver.Status == DriverStatus.Busy
                select new DriverDetailsWithLocation(order.AssignedDriver, order, businessId)).ToList();

            return riders;
        }
    }
}
