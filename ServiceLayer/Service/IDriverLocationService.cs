using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Service
{
    public interface IDriverLocationService : IEntityService
    {
        Task<DriverLocation> GetDriverLocationByDriverIdAsync(int driverId);
        Task UpdateDriverLocation(DriverLocation location);

        Task<IEnumerable<DriverLocation>> FindNearestDriverLocationAsync(Order order, int radiusMiles = 5);
    }
}