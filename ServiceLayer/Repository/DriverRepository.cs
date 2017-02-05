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

namespace ServiceLayer.Repository
{
    public class DriverRepository : EntityRepository, IDriverRepository
    {
        public DriverRepository(IDbContext dbContext) : base(dbContext)
        {

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
    }
}
