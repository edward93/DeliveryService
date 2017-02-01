using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Enums;

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

        public async Task<DriverLocation> FindNearestDriverLocationAsync(Order order, int radiusMiles = 5)
        {
            var locations = await DbContext.DriverLocations.SqlQuery(@"SELECT DriverLocations.* FROM DriverLocations inner Join Drivers on DriverLocations.Id = Drivers.Id
                                                                                Where 
                                                                                        ((ACOS(SIN(@latParam * PI() / 180) * SIN(Lat * PI() / 180) + 
                                                                                        COS(@latParam * PI() / 180) * COS(Lat * PI() / 180) * COS((@lonParam - Long) *
                                                                                            PI() / 180)) *180 / PI()) *60 * 1.1515) <= @radius 
                                                                                AND Drivers.Status = @driverStatus 
                                                                                AND Drivers.Approved = 1 
                                                                                AND Drivers.IsDeleted = 0", 
                                                                        new SqlParameter("latParam", order.PickUpLocation.Lat), 
                                                                        new SqlParameter("driverStatus", DriverStatus.Online),
                                                                        new SqlParameter("radius", radiusMiles),
                                                                        new SqlParameter("lonParam", order.PickUpLocation.Long)).ToListAsync();

            return locations.FirstOrDefault();
        }
    }
}