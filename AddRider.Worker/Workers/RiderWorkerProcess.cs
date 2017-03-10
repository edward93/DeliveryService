using System;
using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Service;

namespace AddRider.Worker.Workers
{
    public class RiderWorkerProcess
    {
        private readonly IDriverService _driverService;
        public RiderWorkerProcess(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task RiderStatusProcessing()
        {
            //var allRiders = await _driverService.GetAllEntitiesAsync<Driver>();
            throw new NotImplementedException();
        }
    }
}