using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public class DriverLocationRepository : EntityRepository, IDriverLocationRepository
    {
        public DriverLocationRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DriverLocation> GetDriverLocationByDriverIdAsync(int driverId)
        {
            return await DbContext.DriverLocations.FirstOrDefaultAsync(c => c.Id == driverId);
        }

        public async Task UpdateDriverLocation(DriverLocation location)
        {
            DbContext.DriverLocations.AddOrUpdate(location);
            await DbContext.SaveChangesAsync();
        }
    }
}