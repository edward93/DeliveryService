using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class DriverLocationService : EntityService, IDriverLocationService
    {
        private readonly IDriverLocationRepository _driverLocationRepository;
        public DriverLocationService(IEntityRepository entityRepository,
            IDriverLocationRepository repository) : base(entityRepository)
        {
            _driverLocationRepository = repository;
        }

        public async Task<DriverLocation> GetDriverLocationByDriverIdAsync(int driverId)
        {
            return await _driverLocationRepository.GetDriverLocationByDriverIdAsync(driverId);
        }

        public async Task UpdateDriverLocation(DriverLocation location)
        {
            await _driverLocationRepository.UpdateDriverLocation(location);
        }

        public async Task<IEnumerable<DriverLocation>> FindNearestDriverLocationAsync(Order order, int radiusMiles = 5)
        {
            return await _driverLocationRepository.FindNearestDriverLocationAsync(order);
        }
    }
}