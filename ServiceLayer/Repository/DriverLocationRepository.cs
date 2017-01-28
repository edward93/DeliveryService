using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task<DriverLocation> FindNearestDriverLocationAsync(Order order)
        {
            var locations = await DbContext.DriverLocations.SqlQuery(@"SELECT * FROM DriverLocations 
                                                                                Where 
                                                                                        ((ACOS(SIN(@latParam * PI() / 180) * SIN(Lat * PI() / 180) + 
                                                                                        COS(@latParam * PI() / 180) * COS(Lat * PI() / 180) * COS((@lonParam - Long) *
                                                                                            PI() / 180)) *180 / PI()) *60 * 1.1515) <= 5", 
                                                                        new SqlParameter("latParam", order.PickUpLocation.Lat), 
                                                                        new SqlParameter("lonParam", order.PickUpLocation.Long)).ToListAsync();

            return locations.FirstOrDefault();
        }
    }
}