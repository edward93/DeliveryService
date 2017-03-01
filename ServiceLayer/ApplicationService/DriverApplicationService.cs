using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Service;

namespace ServiceLayer.ApplicationService
{
    public class DriverApplicationService : IDriverApplicationService
    {
        private readonly Lazy<IDriverService> _driverService;
        private readonly Lazy<IOrderHistoryService> _orderHistoryService;
        private readonly Lazy<IDriverLocationService> _driverLocationService;

        public DriverApplicationService(IDriverService driverService,
            IOrderHistoryService orderHistoryService,
            IDriverLocationService driverLocationService)
        {
            _driverLocationService = new Lazy<IDriverLocationService>(() => driverLocationService);
            _orderHistoryService = new Lazy<IOrderHistoryService>(() => orderHistoryService);
            _driverService = new Lazy<IDriverService>(() => driverService);
        }

        public async Task<Driver> GetNearestDriverAsync(Order order)
        {
            var driverLocations = (await _driverLocationService.Value.FindNearestDriverLocationAsync(order)).ToList();
            if (!driverLocations.Any()) return null;

            var driversWhoRejetedOrder =
                await _orderHistoryService.Value.GetDriverIdsWhoRejectedOrderOrGotRejectedByBusinessAsync(order.Id);

            var nearestLocation = driverLocations.Where(c => !driversWhoRejetedOrder.Contains(c.Id)).ToList();

            var firstOrDefault = nearestLocation.FirstOrDefault();

            if (firstOrDefault != null)
            {
                return await _driverService.Value.GetByIdAsync<Driver>(firstOrDefault.Id);
            }
            return null;
        }
    }
}