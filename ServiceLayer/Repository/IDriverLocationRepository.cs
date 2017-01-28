using System.Threading.Tasks;
using DAL.Entities;

namespace ServiceLayer.Repository
{
    public interface IDriverLocationRepository : IEntityRepository
    {
        Task<DriverLocation> GetDriverLocationByDriverIdAsync(int driverId);
        Task UpdateDriverLocation(DriverLocation location);
        Task<DriverLocation> FindNearestDriverLocationAsync(Order order);
    }
}