using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class DriverRepository : EntityRepository, IDriverRepository
    {
        public DriverRepository(IDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Driver> CreateDriverAsync(Driver driver)
        {
            DbContext.Drivers.AddOrUpdate(driver);
            await DbContext.SaveChangesAsync();
            return driver;
        }

        public async Task<Driver> GetDriverByPersonId(string personId)
        {
            return await DbContext.Drivers.FirstOrDefaultAsync(c => c.Person.UserId == personId);
        }
    }
}
