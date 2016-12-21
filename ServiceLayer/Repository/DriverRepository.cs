using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class DriverRepository: EntityRepository, IDriverRepository
    {
        public DriverRepository(IDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Driver>> GetDriversList()
        {
            return await DbContext.Drivers.ToListAsync();
        }

        public async Task<Driver> GetDriverById(int id)
        {
            return await DbContext.Drivers.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
