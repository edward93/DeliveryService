using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<DriverLocation>> FindNearestDriverLocationAsync(Order order, int radiusMiles = 5)
        {
            var sqlQuery = @"Select TMP.Id, TMP.Name, TMP.Address, TMP.Long, TMP.Lat, TMP.CreatedDt, TMP.CreatedBy, TMP.UpdatedDt, TMP.UpdatedBy, TMP.IsDeleted FROM 
                                (select DriverLocations.*, ((ACOS(SIN(@latParam * PI() / 180) * SIN(Lat * PI() / 180) + 
		                            COS(@latParam * PI() / 180) * COS(Lat * PI() / 180) * COS((@lonParam - Long) *
		                            PI() / 180)) *180 / PI()) *60 * 1.1515) as Distance 
                                FROM DriverLocations inner Join Drivers on (DriverLocations.Id = Drivers.Id) 
                                WHERE Drivers.Status = driverStatus AND Drivers.IsDeleted = 0 AND Drivers.Approved = 1) as TMP
                            WHERE TMP.Distance <= @radius 
                            ORDER By TMP.Distance";

            //var locations = await DbContext.DriverLocations.SqlQuery(@"SELECT DriverLocations.* FROM DriverLocations inner Join Drivers on DriverLocations.Id = Drivers.Id
            //                                                                    Where 
            //                                                                            ((ACOS(SIN(@latParam * PI() / 180) * SIN(Lat * PI() / 180) + 
            //                                                                            COS(@latParam * PI() / 180) * COS(Lat * PI() / 180) * COS((@lonParam - Long) *
            //                                                                                PI() / 180)) *180 / PI()) *60 * 1.1515) <= @radius 
            //                                                                    AND Drivers.Status = @driverStatus 
            //                                                                    AND Drivers.Approved = 1 
            //                                                                    AND Drivers.IsDeleted = 0",
            //                                                            new SqlParameter("latParam", order.PickUpLocation.Lat),
            //                                                            new SqlParameter("driverStatus", DriverStatus.Online),
            //                                                            new SqlParameter("radius", radiusMiles),
            //                                                            new SqlParameter("lonParam", order.PickUpLocation.Long)).ToListAsync();

            var locations = await DbContext.DriverLocations.SqlQuery(sqlQuery,
                                                                        new SqlParameter("latParam", order.PickUpLocation.Lat),
                                                                        new SqlParameter("driverStatus", RiderStatus.Online),
                                                                        new SqlParameter("radius", radiusMiles),
                                                                        new SqlParameter("lonParam", order.PickUpLocation.Long)).ToListAsync();

            return locations;
        }
    }
}